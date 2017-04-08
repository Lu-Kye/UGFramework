using System.Collections;
using System.Collections.Generic;
using SR = System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using System;

namespace UGFramework.Editor.Inspector
{
    public class MemberInfo
    {
        object _target;
        public object Target 
        { 
            get { return _target; }
            set { _target = value; }
        }

        string _name;
        public string Name 
        { 
            get 
            { 
                if (this.IsField)
                    return this.FieldInfo.Name;
                else if (this.IsProperty)
                    return this.PropertyInfo.Name;
                return _name;
            } 
            set 
            {
                _name = value;
            }
        }

        public string Tooltip { get; set; }

        public bool IsUnitySerializable
        {
            get
            {
                if (
                    this.Type.IsGenericType && this.Type.GetGenericTypeDefinition() == typeof(LinkedList<>) ||
                    this.Type.IsGenericType && this.Type.GetGenericTypeDefinition() == typeof(Dictionary<,>)
                )
                {
                    return false;
                }
                return true;
            }
        }


        bool _isReadonly = true;
        /**
         * Private field and properties are readonly
         */
        public bool IsReadonly 
        {
            get
            {
                if (this.IsField)
                {
                    var attributes = this.FieldInfo.GetCustomAttributes(typeof(ShowInInspector), true);
                    var attribute = attributes.Length > 0 ? attributes[0] as ShowInInspector : null;
                    if (attribute != null && attribute.IsReadonly)
                        return true;

                    if (this.Type.IsPublic)
                        return false;

                    return this.Type.IsDefined(typeof(SerializeField), true);
                }
                else if (this.IsProperty)
                {
                    return true;
                }
                return _isReadonly;
            }
            set
            {
                _isReadonly = value;
            }
        }

        public System.Type Type
        {
            get
            {
                if (this.IsField)
                    return this.FieldInfo.FieldType;
                else if (this.IsProperty)
                    return this.PropertyInfo.PropertyType;
                return this.Target.GetType();
            }
        }

        public object Value
        {
            get 
            {
                return this.GetValue<object>();
            }
        }

        public bool IsEnumerable 
        {
            get
            {
                return this.Type.GetInterface("IEnumerable") != null;
            }
        }

        public SR.FieldInfo FieldInfo { get; private set; }
        public bool IsField { get { return this.FieldInfo != null; } }

        public SR.PropertyInfo PropertyInfo { get; private set; }
        public bool IsProperty { get { return this.PropertyInfo != null; } }

        public MemberInfo(object target, SR.FieldInfo info)            
        {
            _name = null;
            _target = target;
            this.FieldInfo = info;
            this.PropertyInfo = null;
            _isReadonly = false;

            var attributes = info.GetCustomAttributes(typeof(InspectorTooltip), true);
            var attribute = attributes.Length > 0 ? attributes[0] as InspectorTooltip : null;
            this.Tooltip = attribute != null ? attribute.Tooltip : info.Name;
        }

        public MemberInfo(object target, SR.PropertyInfo info)            
        {
            _name = null;
            _target = target;
            this.FieldInfo = null;
            this.PropertyInfo = info;
            _isReadonly = true;

            var attributes = info.GetCustomAttributes(typeof(InspectorTooltip), true);
            var attribute = attributes.Length > 0 ? attributes[0] as InspectorTooltip : null;
            this.Tooltip = attribute != null ? attribute.Tooltip : info.Name;
        }

        public MemberInfo(object target, string name, string tooltip = null)
        {
            _name = name;
            _target = target;
            this.FieldInfo = null;
            this.PropertyInfo = null;
            _isReadonly = false;
            this.Tooltip = tooltip;
        }

        public T GetValue<T>()
        {
            if (this.IsField)
                return (T)this.FieldInfo.GetValue(this.Target);
            else if (this.IsProperty)
                return (T)this.PropertyInfo.GetGetMethod(true).Invoke(this.Target, null);
            return (T)this.Target;
        }

        public void SetValue(object value)
        {
            if (this.IsField)
                this.FieldInfo.SetValue(this.Target, value);
            else if (this.IsProperty)
                return;
            this.Target = value;
        }

        public List<object> GetList()
        {
            var values = new List<object>();
            var iValues = this.GetValue<ICollection>();
            if (iValues == null)
                return values;
            foreach (var value in iValues)
            {
                values.Add(value);
            }
            return values;
        }

        public void SetList(List<object> values)
        {
            var json = JsonConvert.SerializeObject(values);
            var value = JsonConvert.DeserializeObject(json, this.Type);
            this.SetValue(value);
        }

        [Serializable]
        public class DictElement
        {
            public object Key;
            public object Value;
        }

        public List<DictElement> GetDict()
        {
            var values = new List<DictElement>();
            var iValues = this.GetValue<IDictionary>();
            var keyType = this.GetValue<IDictionary>().GetType().GetGenericArguments()[0];
            var valueType = this.GetValue<IDictionary>().GetType().GetGenericArguments()[1];
            if (iValues == null)
                return values;
            foreach (var key in iValues.Keys)
            {
                var element = new DictElement();
                element.Key = Convert.ChangeType(key, keyType);
                element.Value = Convert.ChangeType(iValues[key], valueType);
                values.Add(element);
            }
            return values;
        }

        public void SetDict(List<DictElement> values)
        {
            var iValues = this.GetValue<IDictionary>();
            iValues.Clear();
            foreach (var pair in values)
            {
                if (iValues.Contains(pair.Key))
                    continue;
                iValues.Add(pair.Key, pair.Value);
            }
        }
    }
}
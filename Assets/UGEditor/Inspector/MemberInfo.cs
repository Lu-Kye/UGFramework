using System.Collections;
using System.Collections.Generic;
using SR = System.Reflection;
using UnityEngine;

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


        bool _isReadonly;
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

        public bool IsCollection 
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

        public List<object> GetList()
        {
            var values = new List<object>();
            var iValues = this.GetValue<IList>();
            if (iValues == null)
                return values;
            for (int i = 0; i < iValues.Count; ++i)
            {
                values.Add((object)iValues[i]);
            }
            return values;
        }

        public void SetList(List<object> values)
        {
            var iValues = this.GetValue<IList>();
            iValues.Clear();
            for (int i = 0; i < values.Count; ++i)
            {
                iValues.Add(values[i]);
            }
            this.SetValue(iValues);
        }

        public void SetValue(object value)
        {
            if (this.IsField)
                this.FieldInfo.SetValue(this.Target, value);
            else if (this.IsProperty)
                return;
            this.Target = value;
        }
    }
}
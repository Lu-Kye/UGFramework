using SR = System.Reflection;

namespace UGFramework.Editor.Inspector
{
    public struct MemberInfo
    {
        public object Target { get; private set; }

        public string Name { get { return this.Info.Name; } }

        public string Tooltip { get; set; }

        /**
         * Private field and properties are readonly
         */
        public bool IsReadonly 
        {
            get
            {
                if (this.IsField)
                {
                    var attributes = this.Info.GetCustomAttributes(typeof(ShowInInspector), false);
                    var attribute = attributes.Length > 0 ? attributes[0] as ShowInInspector : null;
                    if (attribute != null && attribute.IsReadonly)
                        return true;

                    return this.FieldInfo.IsPublic == false;
                }
                return true;
            }
        }

        public System.Type Type
        {
            get
            {
                if (this.IsField)
                    return this.FieldInfo.FieldType;
                return this.PropertyInfo.PropertyType;
            }
        }

        public bool IsCollection 
        {
            get
            {
                return this.Type.GetInterface("IEnumerable") != null;
            }
        }

        public SR.MemberInfo Info
        {
            get
            {
                return this.IsField ? (SR.MemberInfo)this.FieldInfo : (SR.MemberInfo)this.PropertyInfo; 
            }
        }

        public SR.FieldInfo FieldInfo { get; private set; }
        public bool IsField { get { return this.FieldInfo != null; } }

        public SR.PropertyInfo PropertyInfo { get; private set; }
        public bool IsProperty { get { return this.PropertyInfo != null; } }

        public MemberInfo(object target, SR.FieldInfo info)            
        {
            this.Target = target;
            this.FieldInfo = info;
            this.PropertyInfo = null;

            var attributes = info.GetCustomAttributes(typeof(InspectorTooltip), false);
            var attribute = attributes.Length > 0 ? attributes[0] as InspectorTooltip : null;
            this.Tooltip = attribute != null ? attribute.Tooltip : info.Name;
        }

        public MemberInfo(object target, SR.PropertyInfo info)            
        {
            this.Target = target;
            this.FieldInfo = null;
            this.PropertyInfo = info;

            var attributes = info.GetCustomAttributes(typeof(InspectorTooltip), false);
            var attribute = attributes.Length > 0 ? attributes[0] as InspectorTooltip : null;
            this.Tooltip = attribute != null ? attribute.Tooltip : info.Name;
        }

        public T GetValue<T>()
        {
            if (this.IsField)
                return (T)this.FieldInfo.GetValue(this.Target);
            return (T)this.PropertyInfo.GetGetMethod(true).Invoke(this.Target, null);
        }

        public void SetValue(object value)
        {
            if (this.IsField)
                this.FieldInfo.SetValue(this.Target, value);

            // if (this.PropertyInfo.GetSetMethod(true) != null)
            //     this.PropertyInfo.SetValue(this.Target, value, null);
            return;
        }
    }
}
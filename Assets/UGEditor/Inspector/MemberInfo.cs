using SR = System.Reflection;

namespace UGFramework.Editor.Inspector
{
    public struct MemberInfo
    {
        public object Target { get; private set; }

        public string Name { get { return this.Info.Name; } }

        public System.Type Type
        {
            get
            {
                if (this.IsField)
                    return this.FieldInfo.FieldType;
                return this.PropertyInfo.PropertyType;
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
        }

        public MemberInfo(object target, SR.PropertyInfo info)            
        {
            this.Target = target;
            this.FieldInfo = null;
            this.PropertyInfo = info;
        }

        public T GetValue<T>()
        {
            if (this.IsField)
                return (T)this.FieldInfo.GetValue(this.Target);
            return (T)this.PropertyInfo.GetGetMethod().Invoke(this.Target, null);
        }

        public int GetInt()
        {
            return this.GetValue<int>();
        }
    }
}
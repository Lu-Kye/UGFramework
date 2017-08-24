using System;

namespace UGFramework.UGEditor.Inspector
{
    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = true, 
        AllowMultiple = false)]
    public class ShowInInspector : Attribute
    {
        bool _isReadonly = false;
        public bool IsReadonly 
        { 
            get
            {
                return _isReadonly;
            }
            set
            {
                _isReadonly = value;
            }
        }

        public ShowInInspector() {}
    }

    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = true, 
        AllowMultiple = false)]
    public class InspectorTooltip : Attribute
    {
        public string Tooltip { get; private set; }
        public InspectorTooltip(string tooltip)
        {
            this.Tooltip = tooltip;
        }
    }

    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = true, 
        AllowMultiple = false)]
    public class OverrideDrawer : Attribute
    {
        public string Method { get; private set; }
        public OverrideDrawer(string method)
        {
            this.Method = method;
        }
    }

    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = true, 
        AllowMultiple = false)]
    public class Foldouter : Attribute
    {
        public bool Foldout { get; private set; }
        public Foldouter(bool foldout = true)
        {
            this.Foldout = foldout;
        }
    }
}
using System;

namespace UGFramework.Editor.Inspector
{
    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = false, 
        AllowMultiple = false)]
    public class ShowInInspector : Attribute
    {
    }

    [AttributeUsageAttribute(
        AttributeTargets.Property | AttributeTargets.Field, 
        Inherited = false, 
        AllowMultiple = false)]
    public class InspectorTooltip : Attribute
    {
        public string Tooltip { get; private set; }
        public InspectorTooltip(string tooltip)
        {
            this.Tooltip = tooltip;
        }
    }
}
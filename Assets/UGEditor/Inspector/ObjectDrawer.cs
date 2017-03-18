using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Editor.Inspector
{
    /**
     * --- DOC BEGIN ---
     * As default, only draw public fields
     * --- DOC END ---
     */
    public struct ObjectDrawer
    {
        public static bool Draw(object obj, GUIContent content)
        {
            var drawer = new ObjectDrawer();
            drawer.Object = obj;
            drawer.Setup();
            return drawer.Draw();
        }

        public object Object { get; set; }

        List<MemberInfo> _memberInfos; 

        void Setup()
        {
            var type = this.Object.GetType();

            // Get all fields accessable to inspector 
            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            _memberInfos = new List<MemberInfo>();
            for (int i = 0; i < fields.Length; ++i)
            {
                var field = fields[i];
                if (field.IsPublic) // or is private but accessable
                {
                    _memberInfos.Add(new MemberInfo(this.Object, field));
                }
            }
        }

        bool Draw()
        {
            var iter = _memberInfos.GetEnumerator();
            while (iter.MoveNext())
            {
                InspectorUtility.DrawMember(iter.Current); 
            }
            return false;
        }
    }
}
using UnityEngine;

namespace UGFramework.Editor.Inspector
{
    public static class MemberDrawer
    {
        public static bool CheckMember(MemberInfo info)
        {
            try
            {
                DrawMember(info, null, true);
                return true;
            }
            catch (System.Exception e)
            {
                // LogManager.Instance.Error(e.Message);
                return false;
            }
        }

        public static bool DrawMember(MemberInfo info, GUIContent content)
        {
            return DrawMember(info, content, false);
        }

        static bool DrawMember(MemberInfo info, GUIContent content = null, bool check = false)
        {
            content = content == null ? new GUIContent(info.Name, info.Tooltip) : content;

            var changed = false;

            // Int
            if (info.Type == typeof(int))
            {
                if (check)
                    return false;

                var value = info.GetValue<int>();
                changed = InspectorUtility.DrawInt(ref value, content);
                if (changed && info.IsReadonly == false)
                    info.SetValue(value);
            }
            // Uint
            else if (info.Type == typeof(uint))
            {
                if (check)
                    return false;

                var value = (int)info.GetValue<uint>();
                changed = InspectorUtility.DrawInt(ref value, content);
                if (changed && info.IsReadonly == false)
                    info.SetValue((uint)Mathf.Max(0, value));
            }
            // Class
            else if (info.Type.IsClass)
            {
                if (check)
                    return false;

                var value = info.GetValue<object>();
                changed = InspectorUtility.DrawObject(value, content); 
            }
            else
            {
                throw new System.Exception(string.Format("Unsupport type {0}", info.Type.Name));
            }

            return changed;
        }
    }
}
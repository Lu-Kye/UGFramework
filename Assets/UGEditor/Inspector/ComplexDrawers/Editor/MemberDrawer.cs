#pragma warning disable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Editor.Inspector
{
    public static class MemberDrawer
    {
        static bool _isReadonly = false;
        public static bool IsReadonly
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

        static bool IsWritable(MemberInfo info)
        {
            if (info.IsReadonly)
                return false;
            return MemberDrawer.IsReadonly == false;
        }

        public static bool Check(MemberInfo info)
        {
            try
            {
                Draw(info, null, true);
                return true;
            }
            catch (System.Exception e)
            {
                // LogManager.Instance.Error(e.Message);
                return false;
            }
        }

        public static bool Draw(MemberInfo info, GUIContent content)
        {
            return Draw(info, content, false);
        }

        static bool Draw(MemberInfo info, GUIContent content = null, bool check = false)
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
                if (changed && IsWritable(info))
                    info.SetValue(value);
            }
            // Uint
            else if (info.Type == typeof(uint))
            {
                if (check)
                    return false;

                var value = (int)info.GetValue<uint>();
                changed = InspectorUtility.DrawInt(ref value, content);
                if (changed && IsWritable(info))
                    info.SetValue((uint)Mathf.Max(0, value));
            }
            // Float
            else if (info.Type == typeof(float))
            {
                if (check)
                    return false;
                
                var value = info.GetValue<float>();
                changed = InspectorUtility.DrawFloat(ref value, content);
                if (changed && IsWritable(info))
                    info.SetValue(value);
            }
            // Enum
            else if (info.Type.IsEnum)
            {
                if (check)
                    return false;
                
                var value = info.GetValue<Enum>();
                changed = InspectorUtility.DrawEnum(ref value, content);
                if (changed && IsWritable(info))
                    info.SetValue(value);
            }
            // String
            else if (info.Type == typeof(string))
            {
                if (check)
                    return false;

                var value = info.GetValue<string>();
                changed = InspectorUtility.DrawString(ref value, content);
                if (changed && IsWritable(info))
                    info.SetValue(value);                        
            }
            // Struct
            else if (info.Type.IsValueType && info.Type.IsPrimitive == false)
            {
                if (check)
                    return false;

                // If struct is readonly, members of struct are readonly
                var prevIsReadonly = IsReadonly;
                if (info.IsReadonly)
                    IsReadonly = true;

                var value = info.GetValue<object>();
                changed = InspectorUtility.DrawObject(value, content); 
                if (changed && IsWritable(info))
                    info.SetValue(value);

                IsReadonly = prevIsReadonly;
            }
            // Class
            else if (info.Type.IsClass && info.IsCollection == false)
            {
                if (check)
                    return false;

                // If class is readonly, members of class are readonly
                var prevIsReadonly = IsReadonly;
                if (info.IsReadonly)
                    IsReadonly = true;

                var value = info.GetValue<object>();
                changed = InspectorUtility.DrawObject(value, content); 

                IsReadonly = prevIsReadonly;
            }
#region Collections
            else if (info.Type == typeof(HashSet<>))
            {
                if (check)
                    return false;
                
                var value = info.GetValue<HashSet<object>>();
                changed = InspectorUtility.DrawHashSet(value, content);
            }
#endregion
            else
            {
                throw new Exception();
            }

            return changed;
        }
    }
}
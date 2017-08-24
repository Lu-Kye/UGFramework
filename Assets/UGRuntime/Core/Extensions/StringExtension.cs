using System;
using UGFramework.Log;

namespace UGFramework.Extension
{
    public static class StringExtension
    {
        public static string ReplaceFirst(this string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string ReplaceLast(this string text, string search, string replace)
        {
            int pos = text.LastIndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static T ToEnum<T>(this string text, string errorMsg = null) 
        {
#if UNITY_EDITOR
            try
            {
#endif
                return (T)Enum.Parse(typeof(T), text);
#if UNITY_EDITOR
            }
            catch (Exception e)
            {
                if (string.IsNullOrEmpty(errorMsg) == false) LogManager.Error(errorMsg);
                LogManager.Error(string.Format(
                    "ToEnum error, enum({0}) enumType({1}) error({2})",
                    text,
                    typeof(T).Name,
                    e.Message
                ));
                return default(T);
            }
#endif
        }
    }
}

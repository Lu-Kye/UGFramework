using System;
using System.Collections.Generic;
using UGFramework.Log;

namespace UGFramework.Extension
{
    public static class DictionaryExtension
    {
        public static Dictionary<string, float> Clone(this Dictionary<string, float> dict) 
        {
            var result = new Dictionary<string, float>();
            var iter = dict.GetEnumerator();
            while (iter.MoveNext())
            {
                result[iter.Current.Key] = iter.Current.Value;
            }
            return result;
        }

        public static void Append<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> target, bool overrideExists = true)
        {
            var iter = target.GetEnumerator();
            while (iter.MoveNext())
            {
                var key = iter.Current.Key;
                var value = iter.Current.Value;
                if (overrideExists == false && source.ContainsKey(key))
                    continue;
                source[key] = value;
            }
        }
    }
}
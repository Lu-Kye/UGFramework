using System;

namespace UGFramework.Extension.Array
{
    public static class ArrayExtension
    {
        public static T[] Merge<T>(this T[] array1, T[] array2)
        {
            var result = new T[array1.Length + array2.Length];
            array1.CopyTo(result, 0);
            array2.CopyTo(result, array1.Length);
            return result; 
        }
    }
}

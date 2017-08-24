using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UGFramework.Extension
{
    public static class ArrayExtension
    {
        public static string ToArrayString<T>(this T[][] array)
        {
            if (array == null)
                return "nil";
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < array.Length; ++i)
            {
                sb.Append(array[i].ToArrayString());
                if (i != array.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static string ToArrayString<T>(this T[] array)
        {
            if (array == null)
                return "nil";
            var sb = new StringBuilder();
            sb.Append("[");
            for (int i = 0; i < array.Length; ++i)
            {
                sb.Append(array[i].ToString());
                if (i != array.Length - 1)
                    sb.Append(",");
            }
            sb.Append("]");
            return sb.ToString();
        }

        public static T[] Merge<T>(this T[] array1, T[] array2, int array2Length = -1)
        {
            array2Length = array2Length == -1 ? array2.Length : array2Length;
            var result = new T[array1.Length + array2Length];
            array1.CopyTo(result, 0);
            Array.Copy(array2, 0, result, array1.Length, array2Length);
            return result; 
        }
    
        public static T[] Add<T>(this T[] array, T element, int index = -1)
        {
            var source = array;
            var result = new T[array.Length + 1];
            index = index == -1 ? result.Length - 1 : index;
    
            for (int i = 0; i < result.Length; ++i)
            {
                int resultIndex;
                int sourceIndex;
                if (i < index)
                {
                    resultIndex = i;
                    sourceIndex = i;
                    result[resultIndex] = source[sourceIndex];
                }
                else if (i > index)
                {
                    resultIndex = i;
                    sourceIndex = i - 1;
                    result[resultIndex] = source[sourceIndex];
                }
                else
                {
                    resultIndex = index;
                    sourceIndex = index;
                    result[resultIndex] = element;
                }
            }
    
            return result;
        }
    
        public static T[] Remove<T>(this T[] array, T element)
        {
            var index = array.IndexOf(element);
            if (index == -1)
                return array;
            
            var result = new T[array.Length - 1];
            for (int i = 0; i < array.Length; ++i)
            {
                if (i < index)
                    result[i] = array[i];
                else if (i > index)
                    result[i - 1] = array[i]; 
            }
            return result;
        }
    
        public static int IndexOf<T>(this T[] array, T element, int length = -1)
        {
            var index = -1;
            length = length == -1 ? array.Length : length;
            for (int i = 0; i < length; ++i)
            {
                if (object.Equals(array[i], element))
                    index = i;
            }
            return index;
        }
    }
}

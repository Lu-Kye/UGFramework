using UnityEngine;
using UGFramework.Extension;
using System.Reflection;

namespace UGFramework.Utility
{
    public static class GameObjectUtility
    {
        public static T GetComponent<T>(GameObject gameObject, bool add = true)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component == null && add)
            {
                component = gameObject.AddComponent<T>();
            }
            return component;
        }

        public delegate bool Filter<T>(T component);
        public static T[] GetComponentsInChildren<T>(GameObject gameObject, Filter<T> filter)
        {
            var components = gameObject.GetComponentsInChildren<T>();
            var filteredComponents = new T[0];
            for (int i = 0; i < components.Length; ++i)
            {
                var component = components[i];
                if (filter(component) == false)
                    continue;
                filteredComponents = filteredComponents.Add(component);
            }
            return filteredComponents;
        }

        public static T CopyComponent<T>(GameObject sourceGameObject, GameObject targetGameObject) 
            where T : Component
        {
            var source = sourceGameObject.GetComponent<T>();
            var target = GetComponent<T>(targetGameObject);

            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Default | BindingFlags.DeclaredOnly;
            var properties = typeof(T).GetProperties(flags);
            foreach (var property in properties) 
            {
                if (property.CanWrite) 
                {
                    try 
                    {
                        property.SetValue(target, property.GetValue(source, null), null);
                    }
                    catch { } // In case of NotImplementedException being thrown. For some reason specifying that exception didn't seem to catch it, so I didn't catch anything specific.
                }
            }
            var fields = typeof(T).GetFields(flags);
            foreach (var field in fields) 
            {
                field.SetValue(target, field.GetValue(source));
            }
            return target;
        }

        public static void RemoveComponent<T>(GameObject gameObject, bool immediate = false)
            where T : Component
        {
            var component = gameObject.GetComponent<T>();
            if (component != null) 
                if (immediate) GameObject.DestroyImmediate(component); else GameObject.Destroy(component); 
        }
        public static void RemoveComponentsInChildren<T>(GameObject gameObject)
            where T : Component
        {
            var components = gameObject.GetComponentsInChildren<T>();
            for (int i = 0; i < components.Length; ++i) GameObject.Destroy(components[i]);
        }

        public static TNew ReplaceComponent<TOld, TNew>(GameObject gameObject)
            where TOld : Component
            where TNew : Component
        {
            var componentOld = gameObject.GetComponent<TOld>();
            if (componentOld != null)
                GameObject.DestroyImmediate(componentOld);
            var componentNew = gameObject.GetComponent<TNew>();
            if (componentNew == null)
                componentNew = gameObject.AddComponent<TNew>();    
            return componentNew;
        }

        public static void SetChildrenActive(GameObject gameObject, bool active)
        {
            var count = gameObject.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                gameObject.transform.GetChild(i).gameObject.SetActive(active);
            }
        }

        public static GameObject[] GetChildren(GameObject gameObject)
        {
            var count = gameObject.transform.childCount;
            var childs = new GameObject[count];
            for (int i = 0; i < count; ++i)
            {
                childs[i] = gameObject.transform.GetChild(i).gameObject;
            }
            return childs;
        }

        public static void DestroyChildren(GameObject gameObject)
        {
            var count = gameObject.transform.childCount;
            for (int i = count - 1; i >= 0; --i)
            {
                var child = gameObject.transform.GetChild(i);
                GameObject.Destroy(child.gameObject);
            }
        }

        public static void ChangeLayer(GameObject gameObject, string layerName, bool recursive = false)
        {
            int layer = LayerMask.NameToLayer(layerName);
            ChangeLayer(gameObject, layer, recursive);
        }
        public static void ChangeLayer(GameObject gameObject, int layer, bool recursive = false)
        {
            gameObject.layer = layer;
            if (recursive)
            {
                var childs = GetChildren(gameObject);
                for (int i = 0; i < childs.Length; ++i)
                {
                    ChangeLayer(childs[i], layer, recursive);
                }
            }
        }
    }
}
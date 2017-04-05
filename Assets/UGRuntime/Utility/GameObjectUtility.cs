using UnityEngine;

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
    }
}
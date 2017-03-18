using UnityEngine;

namespace UGFramework.Editor
{
    [ExecuteInEditMode]
    public class InspectorExample : MonoBehaviour
    {
        public int TestInt = 1;     

        void Update()
        {
            TestInt++;
        }
    }
}
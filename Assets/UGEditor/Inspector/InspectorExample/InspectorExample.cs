#pragma warning disable

using UnityEngine;

namespace UGFramework.Editor.Inspector
{
    [ExecuteInEditMode]
    public class InspectorExample : MonoBehaviour
    {
        public int TestInt = 1;     
        public uint TestUInt = 1;     
        public float TestFloat = 1.5f;

        [InspectorTooltip("Test")]
        [ShowInInspector]
        int TestPropertyPrivate { get; set; }

        [ShowInInspector]
        int TestProperty { get; set; }

        public class TestObjectClass
        {
            public int TestObjectClassMember = 1;
            public TestObjectClassA TestObjectClassA = new TestObjectClassA();
        }
        public class TestObjectClassA
        {
            public int TestObjectClassAMember = 1;
        }
        public TestObjectClass TestObject = new TestObjectClass();

        public double TestUnsupport = 0.1;

        public enum TestEnumDef
        {
            Enum1,
            Enum2,
        }
        public TestEnumDef TestEnum;
    }
}
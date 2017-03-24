#pragma warning disable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace UGFramework.Editor.Inspector
{
    [ExecuteInEditMode]
    public class InspectorExample : MonoBehaviour
    {
        [ShowInInspector(IsReadonly = true)]
        public int TestInt = 1;     
        public uint TestUInt = 1;     
        public float TestFloat = 1.5f;

        [InspectorTooltip("Test")]
        [ShowInInspector]
        public int TestProperty { get; set; }

        [Serializable]
        public class TestObjectDef1
        {
            public int TestObject1Member = 1;
            public TestObjectDef2 TestObjectClassA = new TestObjectDef2();
        }
        [Serializable]
        public class TestObjectDef2
        {
            public int TestObject2Member = 1;
        }
        public TestObjectDef1 TestObject = new TestObjectDef1();

        public double TestUnsupport = 0.1;

        public enum TestEnumDef
        {
            Enum1,
            Enum2,
        }
        public TestEnumDef TestEnum;

        [Serializable]
        public struct TestStructDef
        {
            public int TestStructMember;
        }
        public TestStructDef TestStruct;

        public string TestString = "Hello";

        public HashSet<int> TestHashSet = new HashSet<int>() { 1, 2 };
    }
}
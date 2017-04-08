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

        [ShowInInspector]
        [SerializeField]
        int TestPrivate = 1;

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

        public int[] TestArray = new int[] { 1, 2, };
        public TestListClassDef[] TestArrayClass = new TestListClassDef[] 
        {
            new TestListClassDef(1),
            new TestListClassDef(2),
        };

        public List<int> TestList = new List<int>();

        public LinkedList<int> TestLinkedList = new LinkedList<int>(); 

        [Serializable]
        public class TestListClassDef
        {
            public int TestListClassDefMember = 1;
            public TestListClassDef(int value)
            {
                this.TestListClassDefMember = value;
            }
        }
        public List<TestStructDef> TestListStruct = new List<TestStructDef>();
        public List<TestListClassDef> TestListClass = new List<TestListClassDef>() 
        {
            new TestListClassDef(1),
            new TestListClassDef(2),
        };
        [ShowInInspector(IsReadonly = true)]
        public List<TestListClassDef> TestListClassReadonly = new List<TestListClassDef>() 
        {
            new TestListClassDef(1),
            new TestListClassDef(2),
        };

        public Dictionary<string, int> TestDict = new Dictionary<string, int>();
        public Dictionary<int, int> TestDictInt = new Dictionary<int, int>();
        public Dictionary<TestListClassDef, TestListClassDef> TestObjectDict = new Dictionary<TestListClassDef, TestListClassDef>();

        void Awake()
        {
            this.TestLinkedList.AddLast(1);
            this.TestLinkedList.AddLast(2);

            this.TestDict["1"] = 1;
            this.TestDict["2"] = 1;

            this.TestDictInt[1] = 1;
            this.TestDictInt[2] = 1;

            var key = new TestListClassDef(1);
            var value = new TestListClassDef(2);
            this.TestObjectDict[key] = value;
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace XmSerializer.Tests
{
    [TestClass]
    public class AttributeTests
    {
        [TestMethod]
        public void TestAttributes()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(AttributeTestClass));
            var obj = new AttributeTestClass {TestField = "foobar"};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(obj.TestField, result.TestField);
        }    

        [TestMethod]
        public void TestImplicitNameAttributes()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ImplicitAttributeTestClass));
            var obj = new ImplicitAttributeTestClass {TestField = "foobar" };
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(obj.TestField, result.TestField);
        }

        [TestMethod]
        public void TestImplicitFieldAttribute()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ImplicitFieldsTestClass));
            var obj = new ImplicitFieldsTestClass { TestField = "foobar" };
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(obj.TestField, result.TestField);
        }

        [TestMethod]
        public void TestNotSerialized()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(NotSerializedTestClass));
            var obj = new NotSerializedTestClass { TestField = "foobar", NonSerializedTestField = "foobar"};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(obj.TestField, result.TestField);
            Assert.IsNull(result.NonSerializedTestField);
        }

        [TestMethod]
        public void TestInitializeAfterDeserialization()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(InitializeAfterDeserializationTestClass));
            s.AddType(typeof(InitializeAfterDeserializationTestClass1));
            s.AddType(typeof(InitializeAfterDeserializationTestClass2));
            s.AddType(typeof(InitializeAfterDeserializationTestClass3));
            var obj = new InitializeAfterDeserializationTestClass();
            var result = BasicTest.TestSerialization(obj, s);

            Assert.IsNotNull(result.foo);
            Assert.IsNotNull(result.bar);

            Assert.ThrowsException<Exception>(() => BasicTest.TestSerialization(new InitializeAfterDeserializationTestClass1(), s));
            Assert.ThrowsException<Exception>(() => BasicTest.TestSerialization(new InitializeAfterDeserializationTestClass2(), s));
            Assert.ThrowsException<Exception>(() => BasicTest.TestSerialization(new InitializeAfterDeserializationTestClass3(), s));
        }

        [SerializableType("att")]
        public class AttributeTestClass
        {
            [Serialized("tta")] public string TestField;
        }

        [SerializableType]
        public class ImplicitAttributeTestClass
        {
            [Serialized] public string TestField;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class NotSerializedTestClass
        {
            [NotSerialized] public string NonSerializedTestField;
            public string TestField;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class ImplicitFieldsTestClass
        {
            public string TestField;
        }

        [SerializableType]
        public class InitializeAfterDeserializationTestClass
        {
            [InitializeAfterDeserialization] public readonly List<string> foo = new List<string>();
            [InitializeAfterDeserialization(typeof(List<string>))] public readonly IList<string> bar = new List<string>();
        }

        [SerializableType]
        public class InitializeAfterDeserializationTestClass1
        {
            [InitializeAfterDeserialization] private readonly IList<string> bar1 = new List<string>();
        }

        [SerializableType]
        public class InitializeAfterDeserializationTestClass2
        {
            [InitializeAfterDeserialization(typeof(IList<string>))] private readonly List<string> bar2 = new List<string>();
        }

        [SerializableType]
        public class InitializeAfterDeserializationTestClass3
        {
            [InitializeAfterDeserialization(typeof(List<int>))] private readonly List<string> bar3 = new List<string>();
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
    }
}
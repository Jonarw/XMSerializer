using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class ConstructorTests
    {
        [TestMethod]
        public void TestSkipInstantiating()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(SkipInstantiatingTestClass));

            var obj = new SkipInstantiatingTestClass();
            obj.TestList.Add("test");
            var xml = s.Serialize(obj);
            var result = s.Deserialize<SkipInstantiatingTestClass>(xml);
            Assert.AreEqual("foobar", result.TestList[0]);
            Assert.AreEqual("test", result.TestList[1]);
            Assert.AreEqual(2, result.TestList.Count);
        }

        [TestMethod]
        public void TestConstructor()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ConstructorTestClass));

            var obj = new ConstructorTestClass();
            var xml = s.Serialize(obj);
            var result = s.Deserialize<ConstructorTestClass>(xml);
            Assert.AreEqual(obj.TestField, result.TestField);
            Assert.AreEqual(obj.TestField2, result.TestField2);
        }

        [TestMethod]
        public void TestSkipConstructor()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ConstructorTestClass2));

            var obj = new ConstructorTestClass2();
            var xml = s.Serialize(obj);
            var result = s.Deserialize<ConstructorTestClass2>(xml);
            Assert.IsNull(result.TestField);
            Assert.IsNull(result.TestField2);
        }

        [SerializableType("ds", SkipConstructor = false)]
        public class ConstructorTestClass
        {
            public string TestField;

            public string TestField2 = "bar";

            public ConstructorTestClass()
            {
                this.TestField = "foo";
            }
        }

        [SerializableType]
        public class SkipInstantiatingTestClass
        {
            [Serialized(SkipInstantiating = true)] public IList<string> TestList = new List<string>();

            [OnDeserializing]
            public void OnDeserializing()
            {
                this.TestList = new List<string> {"foobar"};
            }
        }

        [SerializableType("sd", SkipConstructor = true)]
        public class ConstructorTestClass2
        {
            public string TestField;

            public string TestField2 = "bar";

            public ConstructorTestClass2()
            {
                this.TestField = "foo";
            }
        }
    }
}
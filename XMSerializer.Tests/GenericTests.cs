using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        public void TestGeneric()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(GenericTestClass<,,>));

            var obj = new GenericTestClass<string, int, bool>
            {
                Field1 = "test",
                Field2 = 5,
                Field3 = true
            };

            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(result.Field1, obj.Field1);
            Assert.AreEqual(result.Field2, obj.Field2);
            Assert.AreEqual(result.Field3, obj.Field3);
        }

        [TestMethod]
        public void TestNestedGeneric()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(GenericTestClass<,,>));

            var obj = new GenericTestClass<string, int, bool>();
            obj.Field1 = "test";
            obj.Field2 = 5;
            obj.Field3 = true;

            var obj2 = new GenericTestClass<string, GenericTestClass<string, int, bool>, int>
            {
                Field1 = "ff",
                Field2 = obj,
                Field3 = 12
            };

            var result = BasicTest.TestSerialization(obj2, s);
            Assert.AreEqual(result.Field1, obj2.Field1);
            Assert.AreEqual(result.Field2.Field1, obj.Field1);
            Assert.AreEqual(result.Field2.Field2, obj.Field2);
            Assert.AreEqual(result.Field2.Field3, obj.Field3);
            Assert.AreEqual(result.Field3, obj2.Field3);
        }

        [TestMethod]
        public void TestReducedNumberOfTypeParameters()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(GenericTestClass<,,>));

            var s2 = new XmSerializerModel();
            s2.AddType(typeof(GenericTestClass2<,>));

            var s3 = new XmSerializerModel();
            s3.AddType(typeof(GenericTestClass3));

            var obj = new GenericTestClass<string, int, bool>
            {
                Field1 = "foobar",
                Field2 = 42,
                Field3 = true
            };

            var xml = s.Serialize(obj);
            var result = (GenericTestClass2<string, int>)s2.Deserialize<object>(xml);
            Assert.AreEqual(result.Field1, obj.Field1);
            Assert.AreEqual(result.Field2, obj.Field2);
            Assert.AreEqual(result.Field3, obj.Field3);

            var result2 = s3.Deserialize<GenericTestClass3>(xml);
            Assert.AreEqual(result2.Field1, obj.Field1);
            Assert.AreEqual(result2.Field2, obj.Field2);
            Assert.AreEqual(result2.Field3, obj.Field3);
        }

        [TestMethod]
        public void TestReducedNumberOfTypeParametersParsing()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(Foo<,,>));

            var s2 = new XmSerializerModel();
            s2.AddType(typeof(Foo<,>));

            var s3 = new XmSerializerModel();
            s3.AddType(typeof(Foo));

            var obj = new Foo<string, int, bool>
            {
                Field1 = "foobar",
                Field2 = 42,
                Field3 = true
            };

            var xml = s.Serialize(obj);
            var result = (Foo<string, int>)s2.Deserialize<object>(xml);
            Assert.AreEqual(result.Field1, obj.Field1);
            Assert.AreEqual(result.Field2, obj.Field2);
            Assert.AreEqual(result.Field3, obj.Field3);

            var result2 = s3.Deserialize<Foo>(xml);
            Assert.AreEqual(result2.Field1, obj.Field1);
            Assert.AreEqual(result2.Field2, obj.Field2);
            Assert.AreEqual(result2.Field3, obj.Field3);
        }

        [SerializableType("foo")]
        public class GenericTestClass<T1, T2, T3>
        {
            [Serialized("b")]
            public T1 Field1;
            [Serialized("a")]
            public T2 Field2;
            [Serialized("r")]
            public T3 Field3;
        }

        [SerializableType("foo")]
        public class GenericTestClass2<T1, T2>
        {
            [Serialized("b")]
            public T1 Field1;
            [Serialized("a")]
            public T2 Field2;
            [Serialized("r")]
            public bool Field3;
        }

        [SerializableType("foo")]
        public class GenericTestClass3
        {
            [Serialized("b")]
            public string Field1;
            [Serialized("a")]
            public int Field2;
            [Serialized("r")]
            public bool Field3;
        }

        [SerializableType]
        public class Foo<T1, T2, T3>
        {
            [Serialized("b")]
            public T1 Field1;
            [Serialized("a")]
            public T2 Field2;
            [Serialized("r")]
            public T3 Field3;
        }

        [SerializableType]
        public class Foo<T1, T2>
        {
            [Serialized("b")]
            public T1 Field1;
            [Serialized("a")]
            public T2 Field2;
            [Serialized("r")]
            public bool Field3;
        }

        [SerializableType]
        public class Foo
        {
            [Serialized("b")]
            public string Field1;
            [Serialized("a")]
            public int Field2;
            [Serialized("r")]
            public bool Field3;
        }

    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class CompatibilityTests
    {
        [TestMethod]
        public void TestBackwardCompatibility()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(CompatibilityTest1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(CompatibilityTest2));

            var dto = new CompatibilityTest2();
            var xml = s2.Serialize(dto);
            var dto2 = s.Deserialize<CompatibilityTest1>(xml);
            Assert.IsNotNull(dto2);
        }

        [TestMethod]
        public void TestDefaultValue()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(CompatibilityTest3));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(CompatibilityTest2));
            var s3 = new XmSerializerModel();
            s3.AddType(typeof(CompatibilityTest1));

            var dto = new CompatibilityTest2();
            var xml = s2.Serialize(dto);
            var dto2 = s.Deserialize<CompatibilityTest3>(xml);
            Assert.IsNotNull(dto2);
            Assert.AreEqual("barfoo", dto2.Foobar);

            var dto3 = new CompatibilityTest1 {Foobar = "foobar"};
            var xml2 = s3.Serialize(dto3);
            var dto4 = s.Deserialize<CompatibilityTest3>(xml2);
            Assert.AreEqual("foobar", dto4.Foobar);
        }

        [TestMethod]
        public void TestForwardCompatibility()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(CompatibilityTest1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(CompatibilityTest2));

            var dto = new CompatibilityTest1 {Foobar = "foobar"};
            var xml = s.Serialize(dto);
            var dto2 = s2.Deserialize<CompatibilityTest2>(xml);
            Assert.IsNotNull(dto2);
        }

        [TestMethod]
        public void TestClassCompatibility()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(CompatibilityTest1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(CompatibilityTest3));

            var dto = new CompatibilityTest1 { Foobar = "foobar" };
            var xml = s.Serialize(dto);
            var dto2 = s2.Deserialize<CompatibilityTest3>(xml);
            Assert.IsNotNull(dto2);
            Assert.AreEqual(dto.Foobar, dto2.Foobar);
        }

        [TestMethod]
        public void TestLegacyName()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(OldTestClass));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(NewTestClass));

            var dto = new OldTestClass { Foobar = "foobar" };
            var xml = s.Serialize(dto);
            var dto2 = s2.Deserialize<NewTestClass>(xml);
            Assert.IsNotNull(dto2);
            Assert.AreEqual(dto.Foobar, dto2.Barfoo);

            var dto3 = BasicTest.TestSerialization(dto2, s2);
            Assert.IsNotNull(dto3);
            Assert.AreEqual(dto.Foobar, dto3.Barfoo);
        }

        [TestMethod]
        public void TestEnumCompatibility()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(FooBar1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(FooBar2));
            var s3 = new XmSerializerModel();
            s3.AddType(typeof(FooBar3));

            var dto = FooBar1.Bar;
            var xml = s.Serialize(dto);

            var result2 = s2.Deserialize<FooBar2>(xml);
            Assert.AreEqual(FooBar2.Bar, result2);

            var result3 = s3.Deserialize<FooBar3>(xml);
            Assert.AreEqual(FooBar3.Bar1, result3);
        }

        [TestMethod]
        public void TestValueConversion()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ValueConversionTestClass1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(ValueConversionTestClass2));

            var dto = new ValueConversionTestClass1()
            {
                V1 = 5,
                V2 = 6,
                V3 = 7,
                V4 = 8,
                V5 = 9,
                V6 = 10,
                V7 = 11,
            };
            var xml = s.Serialize(dto);

            var result = s2.Deserialize<ValueConversionTestClass2>(xml);
            Assert.AreEqual(result.V1, dto.V1);
            Assert.AreEqual(result.V2, dto.V2);
            Assert.AreEqual(result.V3, dto.V3);
            Assert.AreEqual(result.V4, dto.V4);
            Assert.AreEqual(result.V5, dto.V5);
            Assert.AreEqual(result.V6, dto.V6);
            Assert.AreEqual(result.V7, dto.V7);
        }

        [TestMethod]
        public void TestDeactivation()
        {
            var s1 = new XmSerializerModel();
            s1.AddType(typeof(DeactivationTestClass1));
            var s2 = new XmSerializerModel();
            s2.AddType(typeof(DeactivationTestClass2));

            var dto1 = new DeactivationTestClass1 { Foobar = "foo" };
            var dto2 = new DeactivationTestClass2 { Foobar = "bar" };

            var xml1 = s1.Serialize(dto1);
            var result1 = s2.Deserialize<DeactivationTestClass2>(xml1);
            var result2 = s1.Deserialize<DeactivationTestClass1>(xml1);
            Assert.AreEqual(result1.Foobar, dto1.Foobar);
            Assert.IsNull(result2.Foobar);

            var xml2 = s2.Serialize(dto2);
            var result3 = s2.Deserialize<DeactivationTestClass2>(xml2);
            var result4 = s1.Deserialize<DeactivationTestClass1>(xml2);
            Assert.IsNull(result3.Foobar);
            Assert.IsNull(result4.Foobar);
        }

        [SerializableType("CompatibilityTest")]
        public class CompatibilityTest1
        {
            [Serialized] public string Foobar;
        }

        [SerializableType("CompatibilityTest")]
        public class CompatibilityTest3
        {
            [Serialized(DefaultValue = "barfoo")] public string Foobar;
        }

        [SerializableType("CompatibilityTest")]
        public class CompatibilityTest2
        {
        }

        [SerializableType("FooBar")]
        public enum FooBar1
        {
            Foo,
            Bar
        }

        [SerializableType("FooBar")]
        public enum FooBar2
        {
            Bar,
            Foo
        }

        [SerializableType("FooBar")]
        public enum FooBar3
        {
            Foo1,
            Bar1
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class OldTestClass
        {
            public string Foobar;
        }

        [SerializableType(LegacyName = "OldTestClass")]
        public class NewTestClass
        {
            [Serialized(LegacyName = "Foobar")] public string Barfoo;
        }

        [SerializableType]
        public class ValueConversionTestClass1
        {
            [Serialized] public int V1;
            [Serialized] public double V2;
            [Serialized] public short V3;
            [Serialized] public float V4;
            [Serialized] public byte V5;
            [Serialized] public long V6;
            [Serialized] public double V7;
        }

        [SerializableType(nameof(ValueConversionTestClass1))]
        public class ValueConversionTestClass2
        {
            [Serialized] public short V1;
            [Serialized] public long V2;
            [Serialized] public int V3;
            [Serialized] public byte V4;
            [Serialized] public float V5;
            [Serialized] public short V6;
            [Serialized] public int V7;
        }

        [SerializableType("DeactivationTestClass")]
        public class DeactivationTestClass1
        {
            [Serialized(Deserialize = false)] public string Foobar;
        }

        [SerializableType("DeactivationTestClass")]
        public class DeactivationTestClass2
        {
            [Serialized(Serialize = false)] public string Foobar;
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class TestObfuscation
    {
        [TestMethod]
        public void TestObfuscate()
        {
            var dto = new ObfuscationTestClass
            {
                FooBar = "FooBar",
                BarFoo = "BarFoo"
            };

            var s = new XmSerializerModel();
            s.AddType(typeof(ObfuscationTestClass));

            var xml = s.Serialize(dto);
            Assert.IsTrue(xml.ToString().Contains("BarFoo"));
            Assert.IsFalse(xml.ToString().Contains("FooBar"));

            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual(result.FooBar, "FooBar");
            Assert.AreEqual(result.BarFoo, "BarFoo");
        }

        [SerializableType]
        public class ObfuscationTestClass
        {
            [Serialized] public string BarFoo;
            [Serialized(Obfuscate = true)] public string FooBar;
        }
    }
}
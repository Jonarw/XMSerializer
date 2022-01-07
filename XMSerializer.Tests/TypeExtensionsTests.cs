using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Reflection;
using XmSerializer.Extensions;

namespace XmSerializer.Tests
{
    [TestClass]
    public class TypeExtensionsTests
    {
        private const BindingFlags privateFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        [TestMethod]
        public void TestGetAllInstanceProperties()
        {
            var properties = typeof(Foo).GetAllInstanceProperties().ToList();
            Assert.AreEqual(4, properties.Count);
            Assert.IsTrue(properties.Contains(typeof(Bar).GetProperty("PublicBarProperty")));
            Assert.IsTrue(properties.Contains(typeof(Bar).GetProperty("PrivateBarProperty", privateFlags)));
            Assert.IsTrue(properties.Contains(typeof(Foo).GetProperty("PublicFooProperty")));
            Assert.IsTrue(properties.Contains(typeof(Foo).GetProperty("PrivateFooProperty", privateFlags)));
        }

        [TestMethod]
        public void TestGetAllInstanceFields()
        {
            var fields = typeof(Foo).GetAllInstanceFields().ToList();
            Assert.AreEqual(8, fields.Count); // includes 4 backing fields of auto properties
            Assert.IsTrue(fields.Contains(typeof(Bar).GetField("PublicBarField")));
            Assert.IsTrue(fields.Contains(typeof(Bar).GetField("PrivateBarField", privateFlags)));
            Assert.IsTrue(fields.Contains(typeof(Foo).GetField("PublicFooField")));
            Assert.IsTrue(fields.Contains(typeof(Foo).GetField("PrivateFooField", privateFlags)));
        }

        [TestMethod]
        public void TestGetAllInstanceMethods()
        {
            var methods = typeof(Foo).GetAllInstanceMethods().ToList();
            Assert.IsTrue(methods.Count >= 4);
            Assert.IsTrue(methods.Contains(typeof(Bar).GetMethod("PublicBarMethod")));
            Assert.IsTrue(methods.Contains(typeof(Bar).GetMethod("PrivateBarMethod", privateFlags)));
            Assert.IsTrue(methods.Contains(typeof(Foo).GetMethod("PublicFooMethod")));
            Assert.IsTrue(methods.Contains(typeof(Foo).GetMethod("PrivateFooMethod", privateFlags)));
        }

        [TestMethod]
        public void TestGetAncestors()
        {
            var ancestors = typeof(Foo).GetAncestors().ToList();
            Assert.AreEqual(2, ancestors.Count);
            Assert.IsTrue(ancestors.Contains(typeof(Bar)));
            Assert.IsTrue(ancestors.Contains(typeof(object)));
        }        

        [TestMethod]
        public void TestGetInstanceField()
        {
            Assert.AreEqual(typeof(Foo).GetField("PublicFooField"), typeof(Foo).GetInstanceField("PublicFooField"));
            Assert.AreEqual(typeof(Foo).GetField("PrivateFooField", privateFlags), typeof(Foo).GetInstanceField("PrivateFooField"));
            Assert.AreEqual(typeof(Bar).GetField("PublicBarField"), typeof(Foo).GetInstanceField("PublicBarField"));
            Assert.AreEqual(typeof(Bar).GetField("PrivateBarField", privateFlags), typeof(Foo).GetInstanceField("PrivateBarField"));
        }        

        [TestMethod]
        public void TestGetInstanceProperty()
        {
            Assert.AreEqual(typeof(Foo).GetProperty("PublicFooProperty"), typeof(Foo).GetInstanceProperty("PublicFooProperty"));
            Assert.AreEqual(typeof(Foo).GetProperty("PrivateFooProperty", privateFlags), typeof(Foo).GetInstanceProperty("PrivateFooProperty"));
            Assert.AreEqual(typeof(Bar).GetProperty("PublicBarProperty"), typeof(Foo).GetInstanceProperty("PublicBarProperty"));
            Assert.AreEqual(typeof(Bar).GetProperty("PrivateBarProperty", privateFlags), typeof(Foo).GetInstanceProperty("PrivateBarProperty"));
        }        

        [TestMethod]
        public void TestGetInstanceMethod()
        {
            Assert.AreEqual(typeof(Foo).GetMethod("PublicFooMethod"), typeof(Foo).GetInstanceMethod("PublicFooMethod"));
            Assert.AreEqual(typeof(Foo).GetMethod("PrivateFooMethod", privateFlags), typeof(Foo).GetInstanceMethod("PrivateFooMethod"));
            Assert.AreEqual(typeof(Bar).GetMethod("PublicBarMethod"), typeof(Foo).GetInstanceMethod("PublicBarMethod"));
            Assert.AreEqual(typeof(Bar).GetMethod("PrivateBarMethod", privateFlags), typeof(Foo).GetInstanceMethod("PrivateBarMethod"));
        }

        private class Bar
        {
            public string PublicBarField;
            private string PrivateBarField;
            public static string StaticBarProperty { get; set; }
            public string PublicBarProperty { get; set; }
            private string PrivateBarProperty { get; set; }

            public string PublicBarMethod() => null;

            private string PrivateBarMethod() => null;
        }

        private class Foo : Bar
        {
            public string PublicFooField;
            private string PrivateFooField;
            public static string StaticFooProperty { get; set; }
            public string PublicFooProperty { get; set; }
            private string PrivateFooProperty { get; set; }

            public string PublicFooMethod() => null;

            private string PrivateFooMethod() => null;
        }
    }
}
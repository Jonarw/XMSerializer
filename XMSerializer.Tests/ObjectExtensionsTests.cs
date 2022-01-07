using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XmSerializer.Extensions;

namespace XmSerializer.Tests
{
    [TestClass]
    public class ObjectExtensionsTests
    {
        [TestMethod]
        public void TestGetFieldValue()
        {
            var foo = new Foo();
            Assert.ThrowsException<ArgumentNullException>(() => ((object)null).GetFieldValue<string>("PublicFooField"));
            Assert.ThrowsException<ArgumentException>(() => foo.GetFieldValue<string>(null));
            Assert.ThrowsException<ArgumentException>(() => foo.GetFieldValue<string>("FooBar"));
            Assert.ThrowsException<Exception>(() => foo.GetFieldValue<int>("PublicFooField"));
            Assert.AreEqual(foo.PublicFooField, foo.GetFieldValue<string>("PublicFooField"));
            Assert.AreEqual("PrivateFooField", foo.GetFieldValue<string>("PrivateFooField"));
            Assert.AreEqual(foo.PublicBarField, foo.GetFieldValue<string>("PublicBarField"));
            Assert.AreEqual("PrivateBarField", foo.GetFieldValue<string>("PrivateBarField"));
        }

        [TestMethod]
        public void TestGetPropertyValue()
        {
            var foo = new Foo();
            Assert.ThrowsException<ArgumentNullException>(() => ((object)null).GetPropertyValue<string>("PublicFooProperty"));
            Assert.ThrowsException<ArgumentException>(() => foo.GetPropertyValue<string>(null));
            Assert.ThrowsException<ArgumentException>(() => foo.GetPropertyValue<string>("FooBar"));
            Assert.ThrowsException<Exception>(() => foo.GetPropertyValue<string>("PublicWriteOnlyFooProperty"));
            Assert.ThrowsException<Exception>(() => foo.GetPropertyValue<int>("PublicFooProperty"));
            Assert.AreEqual(foo.PublicFooProperty, foo.GetPropertyValue<string>("PublicFooProperty"));
            Assert.AreEqual("PrivateFooProperty", foo.GetPropertyValue<string>("PrivateFooProperty"));
            Assert.AreEqual(foo.PublicBarProperty, foo.GetPropertyValue<string>("PublicBarProperty"));
            Assert.AreEqual("PrivateBarProperty", foo.GetPropertyValue<string>("PrivateBarProperty"));
        }

        [TestMethod]
        public void TestTryGetFieldValue()
        {
            var foo = new Foo();
            Assert.AreEqual(false, foo.TryGetFieldValue(null, out string ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(false, ((object)null).TryGetFieldValue("PublicFooField", out ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(false, foo.TryGetFieldValue("PublicFooField", out int intret));
            Assert.AreEqual(0, intret);

            Assert.AreEqual(false, foo.TryGetFieldValue("Foobar", out ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(true, foo.TryGetFieldValue("PublicFooField", out ret));
            Assert.AreEqual(foo.PublicFooField, ret);

            Assert.AreEqual(true, foo.TryGetFieldValue("PrivateFooField", out ret));
            Assert.AreEqual("PrivateFooField", ret);

            Assert.AreEqual(true, foo.TryGetFieldValue("PublicBarField", out ret));
            Assert.AreEqual(foo.PublicBarField, ret);

            Assert.AreEqual(true, foo.TryGetFieldValue("PrivateBarField", out ret));
            Assert.AreEqual("PrivateBarField", ret);
        }

        [TestMethod]
        public void TestTryGetMemberValue()
        {
            var foo = new Foo();
            Assert.AreEqual(true, foo.TryGetMemberValue("PublicFooProperty", true, out string ret));
            Assert.AreEqual(foo.PublicFooProperty, ret);
            Assert.AreEqual(true, foo.TryGetMemberValue("PublicFooField", false, out ret));
            Assert.AreEqual(foo.PublicFooField, ret);
        }

        [TestMethod]
        public void TestTryGetPropertyValue()
        {
            var foo = new Foo();
            Assert.AreEqual(false, foo.TryGetPropertyValue(null, out string ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(false, ((object)null).TryGetPropertyValue("PublicFooProperty", out ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(false, foo.TryGetPropertyValue("PublicFooProperty", out int intret));
            Assert.AreEqual(0, intret);

            Assert.AreEqual(false, foo.TryGetPropertyValue("Foobar", out ret));
            Assert.AreEqual(null, ret);

            Assert.AreEqual(true, foo.TryGetPropertyValue("PublicFooProperty", out ret));
            Assert.AreEqual(foo.PublicFooProperty, ret);

            Assert.AreEqual(true, foo.TryGetPropertyValue("PrivateFooProperty", out ret));
            Assert.AreEqual("PrivateFooProperty", ret);

            Assert.AreEqual(true, foo.TryGetPropertyValue("PublicBarProperty", out ret));
            Assert.AreEqual(foo.PublicBarProperty, ret);

            Assert.AreEqual(true, foo.TryGetPropertyValue("PrivateBarProperty", out ret));
            Assert.AreEqual("PrivateBarProperty", ret);
        }

        [TestMethod]
        public void TestTrySetFieldValue()
        {
            var foo = new Foo();
            Assert.AreEqual(false, foo.TrySetFieldValue(null, " "));
            Assert.AreEqual("PublicFooField", foo.PublicFooField);

            Assert.AreEqual(false, ((object)null).TrySetFieldValue(null, " "));

            Assert.AreEqual(false, foo.TrySetFieldValue(null, 5));
            Assert.AreEqual(foo.PublicFooField, "PublicFooField");

            Assert.AreEqual(true, foo.TrySetFieldValue("PublicFooField", null));
            Assert.AreEqual(null, foo.PublicFooField);

            Assert.AreEqual(true, foo.TrySetFieldValue("PublicFooField", " "));
            Assert.AreEqual(" ", foo.PublicFooField);

            Assert.AreEqual(true, foo.TrySetFieldValue("PrivateFooField", " "));
            Assert.AreEqual(" ", foo.GetFieldValue<string>("PrivateFooField"));

            Assert.AreEqual(true, foo.TrySetFieldValue("PublicBarField", " "));
            Assert.AreEqual(" ", foo.PublicBarField);

            Assert.AreEqual(true, foo.TrySetFieldValue("PrivateBarField", " "));
            Assert.AreEqual(" ", foo.GetFieldValue<string>("PrivateBarField"));
        }

        [TestMethod]
        public void TestTrySetMemberValue()
        {
            var foo = new Foo();
            Assert.AreEqual(true, foo.TrySetMemberValue("PublicFooProperty", " ", true));
            Assert.AreEqual(" ", foo.PublicFooProperty);
            Assert.AreEqual(true, foo.TrySetMemberValue("PublicFooField", " ", false));
            Assert.AreEqual(" ", foo.PublicFooField);
        }

        [TestMethod]
        public void TestTrySetPropertyValue()
        {
            var foo = new Foo();
            Assert.AreEqual(false, foo.TrySetPropertyValue(null, " "));
            Assert.AreEqual("PublicFooProperty", foo.PublicFooProperty);

            Assert.AreEqual(false, ((object)null).TrySetPropertyValue(null, " "));

            Assert.AreEqual(false, foo.TrySetPropertyValue(null, 5));
            Assert.AreEqual(foo.PublicFooProperty, "PublicFooProperty");

            Assert.AreEqual(true, foo.TrySetPropertyValue("PublicFooProperty", null));
            Assert.AreEqual(null, foo.PublicFooProperty);

            Assert.AreEqual(true, foo.TrySetPropertyValue("PublicFooProperty", " "));
            Assert.AreEqual(" ", foo.PublicFooProperty);

            Assert.AreEqual(true, foo.TrySetPropertyValue("PrivateFooProperty", " "));
            Assert.AreEqual(" ", foo.GetPropertyValue<string>("PrivateFooProperty"));

            Assert.AreEqual(true, foo.TrySetPropertyValue("PublicBarProperty", " "));
            Assert.AreEqual(" ", foo.PublicBarProperty);

            Assert.AreEqual(true, foo.TrySetPropertyValue("PrivateBarProperty", " "));
            Assert.AreEqual(" ", foo.GetPropertyValue<string>("PrivateBarProperty"));
        }

        private class Bar
        {
            public string PublicBarField = "PublicBarField";
            private string PrivateBarField = "PrivateBarField";
            public string PublicBarProperty { get; set; } = "PublicBarProperty";
            private string PrivateBarProperty { get; set; } = "PrivateBarProperty";
        }

        private class Foo : Bar
        {
            public string PublicFooField = "PublicFooField";
            private string PrivateFooField = "PrivateFooField";

            public string PublicFooProperty { get; set; } = "PublicFooProperty";
            private string PrivateFooProperty { get; set; } = "PrivateFooProperty";
            private string PublicReadOnlyFooProperty { get; } = "PublicReadOnlyFooProperty";
            private string PublicWriteOnlyFooProperty { set { } }
        }
    }
}
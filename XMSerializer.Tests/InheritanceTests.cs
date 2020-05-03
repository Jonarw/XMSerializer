using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class InheritanceTests
    {
        [TestMethod]
        public void TestSimpleInheritance()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(DtoTestClass));
            s.AddType(typeof(BaseDto));
            var dto = new DtoTestClass();
            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.GetBooFar(), result.GetBooFar());
            Assert.AreEqual(dto.GetFooBar(), result.GetFooBar());

        }

        [TestMethod]
        public void TestGenericInheritance()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(GenericInheritanceBaseClass<>));
            s.AddType(typeof(GenericInheritanceDerivedClass<>));
            var dto = new GenericInheritanceDerivedClass<string>()
            {
                Barfoo = "Barfoo",
                Foobar = "Foobar"
            };

            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.Barfoo, result.Barfoo);
            Assert.AreEqual(dto.Foobar, result.Foobar);
        }

        [TestMethod]
        public void TestIncludeAncestors()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(TestClass1));
            s.AddType(typeof(TestClass2));
            s.AddType(typeof(AncestorTestClass));
            var dto1 = new TestClass1();
            var dto2 = new TestClass2();
            var result1 = BasicTest.TestSerialization(dto1, s);
            var result2 = BasicTest.TestSerialization(dto2, s);

            Assert.AreEqual(dto1.GetBooFar(), result1.GetBooFar());
            Assert.AreEqual(dto2.GetBooFar(), result2.GetBooFar());
            Assert.AreEqual(dto1.GetFooBar(), result1.GetFooBar());
            Assert.IsNull(result2.GetFooBar());
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public abstract class GenericInheritanceBaseClass<T>
        {
            public T Foobar;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class GenericInheritanceDerivedClass<T> : GenericInheritanceBaseClass<T>
        {
            public T Barfoo;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields)]
        public class DtoTestClass : BaseDto
        {
            private readonly string BooFar = "test";

            public string GetBooFar()
            {
                return this.BooFar;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields)]
        public abstract class BaseDto
        {
            private readonly string FooBar = "test2";

            public string GetFooBar()
            {
                return this.FooBar;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields)]
        public class TestClass1 : AncestorTestClass
        {
            private readonly string BooFar = "test";

            public string GetBooFar()
            {
                return this.BooFar;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields, IncludeAncestors = false)]
        public class TestClass2 : AncestorTestClass
        {
            private readonly string BooFar = "test";

            public string GetBooFar()
            {
                return this.BooFar;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields)]
        public abstract class AncestorTestClass
        {
            private readonly string FooBar = "test2";

            public string GetFooBar()
            {
                return this.FooBar;
            }
        }
    }
}
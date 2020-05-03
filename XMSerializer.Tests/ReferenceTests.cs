using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class ReferenceTests
    {
        [TestMethod]
        public void TestCircularReference()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ReferenceTestClass));

            var dto = new ReferenceTestClass();
            var dto2 = new ReferenceTestClass();
            var dto3 = new ReferenceTestClass();
            dto.RefernceField = dto2;
            dto2.RefernceField = dto3;
            dto3.RefernceField = dto;

            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreSame(result, ((ReferenceTestClass)((ReferenceTestClass)result.RefernceField).RefernceField).RefernceField);
        }

        [TestMethod]
        public void TestCustomReference()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ReferenceTestClass));

            var d = new Dictionary<int, string> {{1, "s"}};
            var dto = new ReferenceTestClass()
            {
                RefernceField = d,
                RefernceField2 = d
            };
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreSame(result.RefernceField, result.RefernceField2);
        }

        [TestMethod]
        public void TestReference()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ReferenceTestClass));
            s.AddType(typeof(ReferencedClass));

            var dto = new ReferenceTestClass {RefernceField = new ReferencedClass {FooBar = "foobar"}};
            dto.RefernceField2 = dto.RefernceField;
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual(((ReferencedClass)result.RefernceField).FooBar, ((ReferencedClass)result.RefernceField2).FooBar);
            Assert.AreSame(result.RefernceField, result.RefernceField2);
        }

        [TestMethod]
        public void TestSelfReference()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ReferenceTestClass));

            var dto = new ReferenceTestClass();
            dto.RefernceField = dto;
            dto.RefernceField2 = dto;
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreSame(result, result.RefernceField);
        }

        [TestMethod]
        public void TestArraySelfReference()
        {
            var s = new XmSerializerModel();
            var dto = new object[2];
            dto[0] = dto;
            dto[1] = dto;
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreSame(result, result[0]);
            Assert.AreSame(result, result[1]);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class ReferenceTestClass
        {
            public object RefernceField;
            public object RefernceField2;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class ReferencedClass
        {
            public string FooBar;
        }
    }
}
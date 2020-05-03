using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class TupleTests
    {
        [TestMethod]
        public void TestTuple()
        {
            var dto = new TupleTestClass
            {
                Foo = new Tuple<string, List<string>>("Foo", new List<string> {"Foo", "Bar"}),
                Bar = new Tuple<int, int, int, int, int, int, int, Tuple<int, int>>(1, 2, 3, 4, 5, 6, 7, new Tuple<int, int>(8, 9))
            };

            var s = new XmSerializerModel();
            s.AddType(typeof(TupleTestClass));
            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.Foo.Item1, result.Foo.Item1);
            Assert.AreEqual(dto.Foo.Item2[0], result.Foo.Item2[0]);
            Assert.AreEqual(dto.Foo.Item2[1], result.Foo.Item2[1]);
            Assert.AreEqual(dto.Bar.Item1, result.Bar.Item1);
            Assert.AreEqual(dto.Bar.Item2, result.Bar.Item2);
            Assert.AreEqual(dto.Bar.Item3, result.Bar.Item3);
            Assert.AreEqual(dto.Bar.Item4, result.Bar.Item4);
            Assert.AreEqual(dto.Bar.Item5, result.Bar.Item5);
            Assert.AreEqual(dto.Bar.Item6, result.Bar.Item6);
            Assert.AreEqual(dto.Bar.Item7, result.Bar.Item7);
            Assert.AreEqual(dto.Bar.Rest.Item1, result.Bar.Rest.Item1);
            Assert.AreEqual(dto.Bar.Rest.Item2, result.Bar.Rest.Item2);
        }

        [TestMethod]
        public void TestTupleReference()
        {
            var dto = new TupleReferenceTestClass
            {
                Foo = new Tuple<int>(5),
            };

            dto.Bar = dto.Foo;

            var s = new XmSerializerModel();
            s.AddType(typeof(TupleReferenceTestClass));
            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.Foo.Item1, result.Foo.Item1);
            Assert.AreSame(dto.Foo, dto.Bar);
        }

        [TestMethod]
        public void TestValueTuple()
        {
            var dto = new ValueTupleTestClass
            {
                Foo = ("Foo", new List<string> {"Foo", "Bar"}),
                Bar = (1, 2, 3, 4, 5, 6, 7, 8, 9, (10, 11))
            };

            var s = new XmSerializerModel();
            s.AddType(typeof(ValueTupleTestClass));
            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.Foo.Item1, result.Foo.Item1);
            Assert.AreEqual(dto.Foo.Item2[0], result.Foo.Item2[0]);
            Assert.AreEqual(dto.Foo.Item2[1], result.Foo.Item2[1]);
            Assert.AreEqual(dto.Bar.Item1, result.Bar.Item1);
            Assert.AreEqual(dto.Bar.Item2, result.Bar.Item2);
            Assert.AreEqual(dto.Bar.Item3, result.Bar.Item3);
            Assert.AreEqual(dto.Bar.Item4, result.Bar.Item4);
            Assert.AreEqual(dto.Bar.Item5, result.Bar.Item5);
            Assert.AreEqual(dto.Bar.Item6, result.Bar.Item6);
            Assert.AreEqual(dto.Bar.Item7, result.Bar.Item7);
            Assert.AreEqual(dto.Bar.Item8, result.Bar.Item8);
            Assert.AreEqual(dto.Bar.Item9, result.Bar.Item9);
            Assert.AreEqual(dto.Bar.Item10.Item1, result.Bar.Item10.Item1);
            Assert.AreEqual(dto.Bar.Item10.Item2, result.Bar.Item10.Item2);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class ValueTupleTestClass
        {
            public (int, int, int, int, int, int, int, int, int, (int, int)) Bar;
            public (string, List<string>) Foo;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class TupleTestClass
        {
            public Tuple<int, int, int, int, int, int, int, Tuple<int, int>> Bar;
            public Tuple<string, List<string>> Foo;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class TupleReferenceTestClass
        {
            public Tuple<int> Foo;
            public Tuple<int> Bar;
        }
    }
}
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class MemberFilterTests
    {
        [TestMethod]
        public void TestPrivateFields()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(MemberFilterDto1));
            var dto = new MemberFilterDto1();
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual("foobar", result.Gett2());
            Assert.AreEqual("foobar", result.t3);
            Assert.AreEqual("foobar", result.Gett4());
            Assert.IsNull(result.t1);
        }

        [TestMethod]
        public void TestPrivateProperties()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(MemberFilterDto3));
            var dto = new MemberFilterDto3();
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual("foobar", result.Gett4());
            Assert.IsNull(result.t1);
            Assert.IsNull(result.t3);
            Assert.IsNull(result.Gett2());
        }

        [TestMethod]
        public void TestPublicFields()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(MemberFilterDto2));
            var dto = new MemberFilterDto2();
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual("foobar", result.t1);
            Assert.IsNull(result.Gett2());
            Assert.IsNull(result.t3);
            Assert.IsNull(result.Gett4());
        }

        [TestMethod]
        public void TestPublicProperties()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(MemberFilterDto4));
            var dto = new MemberFilterDto4();
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual("foobar", result.t3);
            Assert.IsNull(result.t1);
            Assert.IsNull(result.Gett2());
            Assert.IsNull(result.Gett4());
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicFields, SkipConstructor = true)]
        public class MemberFilterDto1
        {
            public string t1 = "foobar";
            private readonly string t2 = "foobar";
            public string t3 { get; set; } = "foobar";
            private string t4 { get; set; } = "foobar";

            public string Gett2()
            {
                return this.t2;
            }

            public string Gett4()
            {
                return this.t4;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields, SkipConstructor = true)]
        public class MemberFilterDto2
        {
            public string t1 = "foobar";
            private readonly string t2 = "foobar";
            public string t3 { get; set; } = "foobar";
            private string t4 { get; set; } = "foobar";

            public string Gett2()
            {
                return this.t2;
            }

            public string Gett4()
            {
                return this.t4;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.NonPublicProperties, SkipConstructor = true)]
        public class MemberFilterDto3
        {
            public string t1 = "foobar";
            private readonly string t2 = "foobar";
            public string t3 { get; set; } = "foobar";
            private string t4 { get; set; } = "foobar";

            public string Gett2()
            {
                return this.t2;
            }

            public string Gett4()
            {
                return this.t4;
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicProperties, SkipConstructor = true)]
        public class MemberFilterDto4
        {
            public string t1 = "foobar";
            private readonly string t2 = "foobar";
            public string t3 { get; set; } = "foobar";
            private string t4 { get; set; } = "foobar";

            public string Gett2()
            {
                return this.t2;
            }

            public string Gett4()
            {
                return this.t4;
            }
        }
    }
}
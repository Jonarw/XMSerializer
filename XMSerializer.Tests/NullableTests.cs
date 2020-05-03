using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class NullableTests
    {
        [TestMethod]
        public void TestNullable()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(NullableDto));

            var dto = new NullableDto();
            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual(result.Foo, dto.Foo);
            Assert.AreEqual(result.Bar, dto.Bar);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class NullableDto
        {
            public int? Foo = 1;
            public int? Bar = null;
        }

    }
}
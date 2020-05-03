using Microsoft.VisualStudio.TestTools.UnitTesting;
using XmSerializer.Serializers;

namespace XmSerializer.Tests
{
    [TestClass]
    public class CompactArrayTest
    {
        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class CompactArrayDto
        {
            public int[] IntArray = {42, 43, 44, 45};
            public byte[] ByteArray = {42, 43, 44, 45};
            public double[] DoubleArray = {42, -58, 44, 45};
        }

        [TestMethod]
        public void TestCompactArray()
        {
            var dto = new CompactArrayDto();
            var s = new XmSerializerModel();
            s.AddType(typeof(CompactArrayDto));
            _ = new CompactArraySerializer(s);
            var result = BasicTest.TestSerialization(dto, s);

            Assert.AreEqual(dto.IntArray[0], result.IntArray[0]);
            Assert.AreEqual(dto.IntArray[1], result.IntArray[1]);
            Assert.AreEqual(dto.IntArray[2], result.IntArray[2]);
            Assert.AreEqual(dto.IntArray[3], result.IntArray[3]);

            Assert.AreEqual(dto.ByteArray[0], result.ByteArray[0]);
            Assert.AreEqual(dto.ByteArray[1], result.ByteArray[1]);
            Assert.AreEqual(dto.ByteArray[2], result.ByteArray[2]);
            Assert.AreEqual(dto.ByteArray[3], result.ByteArray[3]);

            Assert.AreEqual(dto.DoubleArray[0], result.DoubleArray[0]);
            Assert.AreEqual(dto.DoubleArray[1], result.DoubleArray[1]);
            Assert.AreEqual(dto.DoubleArray[2], result.DoubleArray[2]);
            Assert.AreEqual(dto.DoubleArray[3], result.DoubleArray[3]);
        }
    }
}
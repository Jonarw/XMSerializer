using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class BasicTest
    {
        [SerializableType]
        public enum FooBar
        {
            Foo,
            Bar
        }

        [TestMethod]
        public void TestBasicSerialization()
        {
            var serializer = new XmSerializerModel();
            var settings = new TypeSerializingSettings("DTO with whitespace", typeof(PrimitivesDto));
            settings.AddMember(new MemberSerializingSettings(nameof(PrimitivesDto.TestString), "testField", false));
            serializer.AddType(settings);

            var dto = new PrimitivesDto {TestString = "test"};
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.TestString, dto.TestString);
        }

        [TestMethod]
        public void TestEnum()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(EnumDto));
            serializer.AddType(typeof(FooBar));

            var dto = new EnumDto {EnumField = FooBar.Bar, EnumFieldSt = FooBar.Foo};
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.EnumField, dto.EnumField);
            Assert.AreEqual(result.EnumFieldSt, dto.EnumFieldSt);
        }

        [TestMethod]
        public void TestEnumList()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(FooBar));

            var dto = new List<Enum>();
            dto.Add(FooBar.Foo);
            dto.Add(FooBar.Bar);
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result[0], dto[0]);
            Assert.AreEqual(result[1], dto[1]);
        }

        [TestMethod]
        public void TestIdenticalStructs()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(StructDto));
            serializer.AddType(typeof(IdenticalStructsDto));

            var dto = new IdenticalStructsDto();
            dto.Struct1.TestString = "foobar";
            dto.Struct2.TestString = "foobar";
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.Struct1.TestString, dto.Struct2.TestString);
        }

        [TestMethod]
        public void TestNested()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(PrimitivesDto));
            var dto = new PrimitivesDto {SubObject = new PrimitivesDto {TestString = "dfsads"}};
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.SubObject.TestString, dto.SubObject.TestString);
        }

        [TestMethod]
        public void TestPrimitives()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(PrimitivesDto));

            var dto = new PrimitivesDto
            {
                TestString = "test",
                TestInt = -4567,
                TestBool = true,
                TestTimeSpan = new TimeSpan(1, 2, 3, 4),
                TestByte = 200,
                TestDateTime = new DateTime(2005, 2, 3, 1, 5, 3),
                TestDateTimeOffset = new DateTimeOffset(new DateTime(2005, 3, 2)),
                TestDouble = 1.23345,
                TestFloat = 1.1234f,
                TestLong = 403023,
                TestUint = 2131,
                TestUlong = 34535434,
                TestDecimal = new decimal(-3.5234234)
            };

            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.TestString, dto.TestString);
            Assert.AreEqual(result.TestInt, dto.TestInt);
            Assert.AreEqual(result.TestBool, dto.TestBool);
            Assert.AreEqual(result.TestTimeSpan, dto.TestTimeSpan);
            Assert.AreEqual(result.TestByte, dto.TestByte);
            Assert.AreEqual(result.TestDateTime, dto.TestDateTime);
            Assert.AreEqual(result.TestDateTimeOffset, dto.TestDateTimeOffset);
            Assert.AreEqual(result.TestFloat, dto.TestFloat);
            Assert.AreEqual(result.TestLong, dto.TestLong);
            Assert.AreEqual(result.TestUint, dto.TestUint);
            Assert.AreEqual(result.TestUlong, dto.TestUlong);
            Assert.AreEqual(result.TestDecimal, dto.TestDecimal);
        }

        public static T TestSerialization<T>(T dto, XmSerializerModel serializer)
        {
            var xml = serializer.Serialize(dto);
            var ret = serializer.Deserialize<T>(xml);
            var xml2 = serializer.Serialize(ret);
            Assert.AreEqual(xml.ToString(), xml2.ToString());
            Assert.AreEqual(dto.GetType(), ret.GetType());
            Directory.CreateDirectory(@"output\");
            File.WriteAllText($@"output\{typeof(T).Name}.csv", xml2.ToString());
            return ret;
        }

        [TestMethod]
        public void TestStruct()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(StructDto));

            var dto = new StructDto {TestString = "foobar"};
            var result = TestSerialization(dto, serializer);
            Assert.AreEqual(result.TestString, dto.TestString);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class EnumDto
        {
            public Enum EnumField;
            public FooBar EnumFieldSt;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public struct StructDto
        {
            public string TestString;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class IdenticalStructsDto
        {
            public StructDto Struct1;
            public StructDto Struct2;
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class PrimitivesDto
        {
            public PrimitivesDto SubObject;
            public bool TestBool;
            public byte TestByte;
            public DateTime TestDateTime;
            public DateTimeOffset TestDateTimeOffset;
            public decimal TestDecimal;
            public double TestDouble;
            public float TestFloat;
            public int TestInt;
            public long TestLong;
            public string TestString;
            public TimeSpan TestTimeSpan;
            public uint TestUint;
            public ulong TestUlong;
        }
    }
}
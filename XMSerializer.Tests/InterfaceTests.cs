using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class InterfaceTests
    {
        [TestMethod]
        public void TestInterface()
        {
            var s = new XmSerializerModel();
            var dto = new List<IList<IEnumerable>>
            {
                new List<IEnumerable> {new List<int> {3456}}
            };

            var result = BasicTest.TestSerialization(dto, s);
            Assert.AreEqual(3456, result.First().First().Cast<object>().First());
        }
    }
}
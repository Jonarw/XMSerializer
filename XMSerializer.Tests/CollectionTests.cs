using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class CollectionTests
    {
        [TestMethod]
        public void TestArray()
        {
            var s = new XmSerializerModel();
            var obj = new[] {1, 2, 3d};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(3, result.Length);
            Assert.AreEqual(obj[0], result[0]);
            Assert.AreEqual(obj[1], result[1]);
            Assert.AreEqual(obj[2], result[2]);
        }

        [TestMethod]
        public void TestCustomCollection()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(CustomCollectionTestClass));
            s.AddType(typeof(CustomCollection<>));
            var obj = new CustomCollectionTestClass();
            obj.CollectionField = new CustomCollection<int> {125, 132, 164};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(3, result.CollectionField.Count);
            Assert.AreEqual(obj.CollectionField[0], result.CollectionField[0]);
            Assert.AreEqual(obj.CollectionField[1], result.CollectionField[1]);
            Assert.AreEqual(obj.CollectionField[2], result.CollectionField[2]);
        }

        [TestMethod]
        public void TestDictionary()
        {
            var s = new XmSerializerModel();
            var obj = new Dictionary<int, string> {{5, "foo"}, {4, "bar"}};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(obj.Count, result.Count);
            Assert.AreEqual(obj[5], result[5]);
            Assert.AreEqual(obj[4], result[4]);
        }

        [TestMethod]
        public void TestJaggedArray()
        {
            var s = new XmSerializerModel();
            var obj = new double[2][][];
            obj[0] = new double[2][];
            obj[1] = new double[2][];
            obj[0][0] = new[] {1, 2d};
            obj[0][1] = new[] {3, 4d};
            obj[1][0] = new[] {5, 6d};
            obj[1][1] = new[] {7, 8d};

            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(2, result.Length);
            Assert.AreEqual(2, result[0].Length);
            Assert.AreEqual(2, result[1].Length);
            Assert.AreEqual(2, result[0][0].Length);
            Assert.AreEqual(2, result[1][0].Length);
            Assert.AreEqual(2, result[0][1].Length);
            Assert.AreEqual(2, result[1][1].Length);

            Assert.AreEqual(obj[0][0][0], result[0][0][0]);
            Assert.AreEqual(obj[0][0][1], result[0][0][1]);
            Assert.AreEqual(obj[0][1][0], result[0][1][0]);
            Assert.AreEqual(obj[0][1][1], result[0][1][1]);
            Assert.AreEqual(obj[1][0][0], result[1][0][0]);
            Assert.AreEqual(obj[1][0][1], result[1][0][1]);
            Assert.AreEqual(obj[1][1][0], result[1][1][0]);
            Assert.AreEqual(obj[1][1][1], result[1][1][1]);
        }

        [TestMethod]
        public void TestList()
        {
            var s = new XmSerializerModel();
            var obj = new List<int> {1, 2, 3};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(obj[0], result[0]);
            Assert.AreEqual(obj[1], result[1]);
            Assert.AreEqual(obj[2], result[2]);
        }

        [TestMethod]
        public void TestExplicitIList()
        {
            var s = new XmSerializerModel();
            s.AddType(typeof(ExplicitIListTestClass));
            var obj = (IList)new ExplicitIListTestClass();
            obj.Add("Foo");
            obj.Add("Bar");
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(obj[0], result[0]);
            Assert.AreEqual(obj[1], result[1]);
        }

        [TestMethod]
        public void TestListOfArray()
        {
            var s = new XmSerializerModel();
            var obj = new List<int[]>();
            obj.Add(new[] {1, 2});
            obj.Add(new[] {3, 4});

            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual(2, result[0].Length);
            Assert.AreEqual(2, result[1].Length);

            Assert.AreEqual(obj[0][0], result[0][0]);
            Assert.AreEqual(obj[0][1], result[0][1]);
            Assert.AreEqual(obj[1][0], result[1][0]);
            Assert.AreEqual(obj[1][1], result[1][1]);
        }

        [TestMethod]
        public void TestNestedCollection()
        {
            var s = new XmSerializerModel();
            var obj = new List<List<int>> {new List<int> {1, 2, 3}, new List<int> {4, 5, 6}, new List<int> {7, 8, 9}};
            var result = BasicTest.TestSerialization(obj, s);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(3, result[0].Count);
            Assert.AreEqual(3, result[1].Count);
            Assert.AreEqual(3, result[2].Count);
            Assert.AreEqual(obj[0][0], result[0][0]);
            Assert.AreEqual(obj[0][1], result[0][1]);
            Assert.AreEqual(obj[0][2], result[0][2]);
            Assert.AreEqual(obj[1][0], result[1][0]);
            Assert.AreEqual(obj[1][1], result[1][1]);
            Assert.AreEqual(obj[1][2], result[1][2]);
            Assert.AreEqual(obj[2][0], result[2][0]);
            Assert.AreEqual(obj[2][1], result[2][1]);
            Assert.AreEqual(obj[2][2], result[2][2]);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class CollectionPropertyTestClass
        {
            public IList<string> FooBar { get; private set; } = new List<string>();

            [OnDeserializing]
            private void OnDeserializing()
            {
                this.FooBar = new List<string>();
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class CustomCollectionTestClass
        {
            public CustomCollection<int> CollectionField;
        }

        [SerializableType(IsCollection = true, SkipConstructor = false)]
        public class ExplicitIListTestClass : IList
        {
            private IList _listImplementation = new List<object>();
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this._listImplementation.GetEnumerator();
            }

            void ICollection.CopyTo(Array array, int index)
            {
                this._listImplementation.CopyTo(array, index);
            }

            int ICollection.Count => this._listImplementation.Count;

            object ICollection.SyncRoot => this._listImplementation.SyncRoot;

            bool ICollection.IsSynchronized => this._listImplementation.IsSynchronized;

            int IList.Add(object value)
            {
                return this._listImplementation.Add(value);
            }

            bool IList.Contains(object value)
            {
                return this._listImplementation.Contains(value);
            }

            void IList.Clear()
            {
                this._listImplementation.Clear();
            }

            int IList.IndexOf(object value)
            {
                return this._listImplementation.IndexOf(value);
            }

            void IList.Insert(int index, object value)
            {
                this._listImplementation.Insert(index, value);
            }

            void IList.Remove(object value)
            {
                this._listImplementation.Remove(value);
            }

            void IList.RemoveAt(int index)
            {
                this._listImplementation.RemoveAt(index);
            }

            object IList.this[int index]
            {
                get => this._listImplementation[index];
                set => this._listImplementation[index] = value;
            }

            bool IList.IsReadOnly => this._listImplementation.IsReadOnly;

            bool IList.IsFixedSize => this._listImplementation.IsFixedSize;
        }

        [SerializableType(IsCollection = true, SkipConstructor = false)]
        public class CustomCollection<T> : List<T>
        {
        }
    }
}
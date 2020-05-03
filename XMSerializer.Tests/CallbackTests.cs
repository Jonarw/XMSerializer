using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XmSerializer.Tests
{
    [TestClass]
    public class CallbackTests
    {
        [TestMethod]
        public void TestCallbacks()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(CallbackTestClass));

            var instance = new CallbackTestClass();
            instance.DeSerializeTestValue3 = "test";
            var xml = serializer.Serialize(instance);
            var result = serializer.Deserialize<CallbackTestClass>(xml);
            Assert.AreEqual("OnSerialized", instance.SerializeTestValue);
            Assert.AreEqual("OnSerialized", instance.SerializeTestValue2);
            Assert.AreEqual("OnSerializing", result.SerializeTestValue);
            Assert.AreEqual("OnSerializing", result.SerializeTestValue2);
            Assert.AreEqual("test", result.DeSerializeTestValue3);
            Assert.IsNull(result.DeSerializeTestValue4);
            Assert.AreEqual("OnDeserialized", result.DeSerializeTestValue);
            Assert.AreEqual("OnDeserialized", result.DeSerializeTestValue2);
        }

        [TestMethod]
        public void TestDerivedCallbacks()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(DerivedCallbackTestClass));
            serializer.AddType(typeof(CallbackTestClass));

            var instance = new DerivedCallbackTestClass();
            instance.DeSerializeTestValue3 = "test";
            var xml = serializer.Serialize(instance);
            var result = serializer.Deserialize<DerivedCallbackTestClass>(xml);
            Assert.AreEqual("DerivedOnSerialized", instance.SerializeTestValue);
            Assert.AreEqual("DerivedOnSerialized", instance.SerializeTestValue2);
            Assert.AreEqual("DerivedOnSerializing", result.SerializeTestValue);
            Assert.AreEqual("DerivedOnSerializing", result.SerializeTestValue2);
            Assert.AreEqual("test", result.DeSerializeTestValue3);
            Assert.IsNull(result.DeSerializeTestValue4);
            Assert.AreEqual("DerivedOnDeserialized", result.DeSerializeTestValue);
            Assert.AreEqual("DerivedOnDeserialized", result.DeSerializeTestValue2);
        }

        [TestMethod]
        public void TestDeserializingPriority()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(PriorityCallbackTestClass));

            var instance = new PriorityCallbackTestClass();
            var xml = serializer.Serialize(instance);
            var result = serializer.Deserialize<PriorityCallbackTestClass>(xml);
            Assert.AreEqual("-1,0,1,2,9", result.DeserializeOrder);
        }

        [TestMethod]
        public void TestSystemCallbacks()
        {
            var serializer = new XmSerializerModel();
            serializer.AddType(typeof(SystemCallbackTestClass));

            var instance = new SystemCallbackTestClass();
            instance.DeSerializeTestValue3 = "test";
            var xml = serializer.Serialize(instance);
            var result = serializer.Deserialize<SystemCallbackTestClass>(xml);
            Assert.AreEqual("OnSerialized", instance.SerializeTestValue);
            Assert.AreEqual("OnSerialized", instance.SerializeTestValue2);
            Assert.AreEqual("OnSerializing", result.SerializeTestValue);
            Assert.AreEqual("OnSerializing", result.SerializeTestValue2);
            Assert.AreEqual("test", result.DeSerializeTestValue3);
            Assert.IsNull(result.DeSerializeTestValue4);
            Assert.AreEqual("OnDeserialized", result.DeSerializeTestValue);
            Assert.AreEqual("OnDeserialized", result.DeSerializeTestValue2);
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class CallbackTestClass
        {
            public string DeSerializeTestValue;
            public string DeSerializeTestValue2;
            public string DeSerializeTestValue3;
            public string DeSerializeTestValue4;
            public string SerializeTestValue;
            public string SerializeTestValue2;

            [OnDeserialized]
            public virtual void OnDeserializedTestMethod()
            {
                this.DeSerializeTestValue = "OnDeserialized";
            }

            [OnDeserialized]
            public virtual void OnDeserializedTestMethod(object parameter)
            {
                this.DeSerializeTestValue2 = "OnDeserialized";
            }

            [OnDeserializing]
            public virtual void OnDeserializingTestMethod()
            {
                this.DeSerializeTestValue = "OnDeserializing";
                this.DeSerializeTestValue3 = "OnDeserializing";
            }

            [OnDeserializing]
            public virtual void OnDeserializingTestMethod(object parameter)
            {
                this.DeSerializeTestValue = "OnDeserializing";
                this.DeSerializeTestValue4 = "OnDeserializing";
            }

            [OnSerialized]
            public virtual void OnSerializedTestMethod()
            {
                this.SerializeTestValue = "OnSerialized";
            }

            [OnSerialized]
            public virtual void OnSerializedTestMethod(object parameter)
            {
                this.SerializeTestValue2 = "OnSerialized";
            }

            [OnSerializing]
            public virtual void OnSerializingTestMethod()
            {
                this.SerializeTestValue2 = "OnSerializing";
            }

            [OnSerializing]
            public virtual void OnSerializingTestMethod(object parameter)
            {
                this.SerializeTestValue = "OnSerializing";
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class DerivedCallbackTestClass : CallbackTestClass
        {
            public override void OnDeserializedTestMethod()
            {
                this.DeSerializeTestValue = "DerivedOnDeserialized";
            }

            public override void OnDeserializedTestMethod(object parameter)
            {
                this.DeSerializeTestValue2 = "DerivedOnDeserialized";
            }

            public override void OnDeserializingTestMethod()
            {
                this.DeSerializeTestValue = "DerivedOnDeserializing";
                this.DeSerializeTestValue3 = "DerivedOnDeserializing";
            }

            public override void OnDeserializingTestMethod(object parameter)
            {
                this.DeSerializeTestValue = "DerivedOnDeserializing";
                this.DeSerializeTestValue4 = "DerivedOnDeserializing";
            }

            public override void OnSerializedTestMethod()
            {
                this.SerializeTestValue = "DerivedOnSerialized";
            }

            public override void OnSerializedTestMethod(object parameter)
            {
                this.SerializeTestValue2 = "DerivedOnSerialized";
            }

            public override void OnSerializingTestMethod()
            {
                this.SerializeTestValue2 = "DerivedOnSerializing";
            }

            public override void OnSerializingTestMethod(object parameter)
            {
                this.SerializeTestValue = "DerivedOnSerializing";
            }
        }

        [SerializableType]
        public class PriorityCallbackTestClass
        {
            public string DeserializeOrder { get; set; }

            [OnDeserialized]
            private void OnDeserialized0()
            {
                this.DeserializeOrder += ",0";
            }

            [OnDeserialized(1)]
            private void OnDeserialized1()
            {
                this.DeserializeOrder += ",1";
            }

            [OnDeserialized(2)]
            private void OnDeserialized2()
            {
                this.DeserializeOrder += ",2";
            }

            [OnDeserialized(9)]
            private void OnDeserialized9()
            {
                this.DeserializeOrder += ",9";
            }

            [OnDeserialized(-1)]
            private void OnDeserializedN1()
            {
                this.DeserializeOrder += "-1";
            }
        }

        [SerializableType(ImplicitMemberFilter = MemberFilter.PublicFields)]
        public class SystemCallbackTestClass
        {
            public string DeSerializeTestValue;
            public string DeSerializeTestValue2;
            public string DeSerializeTestValue3;
            public string DeSerializeTestValue4;
            public string SerializeTestValue;
            public string SerializeTestValue2;

            [System.Runtime.Serialization.OnDeserialized]
            public virtual void OnDeserializedTestMethod()
            {
                this.DeSerializeTestValue = "OnDeserialized";
            }

            [System.Runtime.Serialization.OnDeserialized]
            public virtual void OnDeserializedTestMethod(object parameter)
            {
                this.DeSerializeTestValue2 = "OnDeserialized";
            }

            [System.Runtime.Serialization.OnDeserializing]
            public virtual void OnDeserializingTestMethod()
            {
                this.DeSerializeTestValue = "OnDeserializing";
                this.DeSerializeTestValue3 = "OnDeserializing";
            }

            [System.Runtime.Serialization.OnDeserializing]
            public virtual void OnDeserializingTestMethod(object parameter)
            {
                this.DeSerializeTestValue = "OnDeserializing";
                this.DeSerializeTestValue4 = "OnDeserializing";
            }

            [System.Runtime.Serialization.OnSerialized]
            public virtual void OnSerializedTestMethod()
            {
                this.SerializeTestValue = "OnSerialized";
            }

            [System.Runtime.Serialization.OnSerialized]
            public virtual void OnSerializedTestMethod(object parameter)
            {
                this.SerializeTestValue2 = "OnSerialized";
            }

            [System.Runtime.Serialization.OnSerializing]
            public virtual void OnSerializingTestMethod()
            {
                this.SerializeTestValue2 = "OnSerializing";
            }

            [System.Runtime.Serialization.OnSerializing]
            public virtual void OnSerializingTestMethod(object parameter)
            {
                this.SerializeTestValue = "OnSerializing";
            }
        }
    }
}
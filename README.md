XMSerializer is a modular, extensible library that facilitates XML serialization of complex .NET object graphs.

### Targets
- .NET Framework 4.5
- .NET Standard 2.0

### Nuget
https://www.nuget.org/packages/XMSerializer/

### Features
* XML-Serialization of arbitrarily complex .NET object graphs
* Serialization can be set up in multiple ways:
    - Using Runtime model - ideal if you don't have access to the source of the type you are serializing
    - Using Attributes (using attributes on members)
    - Implicitly (using a single attribute on the type)   
* Reference tracking 
    - multiple references to the same instance are detected during serialization and restored during deserialization
    - includes circular references   
* Parameter-less constructor is not required (but can be used if available)
* Callbacks before and after (De)Serialization
* Serialization of actual runtime type (as opposed to the type of the reference)
* Full support for generics
* Full support for inheritance chains
* Compatibility - successful deserialization even after:
    - Renaming fields / properties
    - Renaming classes
    - Changing inheritance chain
    - Changing from field to property and vice versa
    - Adding members (with default value)
    - Removing members
    - Adding / reordering enum members
    - Changing the type of members

### Usage

Data transfer object with serialization set up via attributes:
~~~
[SerializableType("My Object")]
public class MyObject
{
    [Serialized("My Property")] public string MyProperty { get; set; }
}
~~~

Code used to (de)serialize:
~~~
var serializer = new XmSerializerModel();
serializer.AddType(typeof(MyObject));
var myObject = new MyObject();
var xml = serializer.Serialize(myObject);
var myObjectCopy = serializer.Deserialize<MyObject>(xml);
~~~

Generated XML:
~~~
<object id="1" type="My Object">
  <property name="My Property">
    <string value="Test" />
  </property>
</object>
~~~

For more usage examples, take a look at the [unit tests](https://github.com/Jonarw/XMSerializer/tree/master/XMSerializer.Tests) in `XMSerializer.Tests`.

### Supported out-of-the-box
* Primitives 
    - `double`
    - `int`
    - `bool`
    - `uint`
    - `long`
    - `ulong`
    - `string`
    - `short`
    - `byte`
    - `DateTime`
    - `TimeSpan`
    - `DateTimeOffset`
    - `decimal`
    - `float`
* `Array` (one-dimensional or multi-dimensional)
* `Enum`
* `Nullable<>`
* `Tuple<>`
* `ValueTuple<>`
* Collections
    - `List<>`
    - `Collection<>`
    - `ArrayList`
    - `SortedSet<>`
    - `IList`
    - `IList<>`
    - `IEnumerable`
    - `IEnumerable<>`
    - `IReadOnlyList<>`
    - `IReadOnlyCollection<>`
    - `ICollection`
    - `ICollection<>`
    - `ISet<>`
* Dictionaries
    - `Dictionary<,>`
    - `SortedList<,>`
    - `SortedDictionary<,>`
    - `ConcurrentDictionary<,>`
    - `IDictionary`
    - `IDictionary<,>`
    - `IReadOnlyDictionary<,>`
   

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Xml;

namespace Sigma
{
    /// <summary>
    /// Provides extended services for data contracts.
    /// </summary>
    public static class DataContract
    {
        #region Global Code
        /// <summary>
        /// Register a dynamically-discovered type.
        /// </summary>
        public static void RegisterType(Type type)
        {
            resolver.RegisterType(type);
        }

        private static readonly DynamicContractResolver resolver = new DynamicContractResolver();
        #endregion

        /// <summary>
        /// Set an internal field of an object during data context initialization.
        /// Useful in the context of DataContext-serializable objects to maintain invariants.
        /// </summary>
        public static void Initialize<T, V>(T obj, string fieldName, V value)
        {
            var type = typeof(T);
            var optField = type.GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            optField.SetValue(obj, value);
        }

        /// <summary>
        /// Serialize according to an object's data-contract.
        /// </summary>
        public static void Serialize(object source, Stream target)
        {
            var settings = new DataContractSerializerSettings() { DataContractResolver = resolver };
            var serializer = new DataContractSerializer(source.GetType(), settings);
            using (var writer = XmlWriter.Create(target, new XmlWriterSettings() { Indent = true })) serializer.WriteObject(writer, source);
        }

        /// <summary>
        /// Deserialize according to an object's data-contract.
        /// </summary>
        public static object Deserialize(Type targetType, Stream source)
        {
            var settings = new DataContractSerializerSettings() { DataContractResolver = resolver };
            var serializer = new DataContractSerializer(targetType, settings);
            return serializer.ReadObject(source);
        }

        /// <summary>
        /// Deserialize according to an object's data-contract.
        /// </summary>
        public static T Deserialize<T>(Stream source)
        {
            return (T)Deserialize(typeof(T), source);
        }

        /// <summary>
        /// Test the accuracy of a data-contract's serialization.
        /// </summary>
        public static bool TestSerialization<T>(T source, Func<T, T, bool> comparator)
        {
            var stream = new MemoryStream();
            Serialize(source, stream);
#if DEBUG
            // NOTE: this is just here for debugging
            stream.Seek(0L, SeekOrigin.Begin);
            var document = new StreamReader(stream).ReadToEnd();
#endif
            stream.Seek(0L, SeekOrigin.Begin);
            var target = Deserialize<T>(stream);
            return comparator(source, target);
        }

        /// <summary>
        /// Test the accuracy of a data-contract's serialization.
        /// </summary>
        public static bool TestSerialization<T>(T source)
        {
            return TestSerialization(source, (left, right) => left.Equals(right));
        }
    }

    /// <summary>
    /// Allows the dynamic resolution of data contracts.
    /// </summary>
    public class DynamicContractResolver : DataContractResolver
    {
        #region Global Code
        public void RegisterType(Type type)
        {
            registeredTypes.Add(type);
        }

        public override Type ResolveName(string typeName, string typeNamespace, Type declaredType, DataContractResolver knownTypeResolver)
        {
            var resolvedType =
                (from type in registeredTypes
                 where type.Name == typeName
                 where $"http://schemas.datacontract.org/2004/07/{type.Namespace}" == typeNamespace
                 select type)
                .FirstOrDefault(knownTypeResolver.ResolveName(typeName, typeNamespace, declaredType, null));
            return resolvedType;
        }

        public override bool TryResolveType(Type type, Type declaredType, DataContractResolver knownTypeResolver, out XmlDictionaryString typeName, out XmlDictionaryString typeNamespace)
        {
            return knownTypeResolver.TryResolveType(type, declaredType, null, out typeName, out typeNamespace);
        }

        private readonly HashSet<Type> registeredTypes = new HashSet<Type>();
        #endregion
    }
    /// <summary>
    /// Type Converter Contract functions.
    /// </summary>
    public static class Tcc
    {
        /// <summary>
        /// Create a Tcc instance using the given value.
        /// </summary>
        public static Tcc<T> Create<T>(T value)
        {
            return new Tcc<T>(value);
        }
    }

    /// <summary>
    /// Tcc - Type Converter Contract. Used to implement a DataContract for type T using just T's type converter.
    /// </summary>
    [DataContract, TypeConverter(typeof(TccConverter))]
    public class Tcc<T> : IEquatable<Tcc<T>>
    {
        /// <summary>
        /// Create a Tcc with the given value.
        /// </summary>
        public Tcc(T value)
        {
            this.value = value;
        }

        /// <summary>
        /// The underlying value.
        /// </summary>
        [IgnoreDataMember]
        public T Value { get { return value; } }

        /// <summary>
        /// The string representation of the value.
        /// </summary>
        [DataMember]
        public string String
        {
            get
            {
                var valueConverter = TypeDescriptor.GetConverter(typeof(T));
                return valueConverter.ConvertToString(value);
            }
            set
            {
                var valueConverter = TypeDescriptor.GetConverter(typeof(T));
                var valueField = GetType().GetField(nameof(Tcc<T>.value));
                var value2 = (T)valueConverter.ConvertFromString(value);
                this.value = value2;
            }
        }

        public override bool Equals(object other)
        {
            return other is Tcc<T> ? Equals((Tcc<T>)other) : false;
        }

        public bool Equals(Tcc<T> other)
        {
            return Value.Equals(other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override string ToString()
        {
            return String;
        }

        private T value;
    }

    /// <summary>
    /// A type converter for type converter contracts.
    /// </summary>
    public class TccConverter : TypeConverter
    {
        public TccConverter(Type pointType)
        {
            this.pointType = pointType;
            var valueProperty = pointType.GetProperty(nameof(Tcc<bool>.Value));
            valueConverter = TypeDescriptor.GetConverter(valueProperty.PropertyType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return valueConverter.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var valueProperty = pointType.GetProperty(nameof(Tcc<bool>.Value));
            var value2 = valueConverter.ConvertFrom(context, culture, value);
            return Activator.CreateInstance(pointType, new object[] { value2 });
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return valueConverter.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            var valueProperty = pointType.GetProperty(nameof(Tcc<bool>.Value));
            var value2 = valueProperty.GetValue(value);
            return valueConverter.ConvertTo(context, culture, value2, destinationType);
        }

        private readonly Type pointType;
        private readonly TypeConverter valueConverter;
    }
}

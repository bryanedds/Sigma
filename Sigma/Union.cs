using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Sigma
{
    /// <summary>
    /// Associates a non-flag enum member with data of the given type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class UnionAttribute : Attribute
    {
        public UnionAttribute(Type type) { this.type = type; }
        public Type Type { get { return type; } }
        private readonly Type type;
    }

    /// <summary>
    /// Similar to a sum type (a la F# Discriminated Unions), but with less static type safety due to C# limitations.
    /// NOTE: the struct, IConvertible type constraints are intended to be enum constraint since it's unavailable in
    /// C#.
    /// </summary>
    [DataContract, TypeConverter(typeof(UnionTypeConverter))]
    public class Union<T, V> : IEquatable<Union<T, V>> where T : struct, IConvertible
    {
        public Union(T tag, V data)
        {
#if DEBUG
            var unionType = ((Enum)(object)tag).TryGetAttributeOfType<UnionAttribute>().TryThen(attr => attr.Type) ?? typeof(V);
            if (!unionType.IsInstanceOfType(data))
                throw new ArgumentException($"Union instantiation did not satisfy type requirements for type '{GetType().FullName}'.");
#endif
            Tag = tag;
            Data = data;
        }

        public override bool Equals(object other)
        {
            return other is Union<T, V> && Equals((Union<T, V>)other);
        }

        public bool Equals(Union<T, V> other)
        {
            return
                Tag.Equals(other.Tag) &&
                Data.Equals(other.Data);
        }

        public override int GetHashCode()
        {
            return Tag.GetHashCode() ^ Data.GetHashCode();
        }

        public override string ToString()
        {
            return Conversion.ValueToString(this);
        }

        public readonly T Tag;
        public readonly V Data;
    }

    /// <summary>
    /// A generalized type converter for unions.
    /// </summary>
    public class UnionTypeConverter : TypeConverter
    {
        public UnionTypeConverter(Type pointType)
        {
            this.pointType = pointType;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return
                sourceType == pointType ||
                sourceType == typeof(string) ||
                sourceType == typeof(Symbol);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            if (sourceType == pointType) return value;
            if (sourceType == typeof(string)) return ConvertFrom(context, culture, Symbol.FromString((string)value));
            if (sourceType == typeof(Symbol))
            {
                var symbol = (Symbol)value;
                try
                {
                    var fields = symbol.AsSymbols;
                    var tagField = pointType.GetField(nameof(Union<bool, bool>.Tag));
                    var tagConverter = new SymbolicConverter(tagField.FieldType);
                    var tag = (Enum)tagConverter.ConvertFromString(fields[0].AsAtom);
                    var dataField = pointType.GetField(nameof(Union<bool, bool>.Data));
                    var dataConverter = new SymbolicConverter(tag.TryGetAttributeOfType<UnionAttribute>().TryThen(attr => attr.Type) ?? dataField.FieldType);
                    var data = fields[1].Match(
                        atom => dataConverter.ConvertFromString(atom),
                        number => dataConverter.ConvertFromString(number),
                        str => dataConverter.ConvertFromString(str),
                        _ => fields[1],
                        _ => dataConverter.ConvertFrom(fields[1]));
                    return Activator.CreateInstance(pointType, new object[] { tag, data });
                }
                catch (Exception exn)
                {
                    throw new ArgumentException($"Invalid form '{symbol}' for {value.GetType().FullName}. Required form is '[Enum Value]'.", exn);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return
                destinationType == pointType ||
                destinationType == typeof(string) ||
                destinationType == typeof(Symbol);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == pointType) return value;
            if (destinationType == typeof(string)) return ConvertTo(context, culture, value, typeof(Symbol)).ToString();
            if (destinationType == typeof(Symbol))
            {
                var tagField = pointType.GetField(nameof(Union<bool, bool>.Tag));
                var tag = (Enum)tagField.GetValue(value);
                var tagConverter = new SymbolicConverter(tagField.FieldType);
                var tagSymbol = new Symbol(tagConverter.ConvertToString(tag));
                var dataField = pointType.GetField(nameof(Union<bool, bool>.Data));
                var data = dataField.GetValue(value);
                var dataConverter = new SymbolicConverter(tag.TryGetAttributeOfType<UnionAttribute>().TryThen(attr => attr.Type) ?? dataField.FieldType);
                var dataSymbol = dataConverter.CanConvertTo(typeof(Symbol)) ? (Symbol)dataConverter.ConvertTo(data, typeof(Symbol)) : new Symbol(dataConverter.ConvertToString(data));
                return new Symbol(new List<Symbol>() { tagSymbol, dataSymbol });
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private readonly Type pointType;
    }
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Sigma
{
    /// <summary>
    /// A record is a Tuple but with an enhanced type converter.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1> : Tuple<T1>
    {
        /// <summary>
        /// Create a Record of 1 item.
        /// </summary>
        public Record(T1 item1) : base(item1) { }
    }

    /// <summary>
    /// A record is a Tuple but with an enhanced type converter.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2> : Tuple<T1, T2>
    {
        /// <summary>
        /// Create a Record of 2 items.
        /// </summary>
        public Record(T1 item1, T2 item2) : base(item1, item2) { }
    }

    /// <summary>
    /// A record is a Tuple but with enhanced serialization.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2, T3> : Tuple<T1, T2, T3>
    {
        /// <summary>
        /// Create a Record of 3 items.
        /// </summary>
        public Record(T1 item1, T2 item2, T3 item3) : base(item1, item2, item3) { }
    }

    /// <summary>
    /// A record is a Tuple but with enhanced serialization.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2, T3, T4> : Tuple<T1, T2, T3, T4>
    {
        /// <summary>
        /// Create a Record of 4 items.
        /// </summary>
        public Record(T1 item1, T2 item2, T3 item3, T4 item4) : base(item1, item2, item3, item4) { }
    }

    /// <summary>
    /// A record is a Tuple but with enhanced serialization.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2, T3, T4, T5> : Tuple<T1, T2, T3, T4, T5>
    {
        /// <summary>
        /// Create a Record of 5 items.
        /// </summary>
        public Record(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) : base(item1, item2, item3, item4, item5) { }
    }

    /// <summary>
    /// A record is a Tuple but with enhanced serialization.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2, T3, T4, T5, T6> : Tuple<T1, T2, T3, T4, T5, T6>
    {
        /// <summary>
        /// Create a Record of 6 items.
        /// </summary>
        public Record(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6) : base(item1, item2, item3, item4, item5, item6) { }
    }

    /// <summary>
    /// A record is a Tuple but with enhanced serialization.
    /// </summary>
    [TypeConverter(typeof(RecordTypeConverter))]
    public class Record<T1, T2, T3, T4, T5, T6, T7> : Tuple<T1, T2, T3, T4, T5, T6, T7>
    {
        /// <summary>
        /// Create a Record of 7 items.
        /// </summary>
        public Record(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7) : base(item1, item2, item3, item4, item5, item6, item7) { }
    }

    /// <summary>
    /// Converts record types.
    /// </summary>
    public class RecordTypeConverter : TypeConverter
    {
        public RecordTypeConverter(Type pointType)
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
                    var symbols = symbol.AsSymbols;
                    var fields = symbols
                        .Select((fieldSymbol, index) => {
                            var fieldName = "Item" + (index + 1);
                            var fieldProperty = pointType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                            var fieldConverter = TypeDescriptor.GetConverter(fieldProperty.PropertyType);
                            var fieldValue = fieldSymbol.Match(
                                atom => fieldConverter.ConvertFromString(atom),
                                number => fieldConverter.ConvertFromString(number),
                                str => fieldConverter.ConvertFromString(str),
                                _ => fieldSymbol,
                                _ => fieldConverter.ConvertFrom(fieldSymbol));
                            return fieldValue; })
                        .ToArray();
                    return Activator.CreateInstance(pointType, fields);
                }
                catch (Exception exn)
                {
                    throw new ArgumentException($"Invalid form '{symbol}' for {value.GetType().FullName}. Required form is '[Value ...]'.", exn);
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
                var recordType = pointType.GetConcreteTypes().Where(type => type.Name.StartsWith("Record`")).First();
                var recordSize = recordType.GetGenericArguments().Length;
                var fieldSymbols =
                    Enumerable.Range(0, recordSize).Select(index =>
                    {
                        var fieldName = "Item" + (index + 1);
                        var field = pointType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance);
                        var fieldValue = field.GetValue(value, null);
                        var fieldConverter = TypeDescriptor.GetConverter(field.PropertyType);
                        return fieldConverter.CanConvertTo(typeof(Symbol)) ? (Symbol)fieldConverter.ConvertTo(fieldValue, typeof(Symbol)) : new Symbol(fieldConverter.ConvertToString(fieldValue));
                    });
                return new Symbol(fieldSymbols);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }

        private readonly Type pointType;
    }
}

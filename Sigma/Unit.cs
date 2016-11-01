using System;
using System.ComponentModel;
using System.Globalization;

namespace Sigma
{
    /// <summary>
    /// The unit type; a type with no value and no alternative possible states.
    /// </summary>
    [TypeConverter(typeof(UnitTypeConverter))]
    public class Unit
    {
        public static readonly Unit Value = new Unit();
        public override string ToString() { return Conversion.ValueToString(this); }
        private Unit() { }
    }

    /// <summary>
    /// A type converter for the Unit type.
    /// </summary>
    public class UnitTypeConverter : TypeConverter
    {
        public UnitTypeConverter(Type pointType)
        {
            this.pointType = pointType;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return
                sourceType == pointType ||
                sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            if (sourceType == pointType) return value;
            if (sourceType == typeof(string))
            {
                var valueStr = (string)value;
                if (valueStr.Trim() == nameof(Unit)) return Unit.Value;
                throw new ArgumentException($"Conversion source string '{valueStr}' should be '{nameof(Unit)}'.");
            }
            throw new ArgumentException($"Cannot convert from '{value.GetType().FullName}' type to Unit.");
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return
                destinationType == pointType ||
                destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == pointType) return value;
            if (destinationType == typeof(string)) return nameof(Unit);
            throw new ArgumentException($"Cannot convert Unit to type '{destinationType.FullName}'.");
        }

        private readonly Type pointType;
    }
}

using System;
using System.ComponentModel;

namespace Sigma
{
    public class SymbolicConverter : TypeConverter
    {
        public SymbolicConverter(Type pointType)
        {
            this.pointType = pointType;
        }

        private static Symbol ToSymbol(Type sourceType, object source)
        {
            var optTypeConverter = sourceType.TryGetCustomTypeConverter();
            if (optTypeConverter != null)
            {
                // symbolize user-defined type
                if (!optTypeConverter.CanConvertTo(typeof(Symbol)))
                    throw new ConversionException("Cannot convert type '" + source.GetType().Name + "' to Sigma.Symbol.");
                else
                    return (Symbol)optTypeConverter.ConvertTo(source, typeof(Symbol));
            }
            else
            {
                // symbolize .NET primitive
                if (sourceType.IsPrimitive)
                {
                    var converted = (string)TypeDescriptor.GetConverter(sourceType).ConvertTo(source, typeof(string));
                    if (sourceType == typeof(bool)) return new Symbol(converted);
                    if (sourceType == typeof(char)) return new Symbol(new StringSpecifier(converted));
                    return new Symbol(new NumberSpecifier(converted));
                }

                // symbolize string
                if (sourceType == typeof(string))
                {
                    var sourceStr = source.ToString();
                    if (SymbolParser.IsNumber(sourceStr)) return new Symbol(new NumberSpecifier(sourceStr));
                    if (SymbolParser.ShouldBeExplicit(sourceStr)) return new Symbol(new StringSpecifier(sourceStr));
                    return new Symbol(sourceStr);
                }

                // symbolize Symbol (no transformation)
                if (sourceType == typeof(Symbol))
                {
                    return (Symbol)source;
                }

                // symbolize vanilla .NET type...
                var typeConverter = TypeDescriptor.GetConverter(sourceType);
                
                // HACK: we do not want to use this converter here as it strips the time when converting to string!
                if (typeConverter is DateTimeConverter)
                {
                    return new Symbol(new StringSpecifier(source.ToString()));
                }

                if (typeConverter.CanConvertTo(typeof(Symbol)))
                {
                    return (Symbol)typeConverter.ConvertTo(source, typeof(Symbol));
                }

                return new Symbol((string)typeConverter.ConvertTo(source, typeof(string)));
            }
        }

        private readonly Type pointType;
    }
}

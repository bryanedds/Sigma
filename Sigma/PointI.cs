using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Linq;

namespace Sigma
{
    /// <summary>
    /// A 2-dimensional integer value.
    /// </summary>
    [DataContract, TypeConverter(typeof(PointIConverter))]
    public class PointI : IEquatable<PointI>
    {
        public PointI() : this(0) { }
        public PointI(int v) : this(v, v) { }
        public PointI(int x, int y) { X = x; Y = y; }
        [DataMember] public readonly int X;
        [DataMember] public readonly int Y;
        public static PointI operator +(PointI left, PointI right) { return new PointI(left.X + right.X, left.Y + right.Y); }
        public static PointI operator -(PointI left, PointI right) { return new PointI(left.X - right.X, left.Y - right.Y); }
        public static bool operator ==(PointI left, PointI right) { return left.Equals(right); }
        public static bool operator !=(PointI left, PointI right) { return !left.Equals(right); }
        public static PointI Zero { get { return new PointI(); } }
        public static PointI One { get { return new PointI(1); } }

        public override bool Equals(object other)
        {
            return other is PointI ? Equals((PointI)other) : false;
        }

        public bool Equals(PointI other)
        {
            return
                X == other.X &&
                Y == other.Y;
        }

        public override int GetHashCode()
        {
            return X + (Y << 16);
        }
    }

    /// <summary>
    /// Formats PointI's with an s-expression syntax.
    /// </summary>
    public class PointIConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return
                sourceType == typeof(PointI) ||
                sourceType == typeof(string) ||
                sourceType == typeof(Symbol);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            if (sourceType == typeof(PointI)) return value;
            if (sourceType == typeof(string)) return ConvertFrom(context, culture, Symbol.FromString((string)value));
            if (sourceType == typeof(Symbol))
            {
                var symbol = (Symbol)value;
                try
                {
                    var fields = symbol.AsSymbols;
                    var ints = fields.Select(field => int.Parse(field.AsNumber)).ToArray();
                    return new PointI(ints[0], ints[0]);
                }
                catch (Exception exn)
                {
                    throw new ArgumentException($"Invalid form '{symbol}' for {nameof(PointI)}. Required form is '[Int Int]'.", exn);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return
                destinationType == typeof(PointI) ||
                destinationType == typeof(string) ||
                destinationType == typeof(Symbol);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(PointI)) return value;
            if (destinationType == typeof(string)) return ConvertTo(context, culture, value, typeof(Symbol)).ToString();
            if (destinationType == typeof(Symbol))
            {
                var point = (PointI)value;
                return
                    new Symbol(new List<Symbol> {
                        new Symbol(new NumberSpecifier(point.X.ToString())),
                        new Symbol(new NumberSpecifier(point.Y.ToString())) });
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

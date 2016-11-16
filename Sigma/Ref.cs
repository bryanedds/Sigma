using System;
using System.ComponentModel;
using System.Globalization;

namespace Sigma
{
    /// <summary>
    /// A mutable variable that can exist inside Unions and / or Records.
    /// Equality uses value semantics.
    /// </summary>
    [TypeConverter(typeof(RefConverter))]
    public class Ref<T> : IEquatable<Ref<T>>
    {
        public Ref(T value)
        {
            this.value = value;
        }

        public T Value
        {
            get { return value; }
            set
            {
                var oldValue = this.value;
                this.value = value;
                Callback?.Invoke(oldValue);
            }
        }

        public event Action<T> Callback;

        public bool Equals(Ref<T> other)
        {
            if (value == null) return other.Value == null;
            return value.Equals(other.Value);
        }

        public override bool Equals(object other)
        {
            var otherAsRefT = other as Ref<T>;
            if (otherAsRefT != null) return Equals(otherAsRefT);
            return false;
        }

        public override int GetHashCode()
        {
            if (value == null) return 0;
            return value.GetHashCode();
        }

        public static bool operator ==(Ref<T> left, Ref<T> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if ((object)left == null || (object)right == null) return false;
            return left.Equals(right);
        }

        public static bool operator !=(Ref<T> left, Ref<T> right)
        {
            return !(left == right);
        }

        private T value;
    }

    /// <summary>
    /// Type converter for Refs.
    /// </summary>
    public class RefConverter : TypeConverter
    {
        public RefConverter(Type pointType)
        {
            this.pointType = pointType;
            var valueProperty = pointType.GetProperty(nameof(Ref<bool>.Value));
            valueConverter = new SymbolicConverter(valueProperty.PropertyType);
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
            var valueProperty = pointType.GetProperty(nameof(Ref<bool>.Value));
            var value2 = valueConverter.ConvertFrom(context, culture, value);
            return Activator.CreateInstance(pointType, new object[] { value2 });
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
            var valueProperty = pointType.GetProperty(nameof(Ref<bool>.Value));
            var value2 = valueProperty.GetValue(value);
            return valueConverter.ConvertTo(context, culture, value2, destinationType);
        }

        private readonly Type pointType;
        private readonly TypeConverter valueConverter;
    }
}
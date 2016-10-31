using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sigma
{
    public static class Reflection
    {
        /// <summary>
        /// Get the type descriptor for this type as returned by the global TypeDescriptor.
        /// </summary>
        public static TypeConverter TryGetCustomTypeConverter(this Type type)
        {
            var globalConverterAttributes = TypeDescriptor.GetAttributes(type).OfType<TypeConverterAttribute>();
            var localConverterAttributes = type.GetCustomAttributes(typeof(TypeConverterAttribute), true).Cast<TypeConverterAttribute>();
            var typeConverterAttributes = Enumerable.Concat(globalConverterAttributes, localConverterAttributes);
            if (typeConverterAttributes.Any())
            {
                var typeConverterAttribute = typeConverterAttributes.First();
                var typeConverterTypeName = typeConverterAttribute.ConverterTypeName;
                var typeConverterType = Type.GetType(typeConverterTypeName);
                var ctor1 = typeConverterType.GetConstructor(new Type[] { typeof(Type) });
                if (ctor1 == null)
                {
                    var ctor0 = typeConverterType.GetConstructor(new Type[] { });
                    return (TypeConverter)ctor0.Invoke(new object[] { });
                }
                return (TypeConverter)ctor1.Invoke(new Type[] { type });
            }
            return null;
        }
    }
}

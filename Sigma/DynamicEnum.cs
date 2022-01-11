using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace Sigma
{
    /// <summary>
    /// A dynamic enum-style type.
    /// </summary>
    [TypeConverter(typeof(DynamicEnumTypeConverter))]
    public class DynamicEnum : ObservableDictionary<string, bool>
    {
        public DynamicEnum(bool flags) : this(flags, new List<KeyValuePair<string, bool>>()) { }

        public DynamicEnum(bool flags, IEnumerable<KeyValuePair<string, bool>> entries)
        {
            this.flags = flags;
            foreach (var entry in entries) Add(entry);
        }

        public DynamicEnum(bool flags, bool initialState, IEnumerable<string> names)
        {
            this.flags = flags;
            foreach (var name in names) Add(name, initialState);
        }

        public string Selection
        {
            get { return this.Where(entry => entry.Value).Select(entry => entry.Key).FirstOrDefault(string.Empty); }
        }

        public bool Flags
        {
            get { return flags; }
        }

        private readonly bool flags;
    }

    public class DynamicEnumTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return
                sourceType == typeof(DynamicEnum) ||
                sourceType == typeof(string) ||
                sourceType == typeof(Symbol);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var sourceType = value.GetType();
            if (sourceType == typeof(DynamicEnum)) return value;
            if (sourceType == typeof(string)) return ConvertFrom(context, culture, Symbol.FromString((string)value));
            if (sourceType == typeof(Symbol))
            {
                var symbol = (Symbol)value;
                try
                {
                    var symbols = symbol.ToSymbols;
                    var flagsSymbol = symbols[0].ToAtom;
                    var flags = bool.Parse(flagsSymbol);
                    var entrySymbols = symbols[1].ToSymbols;
                    var entries = entrySymbols.Select(entry => entry.ToSymbols.Then(fields => KeyValuePair.Create(fields[0].ToAtom, bool.Parse(fields[1].TryAtom))));
                    return new DynamicEnum(flags, entries);
                }
                catch (Exception exn)
                {
                    throw new ArgumentException($"Invalid form '{symbol}' for {nameof(DynamicEnum)}. Required form is '[Bool [[String Bool] ...]]'.", exn);
                }
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return
                destinationType == typeof(DynamicEnum) ||
                destinationType == typeof(string) ||
                destinationType == typeof(Symbol);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(DynamicEnum)) return value;
            if (destinationType == typeof(string)) return ConvertTo(context, culture, value, typeof(Symbol)).ToString();
            if (destinationType == typeof(Symbol))
            {
                var dynum = (DynamicEnum)value;
                return
                    new Symbol(new List<Symbol>() {
                        new Symbol(dynum.Flags.ToString()),
                        new Symbol(dynum.Entries.Select(entry =>
                            new Symbol(new List<Symbol>() {
                                new Symbol(entry.Key),
                                new Symbol(entry.Value.ToString()) }))) });
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

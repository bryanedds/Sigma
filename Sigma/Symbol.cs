using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sprache;
using System.Runtime.Serialization;

namespace Sigma
{
    /// <summary>
    /// A generalized representation of all possible values.
    /// </summary>
    public class Symbol : Union<SymbolTag, object>, IEquatable<Symbol>
    {
        public Symbol(string atom) : base(SymbolTag.Atom, atom) { }
        public Symbol(NumberSpecifier number) : base(SymbolTag.Number, number.Item1) { }
        public Symbol(StringSpecifier str) : base(SymbolTag.String, str.Item1) { }
        public Symbol(QuoteSpecifier quote) : base(SymbolTag.Quote, quote.Item1) { }
        public Symbol(IEnumerable<Symbol> symbols) : base(SymbolTag.Symbols, symbols.ToImmutableList()) { }
        public Symbol(SymbolTag tag, object data) : base(tag, data) { }

        public ImmutableList<Symbol> AsSymbols
        {
            get
            {
                if (Tag != SymbolTag.Symbols) throw new InvalidOperationException();
                return (ImmutableList<Symbol>)Data;
            }
        }

        public string AsQuote
        {
            get
            {
                if (Tag != SymbolTag.Quote) throw new InvalidOperationException();
                return (string)Data;
            }
        }

        public string AsString
        {
            get
            {
                if (Tag != SymbolTag.String) throw new InvalidOperationException();
                return (string)Data;
            }
        }

        public string AsNumber
        {
            get
            {
                if (Tag != SymbolTag.Number) throw new InvalidOperationException();
                return (string)Data;
            }
        }

        public string AsAtom
        {
            get
            {
                if (Tag != SymbolTag.Atom) throw new InvalidOperationException();
                return (string)Data;
            }
        }

        public bool Equals(Symbol other)
        {
            if (other == null) return false;
            if (Tag != other.Tag) return false;
            switch (Tag)
            {
                case SymbolTag.Atom: return AsAtom == other.AsAtom;
                case SymbolTag.Number: return AsNumber == other.AsNumber;
                case SymbolTag.String: return AsString == other.AsString;
                case SymbolTag.Quote: return AsQuote == other.AsQuote;
                case SymbolTag.Symbols: return AsSymbols.SequenceEqual(other.AsSymbols);
                default: throw new InexhausiveException();
            }
        }

        public override bool Equals(object other)
        {
            var otherSymbol = other as Symbol;
            return Equals(otherSymbol);
        }

        public override int GetHashCode()
        {
            switch (Tag)
            {
                case SymbolTag.Atom: return AsAtom.GetHashCode();
                case SymbolTag.Number: return AsNumber.GetHashCode();
                case SymbolTag.String: return AsString.GetHashCode();
                case SymbolTag.Quote: return AsQuote.GetHashCode();
                case SymbolTag.Symbols: return AsSymbols.Aggregate(0, (hash, symbol) => hash ^ symbol.GetHashCode());
                default: throw new InexhausiveException();
            }
        }

        public override string ToString()
        {
            return SymbolParser.UnparseSymbol(this);
        }

        public static Symbol FromString(string sexpr)
        {
            return SymbolParser.ParseSymbol(sexpr);
        }

        public T Match<T>(
            Func<string, T> onAtom,
            Func<string, T> onNumber,
            Func<string, T> onString,
            Func<string, T> onQuote,
            Func<ImmutableList<Symbol>, T> onSymbols)
        {
            switch (Tag)
            {
                case SymbolTag.Atom: return onAtom(AsAtom);
                case SymbolTag.Number: return onNumber(AsNumber);
                case SymbolTag.String: return onString(AsString);
                case SymbolTag.Quote: return onQuote(AsQuote);
                case SymbolTag.Symbols: return onSymbols(AsSymbols);
                default: throw new InexhausiveException();
            }
        }

        public T Match4<T>(
            Func<string, T> onAtom,
            Func<string, T> onString,
            Func<string, T> onNumber,
            Func<ImmutableList<Symbol>, T> onSymbols)
        {
            return Match(
                onAtom,
                onNumber,
                onString,
                quote => Expression<T>.Throw<InexhausiveException>(),
                onSymbols);
        }

        public T Match3<T>(
            Func<string, T> onAtom,
            Func<string, T> onQuote,
            Func<ImmutableList<Symbol>, T> onSymbols)
        {
            return Match(
                onAtom,
                number => Expression<T>.Throw<InexhausiveException>(),
                str => Expression<T>.Throw<InexhausiveException>(),
                onQuote,
                onSymbols);
        }

        public T Match2<T>(
            Func<string, T> onAtom,
            Func<ImmutableList<Symbol>, T> onSymbols)
        {
            return Match(
                onAtom,
                number => Expression<T>.Throw<InexhausiveException>(),
                str => Expression<T>.Throw<InexhausiveException>(),
                quote => Expression<T>.Throw<InexhausiveException>(),
                onSymbols);
        }
    }

    /// <summary>
    /// Union tag for Symbol type.
    /// </summary>
    public enum SymbolTag
    {
        [Union(typeof(string))] Atom = 0,
        [Union(typeof(string))] Number,
        [Union(typeof(string))] String,
        [Union(typeof(string))] Quote,
        [Union(typeof(ImmutableList<Symbol>))] Symbols
    }

    /// Type specifier for Number.
    public class NumberSpecifier : Tuple<string> { public NumberSpecifier(string number) : base(number) { } }

    /// Type specifier for String.
    public class StringSpecifier : Tuple<string> { public StringSpecifier(string str) : base(str) { } }

    /// Type specifier for Quote.
    public class QuoteSpecifier : Tuple<string> { public QuoteSpecifier(string quote) : base(quote) { } }

    /// <summary>
    /// Convert strings and symbols, with the following parses:
    /// 
    /// (* Atom values *)
    /// None
    /// CharacterAnimationFacing
    /// 
    /// (* Number values *)
    /// 0.0f
    /// -5
    ///
    /// (* String value *)
    /// "String with quoted spaces."
    ///
    /// (* Quoted value *)
    /// `[Some 1]'
    /// 
    /// (* Symbols values *)
    /// []
    /// [Some 0]
    /// [Left 0]
    /// [[0 1] [2 4]]
    /// [AnimationData 4 8]
    /// [Gem `[Some 1]']
    ///
    /// ...and so on.
    /// </summary>
    public static class SymbolParser
    {
        public static char OpenSymbolsChar = '[';
        public static char CloseSymbolsChar = ']';
        public static char OpenStringChar = '\"';
        public static char CloseStringChar = '\"';
        public static char OpenQuoteChar = '`';
        public static char CloseQuoteChar = '\'';
        public static string NewlineChars = "\n\r";
        public static string WhitespaceChars = " \t" + NewlineChars;
        public static string StructureCharsNoStr = "[]`\'";
        public static string StructureChars = "\"" + StructureCharsNoStr;

        public static Parser<string> ReadWhitespaces =
            Parse.Chars(WhitespaceChars).Many().Text();

        public static Parser<string> ReadAtomChars =
            Parse.CharExcept(StructureChars + WhitespaceChars).AtLeastOnce().Text();

        public static Parser<string> ReadStringChars =
            Parse.CharExcept(CloseStringChar).Many().Text();

        public static Parser<string> ReadQuoteChars =
            Parse.CharExcept(CloseQuoteChar).Many().Text();

        public static Parser<Symbol> ReadAtom =
            from chars in ReadAtomChars
            from _ in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadNumber =
            from chars in Parse.Number
            from _ in ReadWhitespaces
            select new Symbol(new NumberSpecifier(chars));

        public static Parser<Symbol> ReadString =
            from _ in Parse.Char(OpenStringChar)
            from _2 in ReadWhitespaces
            from chars in ReadStringChars
            from _3 in Parse.Char(CloseStringChar)
            from _4 in ReadWhitespaces
            select new Symbol(new StringSpecifier(chars));

        public static Parser<Symbol> ReadQuote =
            from _ in Parse.Char(OpenQuoteChar)
            from _2 in ReadWhitespaces
            from chars in ReadQuoteChars
            from _3 in Parse.Char(CloseQuoteChar)
            from _4 in ReadWhitespaces
            select new Symbol(new QuoteSpecifier(chars));

        public static Parser<Symbol> ReadSymbols =
            from _ in Parse.Char(OpenSymbolsChar)
            from _2 in ReadWhitespaces
            from symbols in ReadSymbol.Many()
            from _3 in Parse.Char(CloseSymbolsChar)
            from _4 in ReadWhitespaces
            select new Symbol(symbols.ToImmutableList());

        public static Parser<Symbol> ReadSymbol =
            ReadQuote.Or(
            ReadString.Or(
            ReadNumber.Or(
            ReadAtom.Or(
            ReadSymbols))));

        public static Symbol ParseSymbol(string sexpr)
        {
            return
                (from _ in ReadWhitespaces
                 from symbol in ReadSymbol
                 select symbol)
                .Parse(sexpr);
        }

        public static string Distillate(string str)
        {
            return str
                .Replace(OpenStringChar.ToString(), string.Empty)
                .Replace(CloseStringChar.ToString(), string.Empty);
        }

        public static bool IsExplicit(string content)
        {
            return content.StartsWith(OpenStringChar.ToString()) && content.EndsWith(CloseStringChar.ToString());
        }

        public static bool IsNumber(string content)
        {
            return
                (from chars in Parse.Number
                 select chars)
                 .TryParse(content)
                 .WasSuccessful;
        }

        public static bool ShouldBeExplicit(string content)
        {
            return content.Any(chr => char.IsWhiteSpace(chr) || StructureCharsNoStr.Contains(chr));
        }

        private static string WriteAtom(string content)
        {
            var distilled = Distillate(content);
            if (distilled.Empty()) return $"{OpenStringChar}{CloseStringChar}";
            if (!IsExplicit(distilled) && ShouldBeExplicit(distilled)) return $"{OpenStringChar}{distilled}{CloseStringChar}";
            if (IsExplicit(distilled) && !ShouldBeExplicit(distilled)) return distilled.Substring(1, distilled.Length - 2);
            return distilled;
        }

        private static string WriteNumber(string content)
        {
            return Distillate(content);
        }

        private static string WriteString(string content)
        {
            return $"{OpenStringChar}{Distillate(content)}{CloseStringChar}";
        }

        private static string WriteQuote(string content)
        {
            return $"{OpenQuoteChar}{Distillate(content)}{CloseQuoteChar}";
        }

        private static string WriteSymbols(ImmutableList<Symbol> symbols)
        {
            return $"{OpenSymbolsChar}{string.Join(" ", symbols.Select(WriteSymbol))}{CloseSymbolsChar}";
        }

        public static string WriteSymbol(Symbol symbol)
        {
            return symbol.Match(
                atom => WriteAtom(atom),
                number => WriteNumber(number),
                str => WriteString(str),
                quote => WriteQuote(quote),
                symbols => WriteSymbols(symbols));
        }

        public static string UnparseSymbol(Symbol symbol)
        {
            return WriteSymbol(symbol);
        }
    }

    [Serializable]
    public class ConversionException : Exception
    {
        public ConversionException() { }
        public ConversionException(string message) : base(message) { }
        public ConversionException(string message, Exception inner) : base(message, inner) { }
        protected ConversionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}

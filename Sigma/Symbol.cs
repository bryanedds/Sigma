using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sprache;

namespace Sigma
{
    /// <summary>
    /// Union tag for Symbol type.
    /// </summary>
    public enum SymbolTag
    {
        [Union(typeof(ImmutableList<Symbol>))] Symbols = 0,
        [Union(typeof(string))] Quote,
        [Union(typeof(string))] Atom
    }

    /// <summary>
    /// A generalized representation of all possible values.
    /// </summary>
    public class Symbol : Union<SymbolTag, object>, IEquatable<Symbol>
    {
        public Symbol(IEnumerable<Symbol> symbols) : base(SymbolTag.Symbols, symbols.ToImmutableList()) { }
        public Symbol(Tuple<string> quote) : base(SymbolTag.Quote, quote.Item1) { }
        public Symbol(string atom) : base(SymbolTag.Atom, atom) { }
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
                case SymbolTag.Symbols: return AsSymbols.SequenceEqual(other.AsSymbols);
                case SymbolTag.Quote: return AsQuote == other.AsQuote;
                case SymbolTag.Atom: return AsAtom == other.AsAtom;
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
                case SymbolTag.Symbols: return AsSymbols.Aggregate(0, (hash, symbol) => hash ^ symbol.GetHashCode());
                case SymbolTag.Quote: return AsQuote.GetHashCode();
                case SymbolTag.Atom: return AsAtom.GetHashCode();
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
    }

    /// <summary>
    /// Parses and unparses Symbols from / to strings. Example strings include:
    /// 
    /// /* Atom values */
    /// 
    /// 0
    /// None
    /// Hello_World
    /// CharacterAnimationFacing
    /// "String with quoted spaces."
    /// String_with_underscores_for_spaces.
    /// 
    /// /* Quote values */
    /// 
    /// `True'
    /// `[Some 1.0]'
    /// 
    /// /* Symbols values */
    ///
    /// []
    /// [Some 0]
    /// [Left 0]
    /// [[0 1] [2 4]]
    /// [AnimationData 4 8]
    /// [Gem `[Some 12]']
    /// 
    /// NOTE: Quotes currently do not recurse.
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

        public static Parser<Symbol> ReadAtomAsString =
            from _ in Parse.Char(OpenStringChar)
            from _2 in ReadWhitespaces
            from chars in ReadStringChars
            from _3 in Parse.Char(CloseStringChar)
            from _4 in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadQuote =
            from _ in Parse.Char(OpenQuoteChar)
            from _2 in ReadWhitespaces
            from chars in ReadQuoteChars
            from _3 in Parse.Char(CloseQuoteChar)
            from _4 in ReadWhitespaces
            select new Symbol(Tuple.Create(chars));

        public static Parser<Symbol> ReadSymbols =
            from _ in Parse.Char(OpenSymbolsChar)
            from _2 in ReadWhitespaces
            from symbols in ReadSymbol.Many()
            from _3 in Parse.Char(CloseSymbolsChar)
            from _4 in ReadWhitespaces
            select new Symbol(symbols.ToImmutableList());

        public static Parser<Symbol> ReadSymbol =
            ReadAtomAsString.Or(
            ReadQuote.Or(
            ReadAtom.Or(
            ReadSymbols)));

        public static Symbol ParseSymbol(string sexpr)
        {
            return
                (from _ in ReadWhitespaces
                 from symbol in ReadSymbol
                 select symbol)
                .Parse(sexpr);
        }

        public static bool IsExplicit(string content)
        {
            return content.StartsWith(OpenStringChar.ToString()) && content.EndsWith(CloseStringChar.ToString());
        }

        public static bool ShouldBeExplicit(string content)
        {
            return content.Any(chr => char.IsWhiteSpace(chr) || StructureCharsNoStr.Contains(chr));
        }

        private static string WriteAtom(string content)
        {
            if (content.IsEmpty()) return $"{OpenStringChar}{CloseStringChar}";
            if (!IsExplicit(content) && ShouldBeExplicit(content)) return $"{OpenStringChar}{content}{CloseStringChar}";
            if (IsExplicit(content) && !ShouldBeExplicit(content)) return content.Substring(1, content.Length - 2);
            return content;
        }

        private static string WriteQuote(string content)
        {
            return $"{OpenQuoteChar}{content}{CloseQuoteChar}";
        }

        private static string WriteSymbols(ImmutableList<Symbol> symbols)
        {
            return $"{OpenSymbolsChar}{string.Join(" ", symbols.Select(WriteSymbol))}{CloseSymbolsChar}";
        }

        public static string WriteSymbol(Symbol symbol)
        {
            switch (symbol.Tag)
            {
                case SymbolTag.Symbols: return WriteSymbols((ImmutableList<Symbol>)symbol.Data);
                case SymbolTag.Quote: return WriteQuote((string)symbol.Data);
                case SymbolTag.Atom: return WriteAtom((string)symbol.Data);
                default: throw new InexhausiveException();
            }
        }

        public static string UnparseSymbol(Symbol symbol)
        {
            return WriteSymbol(symbol);
        }
    }
}

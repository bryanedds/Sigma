using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.Serialization;
using Sprache;

namespace Sigma
{


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
			select Symbol.Atom(chars);

		public static Parser<Symbol> ReadNumber =
			from chars in Parse.Number
			from _ in ReadWhitespaces
			select Symbol.Number(chars);

		public static Parser<Symbol> ReadString =
			from _ in Parse.Char(OpenStringChar)
			from _2 in ReadWhitespaces
			from chars in ReadStringChars
			from _3 in Parse.Char(CloseStringChar)
			from _4 in ReadWhitespaces
			select Symbol.Text(chars);

		public static Parser<Symbol> ReadQuote =
			from _ in Parse.Char(OpenQuoteChar)
			from _2 in ReadWhitespaces
			from chars in ReadQuoteChars
			from _3 in Parse.Char(CloseQuoteChar)
			from _4 in ReadWhitespaces
			select Symbol.Quote(chars);

		public static Parser<Symbol> ReadSymbols =
			from _ in Parse.Char(OpenSymbolsChar)
			from _2 in ReadWhitespaces
			from symbols in ReadSymbol.Many()
			from _3 in Parse.Char(CloseSymbolsChar)
			from _4 in ReadWhitespaces
			select Symbol.Symbols(symbols.ToImmutableList());

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

		public static bool IsNumber(string content)
		{
			return
				(from chars in Parse.Number
				 select chars)
				 .TryParse(content)
				 .WasSuccessful;
		}

		public static bool IsExplicit(string content)
		{
			return content.StartsWith(OpenStringChar.ToString()) && content.EndsWith(CloseStringChar.ToString());
		}

		public static bool ShouldBeExplicit(string content)
		{
			return content.Any(chr => char.IsWhiteSpace(chr) || StructureCharsNoStr.Contains(chr));
		}

		public static string Implicitize(string str)
		{
			var distilled = Distillate(str);
			return distilled.Substring(1, distilled.Length - 2);
		}

		public static string Explicitize(string str)
		{
			var distilled = Distillate(str);
			return $"{OpenStringChar}{distilled}{CloseStringChar}";
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
			return symbol.Match(WriteAtom, WriteNumber, WriteString, WriteQuote, WriteSymbols);
		}

		public static string UnparseSymbol(Symbol symbol)
		{
			return WriteSymbol(symbol);
		}
	}

	public class Symbol : Union<string, string, string, string, ImmutableList<Symbol>>
	{
		public static Symbol Atom(string atom) => new Symbol(atom, Overload1.Value);
		public static Symbol Number(string number) => new Symbol(number, Overload2.Value);
		public static Symbol Text(string text) => new Symbol(text, Overload3.Value);
		public static Symbol Quote(string quote) => new Symbol(quote, Overload4.Value);
		public static Symbol Symbols(IEnumerable<Symbol> symbols) => new Symbol(symbols, Overload5.Value);
		public bool IsAtom => IsT1;
		public bool IsNumber => IsT2;
		public bool IsText => IsT3;
		public bool IsQuote => IsT4;
		public bool IsSymbols => IsT5;
		public string ToAtom => ToT1;
		public string ToNumber => ToT2;
		public string ToText => ToT3;
		public string ToQuote => ToT4;
		public ImmutableList<Symbol> ToSymbols => ToT5;
		public string TryAtom => TryT1.ToReference();
		public string TryNumber => TryT2.ToReference();
		public string TryText => TryT3.ToReference();
		public string TryQuote => TryT4.ToReference();
		public ImmutableList<Symbol> TrySymbols => TryT5.ToReference();
		public Symbol(string atom, Overload1 o = default) : base(atom, o) { }
		public Symbol(string number, Overload2 o = default) : base(number, o) { }
		public Symbol(string text, Overload3 o = default) : base(text, o) { }
		public Symbol(string quote, Overload4 o = default) : base(quote, o) { }
		public Symbol(IEnumerable<Symbol> symbols, Overload5 o = default) : base(symbols.ToImmutableList(), o) { }
		public static Symbol FromString(string str) => SymbolParser.ParseSymbol(str);
		public string ToString(Symbol symbol) => SymbolParser.WriteSymbol(symbol);
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

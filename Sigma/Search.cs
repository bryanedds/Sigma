using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Sprache;

namespace Sigma
{
    public static class Search
    {
        public static Symbol Parse(string searchString)
        {
            return SearchParser.ParseSearchString(searchString);
        }

        public static bool Run(string str, Symbol search)
        {
            return Run(string.Empty, str, search);
        }

        public static bool Run(string key, string str, Symbol search)
        {
            var entries = new List<KeyValuePair<string, string>>() { KeyValuePair.Create(key, str) };
            return Run(entries, search);
        }

        public static bool Run(List<KeyValuePair<string, string>> strings, Symbol search)
        {
            switch (search.Tag)
            {
                case SymbolTag.Symbols: return Run(strings, search.AsSymbols);
                case SymbolTag.Quote: throw new InexhausiveException(); // quotes aren't used
                case SymbolTag.Atom: return strings.Any(entry => entry.Value.Search(search.AsAtom));
                default: throw new InexhausiveException();
            }
        }

        private static bool Run(List<KeyValuePair<string, string>> entries, ImmutableList<Symbol> searches)
        {
            // ensure we have an op and at least one operand
            if (searches.Count >= 2)
            {
                var op = searches[0].AsAtom;
                var operands = searches.Skip(1);
                switch (op)
                {
                    case "KeyValue":
                        {
                            var key = operands.ElementAt(0).AsAtom;
                            var value = operands.ElementAt(1).AsAtom;
                            return entries
                                .Where(entry => entry.Key.Search(key))
                                .Where(entry => entry.Value.Search(value))
                                .Any();
                        }
                    case "Not": return operands.None(search => Run(entries, search));
                    case "And": return operands.All(search => Run(entries, search));
                    case "Or": return operands.Any(search => Run(entries, search));
                    default: throw new InexhausiveException();
                }
            }
            throw new InexhausiveException();
        }
    }

    public static class SearchParser
    {
        public static char SeparatorChar = ':';
        public static char OpenStringChar = '\"';
        public static char CloseStringChar = '\"';
        public static char OpenQuoteChar = '`';
        public static char CloseQuoteChar = '\'';
        public static string NewlineChars = "\n\r";
        public static string WhitespaceChars = " \t" + NewlineChars;
        public static string StructureCharsNoStr = ":";
        public static string StructureChars = "\"" + StructureCharsNoStr;

        public static Parser<string> ReadWhitespaces =
            Parse.Chars(WhitespaceChars).Many().Text();

        public static Parser<string> ReadIdentifierChars =
            Parse.CharExcept(StructureChars + WhitespaceChars).AtLeastOnce().Text();

        public static Parser<string> ReadStringChars =
            Parse.CharExcept(CloseStringChar).Many().Text();

        public static Parser<string> ReadQuoteChars =
            Parse.CharExcept(CloseQuoteChar).Many().Text();

        public static Parser<Symbol> ReadIdentifier =
            from chars in ReadIdentifierChars
            from _ in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadIdentifierQuoted =
            from _ in Parse.Char(OpenStringChar)
            from _2 in ReadWhitespaces
            from chars in ReadStringChars
            from _3 in Parse.Char(CloseStringChar)
            from _4 in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadTerm =
            ReadIdentifier
            .Or(ReadIdentifierQuoted)
            .Where(term => term.AsAtom.IndexOf("Or", 0, StringComparison.CurrentCultureIgnoreCase) == -1);

        public static Parser<Symbol> ReadKeyValue =
            from key in ReadTerm
            from _ in ReadWhitespaces
            from _2 in Parse.Char(SeparatorChar)
            from _3 in ReadWhitespaces
            from value in ReadTerm
            from _4 in ReadWhitespaces
            select new Symbol(new List<Symbol>() { new Symbol("KeyValue"), key, value });

        public static Parser<Symbol> ReadNot =
            from _ in ReadIdentifier.Where(symbol => symbol.AsAtom.IndexOf("Not", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
            from _2 in ReadWhitespaces
            from search in ReadSearch
            select new Symbol(new List<Symbol>() { new Symbol("Not"), search });

        public static Parser<Symbol> ReadOr =
            from _ in ReadIdentifier.Where(symbol => symbol.AsAtom.IndexOf("Or", 0, StringComparison.CurrentCultureIgnoreCase) != -1)
            from _2 in ReadWhitespaces
            select new Symbol("Or");

        public static Parser<Symbol> ReadAnd =
            from term in ReadKeyValue.Or(ReadNot).Or(ReadTerm)
            from _ in ReadWhitespaces
            from search in ReadSearch
            select new Symbol(new List<Symbol>() { new Symbol("And"), term, search });

        public static Parser<Symbol> ReadSearch =
            ReadAnd
            .Or(ReadKeyValue
            .Or(ReadNot
            .Or(ReadTerm)));

        public static Parser<Symbol> ReadSearches =
            from searches in Parse.ChainOperator(
                ReadOr,
                ReadSearch,
                (_, left, right) => new Symbol(new List<Symbol>() { new Symbol("Or"), left, right }))
            from _ in Parse.AnyChar.Many().Text() // always parse, discarding unintelligible portion
            select searches;

        public static Symbol ParseSearchString(string searchString)
        {
            // return empty symbol if search string is empty
            if (searchString.None()) return new Symbol(string.Empty);

            // parse of non-empty string always succeeds
            return
                  (from _ in ReadWhitespaces
                   from terms in ReadSearches
                   select terms)
                  .Parse(searchString);
        }
    }
}

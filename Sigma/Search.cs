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
            return search.Match(
                atom => strings.Any(entry => entry.Value.Search(atom)),
                number => Expression<bool>.Throw<InexhausiveException>(), // numbers aren't used
                str => strings.Any(entry => entry.Value.Search(str)),
                quote => Expression<bool>.Throw<InexhausiveException>(), // quotes aren't used
                symbols => Run(strings, symbols));
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
        public static string NewlineChars = "\n\r";
        public static string WhitespaceChars = " \t" + NewlineChars;
        public static string StructureCharsNoStr = ":";
        public static string StructureChars = "\"" + StructureCharsNoStr;

        public static Parser<string> ReadWhitespaces =
            Parse.Chars(WhitespaceChars).Many().Text();

        public static Parser<string> ReadIdentifierChars =
            Parse.CharExcept(StructureChars + WhitespaceChars).AtLeastOnce().Text();

        public static Parser<string> ReadLiteralChars =
            Parse.CharExcept(CloseStringChar).Many().Text();

        public static Parser<Symbol> ReadIdentifier =
            from chars in ReadIdentifierChars
            from _ in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadLiteral =
            from _ in Parse.Char(OpenStringChar)
            from _2 in ReadWhitespaces
            from chars in ReadLiteralChars
            from _3 in Parse.Char(CloseStringChar)
            from _4 in ReadWhitespaces
            select new Symbol(chars);

        public static Parser<Symbol> ReadTerm =
            ReadLiteral
            .Or(ReadIdentifier)
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

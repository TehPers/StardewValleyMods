using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;
using StardewValley.Menus;
using TehPers.CoreMod.Api.ContentPacks.Tokens;
using TehPers.CoreMod.Api.Extensions;

namespace TehPers.CoreMod.ContentPacks.Tokens.Parsing {
    internal class TokenParser {
        private readonly TokenRegistry _tokenRegistry;

        public TokenParser(TokenRegistry tokenRegistry) {
            this._tokenRegistry = tokenRegistry;
        }

        public IEnumerable<ITokenizedStringPart> ParseRawValue(string rawInput) {
            Parser<IEnumerable<ITokenizedStringPart>> parser = this.TokenizedString();
            return parser.Parse(rawInput);
        }

        public Parser<IEnumerable<ITokenizedStringPart>> TokenizedString() {
            return from partsWithTokens in TokenParser.LiteralThenTokenNonEndPart().Many().Select(this.CreateParts)
                   from trailingLiteral in TokenParser.LiteralEndPart()
                   let trailingLiteralPart = string.IsNullOrEmpty(trailingLiteral) ? null : new ConstantTokenizedStringPart(trailingLiteral)
                   select partsWithTokens.Concat(trailingLiteralPart?.Yield() ?? Enumerable.Empty<ITokenizedStringPart>());
        }

        public IEnumerable<ITokenizedStringPart> CreateParts(IEnumerable<(string literal, string tokenName, string[] arguments)> parsedParts) {
            foreach ((string literal, string tokenName, string[] arguments) in parsedParts) {
                // Literal part
                if (!string.IsNullOrEmpty(literal)) {
                    yield return new ConstantTokenizedStringPart(literal);
                }

                if (!this._tokenRegistry.TryGetToken(tokenName, out IToken token)) {
                    throw new ParseException($"Unknown token '{tokenName}'");
                }

                yield return new TokenTokenizedStringPart(token, arguments);
            }
        }

        public static Parser<(string literal, string tokenName, string[] arguments)> LiteralThenTokenNonEndPart() {
            return from literal in TokenParser.LiteralNonEndPart()
                   from token in TokenParser.Token()
                   select (literal, token.name, token.arguments);
        }

        public static Parser<string> LiteralNonEndPart() {
            return TokenParser.EscapedOpenToken()
                .Or(TokenParser.EscapedCloseToken())
                .Or(from brace in Parse.Char('{')
                    from nextChar in Parse.CharExcept('{')
                    select $"{brace}{nextChar}")
                .Or(Parse.String("{").End())
                .Or(Parse.CharExcept('{').Select(c => c.ToString()))
                .Many()
                .Select(parts => parts.Aggregate(new StringBuilder(), (builder, part) => builder.Append(part)).ToString());
        }

        public static Parser<string> LiteralEndPart() {
            Parser<string> anyEscape = TokenParser.EscapedOpenToken().Or(TokenParser.EscapedCloseToken());
            return anyEscape
                .Or(Parse.AnyChar.Select(c => c.ToString()))
                .Many()
                .Select(parts => parts.Aggregate(new StringBuilder(), (builder, part) => builder.Append(part)).ToString());
        }

        public static Parser<T> Fail<T>(string reason, IEnumerable<string> expectations) {
            return input => Result.Failure<T>(input, reason, expectations);
        }

        public static Parser<(string name, string[] arguments)> Token() {
            return TokenParser.TokenWithArgContents().Or(TokenParser.SimpleTokenContents()).Contained(TokenParser.OpenToken(), TokenParser.CloseToken());
        }

        public static Parser<string> OpenToken() {
            return Parse.String("{{").Text();
        }

        public static Parser<string> CloseToken() {
            return Parse.String("}}").Text();
        }

        public static Parser<string> EscapedOpenToken() {
            return TokenParser.OpenToken().Then(escape => TokenParser.OpenToken());
        }

        public static Parser<string> EscapedCloseToken() {
            return TokenParser.CloseToken().Then(escape => TokenParser.CloseToken());
        }

        public static Parser<string> Name() {
            return from firstChar in Parse.Letter
                   from rest in Parse.LetterOrDigit.Many()
                   select new string(rest.Prepend(firstChar).ToArray());
        }

        public static Parser<string> SimpleArgument() {
            return Parse.CharExcept(",}").Many().Text();
        }

        public static Parser<string> QuotedArgument() {
            return from openQuote in TokenParser.OpenQuote()
                   from contents in TokenParser.EscapedOpenQuote().Or(TokenParser.EscapedCloseQuote()).Or(TokenParser.NonQuoteArgumentCharacter()).Many().Text()
                   from closeQuote in TokenParser.ClosedQuote()
                   select contents;
        }

        public static Parser<string> Argument() {
            return TokenParser.QuotedArgument().Token().Or(TokenParser.SimpleArgument().Select(t => t.Trim()));
        }

        public static Parser<IEnumerable<string>> ArgumentList() {
            return from first in TokenParser.Argument()
                   from rest in Parse.Char(',').Token().Then(_ => TokenParser.Argument()).Many()
                   select rest.Prepend(first);
        }

        public static Parser<(string name, string[] arguments)> SimpleTokenContents() {
            return TokenParser.Name().Token().Select(name => (name, new string[0]));
        }

        public static Parser<(string name, string[] arguments)> TokenWithArgContents() {
            return from name in TokenParser.Name().Token()
                   from delimiter in Parse.Char(':')
                   from argumentList in TokenParser.ArgumentList()
                   select (name, argumentList.ToArray());
        }

        public static Parser<char> OpenQuote() {
            return Parse.Char('<');
        }

        public static Parser<char> ClosedQuote() {
            return Parse.Char('>');
        }

        public static Parser<char> EscapedOpenQuote() {
            return TokenParser.OpenQuote().Then(escape => TokenParser.OpenQuote());
        }

        public static Parser<char> EscapedCloseQuote() {
            return TokenParser.ClosedQuote().Then(escape => TokenParser.ClosedQuote());
        }

        public static Parser<char> NonQuoteArgumentCharacter() {
            return Parse.AnyChar.Except(TokenParser.OpenQuote().Or(TokenParser.ClosedQuote()));
        }
    }
}

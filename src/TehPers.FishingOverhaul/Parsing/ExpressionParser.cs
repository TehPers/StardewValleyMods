using Superpower;
using Superpower.Display;
using Superpower.Parsers;
using Superpower.Tokenizers;
using System.Diagnostics.CodeAnalysis;

namespace TehPers.FishingOverhaul.Parsing
{
    internal class ExpressionParser
    {
        private static readonly Tokenizer<ExpressionToken> tokenizer =
            new TokenizerBuilder<ExpressionToken>().Ignore(Character.WhiteSpace)
                .Match(Numerics.Decimal, ExpressionToken.Number)
                .Match(Character.Letter.AtLeastOnce(), ExpressionToken.Ident)
                .Match(Character.EqualTo('+'), ExpressionToken.Add)
                .Match(Character.EqualTo('-'), ExpressionToken.Subtract)
                .Match(Character.EqualTo('*'), ExpressionToken.Multiply)
                .Match(Character.EqualTo('/'), ExpressionToken.Divide)
                .Match(Character.EqualTo('%'), ExpressionToken.Modulo)
                .Match(Character.EqualTo('^'), ExpressionToken.Power)
                .Match(Character.EqualTo('('), ExpressionToken.LParen)
                .Match(Character.EqualTo(')'), ExpressionToken.RParen)
                .Build();

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> add =
            Token.EqualTo(ExpressionToken.Add).Value(BinaryOperator.Add);

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> subtract =
            Token.EqualTo(ExpressionToken.Subtract).Value(BinaryOperator.Subtract);

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> multiply =
            Token.EqualTo(ExpressionToken.Multiply).Value(BinaryOperator.Multiply);

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> divide =
            Token.EqualTo(ExpressionToken.Divide).Value(BinaryOperator.Divide);

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> modulo =
            Token.EqualTo(ExpressionToken.Modulo).Value(BinaryOperator.Modulo);

        private static readonly TokenListParser<ExpressionToken, BinaryOperator> power =
            Token.EqualTo(ExpressionToken.Power).Value(BinaryOperator.Power);

        private static readonly TokenListParser<ExpressionToken, UnaryOperator> negate =
            Token.EqualTo(ExpressionToken.Subtract).Value(UnaryOperator.Negate);

        private static readonly TokenListParser<ExpressionToken, Expr<double>> number =
            Token.EqualTo(ExpressionToken.Number)
                .Apply(Numerics.DecimalDouble)
                .Select(n => (Expr<double>)new ConstantExpr<double>(n))
                .Named("number");

        private static readonly TokenListParser<ExpressionToken, Expr<double>> ident =
            Token.EqualTo(ExpressionToken.Ident)
                .Apply(Character.Letter.AtLeastOnce())
                .Select(chars => (Expr<double>)new IdentExpr(new(chars)))
                .Named("identifier");

        private static readonly TokenListParser<ExpressionToken, Expr<double>> group =
            Token.EqualTo(ExpressionToken.LParen)
                .IgnoreThen(Parse.Ref(() => ExpressionParser.expr!))
                .Then(expr => Token.EqualTo(ExpressionToken.RParen).Value(expr));

        private static readonly TokenListParser<ExpressionToken, Expr<double>> factor =
            ExpressionParser.group.Or(ExpressionParser.number).Or(ExpressionParser.ident);

        private static readonly TokenListParser<ExpressionToken, Expr<double>> unaryAssoc =
            (from op in ExpressionParser.negate
                from inner in Parse.Ref(() => ExpressionParser.unaryAssoc!)
                select (Expr<double>)new UnaryExpr(op, inner)).Or(ExpressionParser.factor);

        private static readonly TokenListParser<ExpressionToken, Expr<double>> exponentialAssoc =
            Parse.Chain(
                ExpressionParser.power,
                ExpressionParser.unaryAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        private static readonly TokenListParser<ExpressionToken, Expr<double>> multiplicativeAssoc =
            Parse.Chain(
                ExpressionParser.multiply.Or(ExpressionParser.divide).Or(ExpressionParser.modulo),
                ExpressionParser.exponentialAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        private static readonly TokenListParser<ExpressionToken, Expr<double>> additiveAssoc =
            Parse.Chain(
                ExpressionParser.add.Or(ExpressionParser.subtract),
                ExpressionParser.multiplicativeAssoc,
                (op, l, r) => new BinaryExpr(op, l, r)
            );

        private static readonly TokenListParser<ExpressionToken, Expr<double>> expr =
            ExpressionParser.additiveAssoc;

        public static bool TryParse(
            string source,
            [MaybeNullWhen(false)] out Expr<double> result,
            [MaybeNullWhen(true)] out string error
        )
        {
            // Tokenize
            var tokenized = ExpressionParser.tokenizer.TryTokenize(source);
            if (!tokenized.HasValue)
            {
                result = default;
                error = tokenized.FormatErrorMessageFragment();
                return false;
            }

            // Parse
            var parsed = ExpressionParser.expr.AtEnd().TryParse(tokenized.Value);
            if (!parsed.HasValue)
            {
                result = default;
                error =
                    $"{parsed.FormatErrorMessageFragment()} (at position {parsed.ErrorPosition.Absolute})";
                return false;
            }

            // Optimize and return
            result = parsed.Value.Optimized();
            error = default;
            return true;
        }

        public enum ExpressionToken
        {
            [Token(Example = "+")]
            Add,

            [Token(Example = "-")]
            Subtract,

            [Token(Example = "*")]
            Multiply,

            [Token(Example = "/")]
            Divide,

            [Token(Example = "%")]
            Modulo,

            [Token(Example = "^")]
            Power,

            [Token(Example = "(")]
            LParen,

            [Token(Example = ")")]
            RParen,

            [Token(Description = "identifier", Example = "x")]
            Ident,

            [Token(Description = "number", Example = "24.5")]
            Number,
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class Token
    {
        public enum TokenType
        {
            Identifier,
            Keyword,
            Literal,
            Operator,
            Separator,
            Comment,
        }

        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return $"Type: {Type}, Value: {Value}";
        }
    }
}

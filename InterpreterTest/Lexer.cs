using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class Lexer
    {
        private readonly string _source;
        private int _position;

        private static readonly string[] Keywords = { "BOOL", "CHAR", "FALSE", "FLOAT", "INT", "TRUE" };

        public Lexer(string source)
        {
            _source = source;
            _position = 0;
        }

        public List<Token> Tokenize()
        {
            var tokens = new List<Token>();

            while (_position < _source.Length)
            {
                char currentChar = _source[_position];

                if (char.IsWhiteSpace(currentChar))
                {
                    _position++;
                    continue;
                }

                if (char.IsLetter(currentChar) || currentChar == '_')
                {
                    string identifier = ReadWhile(c => char.IsLetterOrDigit(c) || c == '_');
                    if (Keywords.Contains(identifier))
                    {
                        tokens.Add(new Token(Token.TokenType.Identifier, identifier));
                    }
                    else
                    {
                        tokens.Add(new Token(Token.TokenType.Keyword, identifier));
                    }
                }
                else if (char.IsDigit(currentChar))
                {
                    string literal = ReadWhile(char.IsDigit);
                    tokens.Add(new Token(Token.TokenType.Literal, literal));
                }
                else if (IsOperator(currentChar))
                {
                    string op = ReadWhile(IsOperator);
                    tokens.Add(new Token(Token.TokenType.Operator, op));
                }
                else if (IsSeparator(currentChar))
                {
                    string separator = currentChar.ToString();
                    tokens.Add(new Token(Token.TokenType.Separator, separator));
                    _position++;
                }
                else if (currentChar == '"' || currentChar == '\'')
                {
                    tokens.Add(ReadStringLiteral());
                }
                else if (currentChar == '#')
                {
                    tokens.Add(ReadSingleLineComment());
                }
                else
                {
                    throw new InvalidOperationException($"Unknown token at position {_position}: {currentChar}");
                }
            }
            return tokens;
        }

        private bool IsOperator(char c)
        {
            string operators = "()*/%+-=<>";
            return operators.Contains(c);
        }

        private bool IsSeparator(char c)
        {
            string separators = ";";
            return separators.Contains(c);
        }

        private Token ReadStringLiteral()
        {
            char quote = _source[_position];
            _position++;

            string literal = "";

            while (_position < _source.Length)
            {
                char currentChar = _source[_position];
                if (currentChar == quote)
                {
                    _position++;
                    return new Token(Token.TokenType.Literal, literal);
                }
                literal += currentChar;
                _position++;
            }

            throw new InvalidOperationException("Unterminated string literal");
        }

        private Token ReadSingleLineComment()
        {
            string comment = ReadWhile(c => c != '\n');
            return new Token(Token.TokenType.Comment, comment);
        }

        private string ReadWhile(Func<char, bool> predicate)
        {
            string result = "";
            while (_position < _source.Length && predicate(_source[_position]))
            {
                result += _source[_position];
                _position++;
            }
            return result;
        }
    }
}

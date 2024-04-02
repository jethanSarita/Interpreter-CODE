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

        private static readonly string[] Keywords = { "INT", "CHAR", "FLOAT", "BOOL", "TRUE", "FALSE", "IF", "NOT", "AND", "OR", "DISPLAY", "BEGIN", "END", "CODE" };

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
                //check current char if it's a letter
                if (IsLetterOr_(currentChar))
                {
                    //continue to read the rest of it and store in
                    //data. Stop until space or non letter/digit/_
                    //sample: x, INT, variable_name, num1
                    string data = ReadWhile(IsLetterOrDigitOr_);
                    //_position++;
                    //Check if its reserved word
                    if (Keywords.Contains(data))
                    {
                        switch(data)
                        {
                            case "INT":
                                tokens.Add(new Token(Token.TokenType.INT, data));
                                break;
                            case "CHAR":
                                tokens.Add(new Token(Token.TokenType.CHAR, data));
                                break;
                            case "FLOAT":
                                tokens.Add(new Token(Token.TokenType.FLOAT, data));
                                break;
                            case "BOOL":
                                tokens.Add(new Token(Token.TokenType.BOOL, data));
                                break;
                            case "TRUE":
                                tokens.Add(new Token(Token.TokenType.TRUE, data));
                                break;
                            case "FALSE":
                                tokens.Add(new Token(Token.TokenType.FALSE, data));
                                break;
                            case "IF":
                                tokens.Add(new Token(Token.TokenType.IF, data));
                                break;
                            case "NOT":
                                tokens.Add(new Token(Token.TokenType.NOT, data));
                                break;
                            case "AND":
                                tokens.Add(new Token(Token.TokenType.AND, data));
                                break;
                            case "OR":
                                tokens.Add(new Token(Token.TokenType.OR, data));
                                break;
                            case "DISPLAY":
                                tokens.Add(new Token(Token.TokenType.DISPLAY, data));
                                break;
                            case "BEGIN":
                                tokens.Add(new Token(Token.TokenType.BEGIN, data));
                                break;
                            case "END":
                                tokens.Add(new Token(Token.TokenType.END, data));
                                break;
                            case "CODE":
                                tokens.Add(new Token(Token.TokenType.CODE, data));
                                break;
                        }
                    }
                    else
                    {
                        tokens.Add(new Token(Token.TokenType.IDENTIFIER, data));
                    }
                }
                else if (char.IsDigit(currentChar))
                {
                    string number = ReadWhile(char.IsDigit);
                    tokens.Add(new Token(Token.TokenType.NUMBER, number));
                    //_position++;
                }
                else if (IsBinaryOperator(currentChar))
                {
                    string data = "" + currentChar;
                    //+-/*%
                    switch (currentChar)
                    {
                        case '+':
                            tokens.Add(new Token(Token.TokenType.PLUS, data));
                            break;
                        case '-':
                            tokens.Add(new Token(Token.TokenType.MINUS, data));
                            break;
                        case '/':
                            tokens.Add(new Token(Token.TokenType.SLASH, data));
                            break;
                        case '*':
                            tokens.Add(new Token(Token.TokenType.STAR, data));
                            break;
                        case '%':
                            tokens.Add(new Token(Token.TokenType.MODULO, data));
                            break;
                        case '=':
                            if (Peek() == '=')
                            {
                                data += '=';
                                tokens.Add(new Token(Token.TokenType.EQUAL_EQUAL, data));
                                _position++;
                            }
                            else
                            {
                                tokens.Add(new Token(Token.TokenType.EQUAL, data));
                            }
                            break;
                    }
                    _position++;
                }
                else if (IsSeparator(currentChar))
                {
                    string data = "" + currentChar;
                    switch (currentChar)
                    {
                        //()[],:&$"
                        case '(':
                            tokens.Add(new Token(Token.TokenType.LEFT_PAREN, data));
                            break;
                        case ')':
                            tokens.Add(new Token(Token.TokenType.RIGHT_PAREN, data));
                            break;
                        /*case '[':
                            tokens.Add(new Token(Token.TokenType.LEFT_SQUARE, data));
                            break;
                        case ']':
                            tokens.Add(new Token(Token.TokenType.RIGHT_SQUARE, data));
                            break;*/
                        case ',':
                            tokens.Add(new Token(Token.TokenType.COMMA, data));
                            break;
                        case ':':
                            tokens.Add(new Token(Token.TokenType.COLON, data));
                            break;
                        case '&':
                            tokens.Add(new Token(Token.TokenType.CONCATENATE, data));
                            break;
                        case '$':
                            tokens.Add(new Token(Token.TokenType.NEXT_LINE, data));
                            break;
                    }
                    _position++;
                }
                else if (IsLogicOperator(currentChar))
                {
                    string data = "" + currentChar;
                    switch(currentChar)
                    {
                        //<,>,<=,>=,<>
                        case '<':
                            if (Peek() == '=')
                            {
                                data += '=';
                                tokens.Add(new Token(Token.TokenType.LESS_EQUAL, data));
                                _position++;
                            }
                            else if (Peek() == '>')
                            {
                                data += '>';
                                tokens.Add(new Token(Token.TokenType.NOT_EQUAL, data));
                                _position++;
                            }
                            else
                            {
                                tokens.Add(new Token(Token.TokenType.LESS, data));
                            }
                            break;
                        case '>':
                            if (Peek() == '=')
                            {
                                data += '=';
                                tokens.Add(new Token(Token.TokenType.GREATER_EQUAL, data));
                                _position++;
                            }
                            else
                            {
                                tokens.Add(new Token(Token.TokenType.GREATER, data));
                            }
                            break;
                    }
                    _position++;
                }
                else if (currentChar == '"')
                {
                    tokens.Add(ReadStringLiteral());
                    //_position++;
                }
                else if (currentChar == '\'' || currentChar == '[' || currentChar == ']')
                {
                    tokens.Add(ReadCharLiteral());
                    //_position++;
                }
                else if (currentChar == '#')
                {
                    tokens.Add(ReadSingleLineComment());
                    //_position++;
                }
                else
                {
                    throw new InvalidOperationException($"Unknown token at position {_position}: {currentChar}");
                }
            }
            return tokens;
        }

        private char Peek()
        {
            int peekPos = _position + 1;
            if (peekPos < _source.Length)
            {
                return _source[_position + 1];
            }
            return '\0';
        }

        private bool IsLogicOperator(char c)
        {
            string logicOperators = "><=";
            return logicOperators.Contains(c);
        }

        private bool IsBinaryOperator(char c)
        {
            string operators = "+-/*%=";
            return operators.Contains(c);
        }

        private bool IsSeparator(char c)
        {
            string operators = "(),:&$";
            return operators.Contains(c);
        }

        private bool IsLetterOr_(char c)
        {
            return char.IsLetter(c) || c == '_';
        }

        private bool IsLetterOrDigitOr_(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
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
                    return new Token(Token.TokenType.STRING, literal);
                }
                literal += currentChar;
                _position++;
            }

            throw new InvalidOperationException("Unterminated string literal");
        }

        private Token ReadCharLiteral()
        {
            char quote = _source[_position];
            _position++;

            if (quote == '[')
            {
                quote = ']';
            }
            else if (quote == ']')
            {
                throw new InvalidOperationException($"Wrong escape code open at line {_position}: {quote}");
            }

            string letter = "" + _source[_position];
            
            if (Peek() == quote)
            {
                _position++;
                _position++;
                return new Token(Token.TokenType.LETTER, letter);
            }
            else
            {
                throw new InvalidOperationException($"Char literal error {Peek()} != {quote}");
            }
        }

        private Token ReadSingleLineComment()
        {
            string comment = ReadWhile(c => c != '\n');
            string srubbed = comment.Remove(comment.Length - 1);
            return new Token(Token.TokenType.COMMENT, srubbed);
        }


        //This is basically a custom While Loop
        //It takes a function calling it "predicate"
        //Func<char, bool> is the type of function it takes
        //where char is the parameter
        //and bool is the return type
        //sample: private bool funcName(char c){}
        private string ReadWhile(Func<char, bool> predicate)
        {
            string result = "";
            //here we can see predicate used to check if the current char of
            //the sent code follows the conditions of the function that's sent
            while (_position < _source.Length && predicate(_source[_position]))
            {
                result += _source[_position];
                _position++;
            }
            return result;
        }
    }
}

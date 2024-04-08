using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.XPath;
using static InterpreterTest.Token;

namespace InterpreterTest
{
    internal class Parser
    {
        private readonly List<Token> _tokens;
        private int _position;

        private readonly string[] _keywords = {
            "INT", "BOOL", "CHAR", "FLOAT", "FALSE", "TRUE", "BEGIN", "END",
            "DISPLAY", "SCAN", "IF", "ELSE", "WHILE", "AND", "OR", "NOT"
        };

        private bool _insideCodeBlock = false;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        public ProgramNode Parse()
        {
            Token currentToken = _tokens[_position];
            if (!(currentToken.Type == TokenType.BEGIN && Peek().Type == TokenType.CODE))
            {
                throw new Exception("Expected 'BEGIN CODE'");
            }

            _position++;
            _position++;
            _insideCodeBlock = true;

            var statements = new List<ASTNode>();
            while (_position < _tokens.Count)
            {
                currentToken = _tokens[_position];
                if (currentToken.Type == TokenType.END && Peek().Type == TokenType.CODE)
                {
                    _insideCodeBlock = false;
                    break;
                }

                //[DataType][Identifier] + ([Comma][Identifier])* + [Equals][Literal]
                if (
                    currentToken.Type == TokenType.INT ||
                    currentToken.Type == TokenType.CHAR ||
                    currentToken.Type == TokenType.FLOAT ||
                    currentToken.Type == TokenType.BOOL
                   )
                {
                    if (Peek().Type == TokenType.IDENTIFIER)
                    {
                        _position++;
                        List<Token> identifiers = ReadIdentifiers();
                        foreach (var toks in identifiers)
                        {
                            statements.Add(ParseVariableDeclaration(currentToken, toks));
                        }
                        //Console.WriteLine("Current Token: " + _tokens[_position]);
                        _position++;
                        currentToken = _tokens[_position];
                        if (currentToken.Type == TokenType.EQUAL)
                        {
                            if (!(Peek().Type == TokenType.NUMBER ||
                                Peek().Type == TokenType.LETTER ||
                                Peek().Type == TokenType.TRUE ||
                                Peek().Type == TokenType.FALSE ||
                                Peek().Type == TokenType.DECIMAL_NUMBER))
                            {
                                throw new InvalidOperationException($"No literal assignment after '='");
                            }
                            else
                            {
                                foreach (var toks in identifiers)
                                {
                                    statements.Add(ParseVariableAssignment(toks, Peek()));
                                }
                                _position++;
                            }

                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid {currentToken.Value} declaration");
                    }
                }
                //[Identifier][Equals][Literal]
                else if (currentToken.Type == TokenType.IDENTIFIER)
                {
                    string idenitiferName = currentToken.Value;
                    Token IdentifierToken = currentToken;
                    if (Peek().Type == TokenType.EQUAL)
                    {
                        _position++;
                        if (Peek().Type == TokenType.NUMBER ||
                            Peek().Type == TokenType.LETTER ||
                            Peek().Type == TokenType.TRUE ||
                            Peek().Type == TokenType.FALSE ||
                            Peek().Type == TokenType.DECIMAL_NUMBER)
                        {
                            statements.Add(ParseVariableAssignment(IdentifierToken, Peek()));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Invalid {idenitiferName}(Identifier) assignment. No succeeding literal after equals sign");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid {idenitiferName}(Identifier) assignment, no succeeding equals sign");
                    }
                }
                //[Display][Colon][{contents}]
                else if (currentToken.Type == TokenType.DISPLAY)
                {
                    statements.Add(ParseDisplayStatement());
                }
                else if (currentToken.Type == TokenType.SCAN)
                {
                    statements.Add(ParseScanStatement());
                }
                _position++;
            }
            return new ProgramNode(statements);
        }

        private Token Peek()
        {
            return _tokens[_position + 1];
        }

        private List<Token> ReadIdentifiers()
        {
            List<Token> toks = new List<Token>();
            Token currTok;
            bool commaCheck = false;
            while (true)
            {
                currTok = _tokens[_position];
                //Console.WriteLine("Currently reading: " + currTok.Value);
                if (currTok.Type == TokenType.IDENTIFIER)
                {
                    commaCheck = true;
                    //Console.WriteLine("Currently storing: " + currTok.Value);
                    toks.Add(currTok);
                }
                else if (currTok.Type == TokenType.COMMA && commaCheck)
                {
                    commaCheck = false;
                    if (!(Peek().Type == TokenType.IDENTIFIER))
                    {
                        throw new InvalidOperationException("Invalid comma after variable name with no following variable declaration");
                    }
                }
                else
                {
                    _position--;
                    break;
                }
                _position++;
            }
            return toks;

        }
        private ASTNode ParseVariableDeclaration(Token dataTypeToken, Token varNameToken)
        {
            return new VariableDeclarationNode(dataTypeToken.Value, varNameToken.Value);
        }

        private ASTNode ParseDisplayStatement()
        {
            _position++;

            //after DISPLAY there should be a colon ':'
            if (_tokens[_position].Type != TokenType.COLON)
            {
                throw new InvalidOperationException("Expected ':' after DISPLAY statement");
            }
            _position++;

            //parse display items until end of the line
            DisplayNode displayNode = ParseDisplayItem();


            _position++;
            return displayNode;
        }

        private DisplayNode ParseDisplayItem()
        {
            Token currToken = _tokens[_position];
            DisplayNode result = new DisplayVariableNode("a");
            //handle different types of display items
            while (currToken.Type != TokenType.LINE_SEPARATOR)
            {
                if (currToken.Type == TokenType.IDENTIFIER)
                {
                    result = new DisplayVariableNode(currToken.Value);
                }
                _position++;
                currToken = _tokens[_position];
            }
            return result;
        }

        private ASTNode ParseScanStatement()
        {
            _position++;

            //there should be colon after SCAN
            if (_tokens[_position].Type != TokenType.COLON)
            {
                throw new InvalidOperationException("Expected ':' after SCAN statement");
            }
            _position++;

            List<ASTNode> scans = new List<ASTNode>();
            while (_tokens[_position].Type != TokenType.LINE_SEPARATOR)
            {
                Token currToken = _tokens[_position];

                switch (currToken.Type)
                {
                    //case TokenType.STRING:
                    //    scans.Add(new StringLiteralNode(currToken.Value));
                    //    break;
                    //
                    //case TokenType.NUMBER:
                    //    scans.Add(new NumberLiteralNode(currToken.Value));
                    //    break;
                    case TokenType.IDENTIFIER:
                        scans.Add(new ScannedIdentifierNode(currToken.Value));
                        break;

                    //should I add for bool    

                    default:
                        throw new InvalidOperationException("Invalid token in SCAN statement");
                }

                _position++;

                //if there is comma then there is another pa
                if (_tokens[_position].Type == TokenType.COMMA)
                {
                    _position++;
                }
            }
            _position++;
            return new ScanStatementNode(scans);
        }

        private ASTNode ParseVariableAssignment(Token variableName, Token literal)
        {
            return new VariableAssignmentNode(variableName.Value, literal.Value, literal.Type.ToString());
        }


    }
}
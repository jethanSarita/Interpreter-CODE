using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
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
        private int _lineCounter;

        private readonly string[] _keywords = {
            "INT", "BOOL", "CHAR", "FLOAT", "FALSE", "TRUE", "BEGIN", "END",
            "DISPLAY", "SCAN", "IF", "ELSE", "WHILE", "AND", "OR", "NOT"
        };

        private bool _insideCodeBlock = false;

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
            _lineCounter = 1;
        }

        public ProgramNode Parse()
        {
            Token currentToken = _tokens[_position];
            if (!(Peek(1) != null && currentToken.Type == TokenType.BEGIN && Peek(1).Type == TokenType.CODE))
            {
                throw new Exception($"Error at line {_lineCounter}: Expected 'BEGIN CODE'");
            }

            _position++;
            _position++;
            _insideCodeBlock = true;

            var statements = new List<ASTNode>();
            while (_position < _tokens.Count)
            {
                currentToken = _tokens[_position];
                if (Peek(1) != null && currentToken.Type == TokenType.END && Peek(1).Type == TokenType.CODE)
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
                    if (Peek(1) != null && Peek(1).Type == TokenType.IDENTIFIER)
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
                            if (!(Peek(1) != null &&
                                Peek(1).Type == TokenType.NUMBER ||
                                Peek(1).Type == TokenType.LETTER ||
                                Peek(1).Type == TokenType.TRUE ||
                                Peek(1).Type == TokenType.FALSE ||
                                Peek(1).Type == TokenType.DECIMAL_NUMBER))
                            {
                                throw new InvalidOperationException($"Error at line {_lineCounter}: {UnPeek(1).Value} = {Peek(1).Value} is not a valid assignment");
                            }
                            else
                            {
                                foreach (var toks in identifiers)
                                {
                                    statements.Add(ParseVariableAssignment(toks, Peek(1)));
                                }
                                _position++;
                            }

                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid {currentToken.Value} declaration");
                    }
                }
                //[Identifier][Equals][Literal]
                else if (currentToken.Type == TokenType.IDENTIFIER)
                {
                    string idenitiferName = currentToken.Value;
                    Token IdentifierToken = currentToken;
                    if (Peek(1) != null && Peek(1).Type == TokenType.EQUAL)
                    {
                        _position++;
                        if (Peek(1) != null && 
                            Peek(1).Type == TokenType.NUMBER ||
                            Peek(1).Type == TokenType.LETTER ||
                            Peek(1).Type == TokenType.TRUE ||
                            Peek(1).Type == TokenType.FALSE ||
                            Peek(1).Type == TokenType.DECIMAL_NUMBER)
                        {
                            statements.Add(ParseVariableAssignment(IdentifierToken, Peek(1)));
                        }
                        else
                        {
                            throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid {idenitiferName}(Identifier) assignment. No succeeding literal after equals sign");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error at line  {_lineCounter} : Invalid {idenitiferName}(Identifier) assignment, no succeeding equals sign");
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
                else if (currentToken.Type == TokenType.LINE_SEPARATOR)
                {
                    _lineCounter++;
                }
                _position++;
            }            
            if(_insideCodeBlock)
            {
                throw new Exception($"Error at line {_lineCounter}: Expected 'END CODE'");
            }
            return new ProgramNode(statements);
        }

        private Token Peek(int numOfJumps)
        {
            int total = _position + numOfJumps;
            if (total < _tokens.Count)
            {
                return _tokens[_position + numOfJumps];
            }
            else
            {
                return null;
            }
        }

        private Token UnPeek(int numOfBackFlips)
        {
            int total = _position - numOfBackFlips;
            if (total < _tokens.Count && total >= 0)
            {
                return _tokens[_position - numOfBackFlips];
            }
            else
            {
                return null;
            }
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
                    if (!(Peek(1) != null && Peek(1).Type == TokenType.IDENTIFIER))
                    {
                        throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid comma after variable name with no following variable declaration");
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
                throw new InvalidOperationException($"Error at line {_lineCounter}: Expected ':' after DISPLAY statement");
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
            bool concatLock = true;
            DisplayNode result = new DisplayVariableNode("null");
            //handle different types of display items
            while (_position < _tokens.Count && currToken.Type != TokenType.LINE_SEPARATOR)
            {
                if (CheckIfDisplayable(currToken) && concatLock)
                {
                    if (Peek(1) != null && Peek(1).Type == TokenType.CONCATENATE)
                    {
                        if (Peek(2) != null && CheckIfDisplayable(Peek(2)))
                        {
                            result = new DisplayConcatNode(PraseDisplayable(currToken), PraseDisplayable(Peek(2)));
                            _position++;
                            _position++;
                            concatLock = false;
                        }
                        else
                        {
                            throw new InvalidOperationException($"Error at line {_lineCounter}: Expected data after concatenation at Display");
                        }
                    }
                    else
                    {
                        result = new DisplayVariableNode(currToken.Value);
                    }
                }
                else if (currToken.Type == TokenType.CONCATENATE && !concatLock)
                {
                    if (Peek(1) != null && CheckIfDisplayable(Peek(1)))
                    {
                        result = new DisplayConcatNode(result, PraseDisplayable(Peek(1)));
                        _position++;
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error at line {_lineCounter}: {Peek(1).Value} in display is invalid");
                    }
                }
                else
                {
                    throw new InvalidOperationException($"Error at line {_lineCounter}: {currToken.Value} in display is invalid");
                }
                _position++;
              
                if (_position < _tokens.Count)
                {
                    currToken = _tokens[_position];
                }
            }
            Console.WriteLine(result);
            return result;
        }

        private DisplayNode PraseDisplayable(Token token)
        {
            if (token.Type == TokenType.IDENTIFIER)
            {
                return new DisplayVariableNode(token.Value);
            }
            else if (token.Type == TokenType.NEXT_LINE)
            {
                return new DisplayStringNode(Environment.NewLine);
            }
            else if (token.Type == TokenType.LETTER)
            {
                return new DisplayStringNode(token.Value);
            }
            else if (token.Type == TokenType.STRING)
            {
                return new DisplayStringNode(token.Value);
            }
            //add more NEXT_LINE, LETTER, and STRING
            return null;
        }

        private bool CheckIfDisplayable(Token token)
        {
            if (token.Type == TokenType.IDENTIFIER ||
                token.Type == TokenType.NEXT_LINE ||
                token.Type == TokenType.LETTER ||
                token.Type == TokenType.STRING)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private ASTNode ParseScanStatement()
        {
            _position++;

            //there should be colon after SCAN
            if (_tokens[_position].Type != TokenType.COLON)
            {
                throw new InvalidOperationException($"Error at line {_lineCounter}: Expected ':' after SCAN statement");
            }
            _position++;

            List<ASTNode> scans = new List<ASTNode>();
            List<string> varNames = new List<string>();

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
                        varNames.Add(currToken.Value);
                        break;

                    //skip comma, then continue parsing still
                    case TokenType.COMMA:
                        _position++;
                        continue; 

                    default:
                        throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid token in SCAN statement");
                }
                _position++;
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
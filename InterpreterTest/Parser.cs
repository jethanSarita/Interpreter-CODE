using System;
using System.Collections.Generic;
using System.Linq;
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
                if (currentToken.Type == TokenType.END && Peek().Type == TokenType.CODE)
                {
                    _insideCodeBlock = false;
                    break;
                }

                if (
                    currentToken.Type == TokenType.INT ||
                    currentToken.Type == TokenType.CHAR ||
                    currentToken.Type == TokenType.FLOAT ||
                    currentToken.Type == TokenType.BOOL
                   )
                {
                    if (Peek().Type == TokenType.IDENTIFIER)
                    {
                        List<Token> identifiers = ReadIdentifiers();
                        foreach (var toks in identifiers)
                        {
                            statements.Add(ParseVariableDeclaration(currentToken, toks));
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Invalid {currentToken.Value} declaration");
                    }
                }
                if (currentToken.Type == TokenType.DISPLAY)
                {

                }
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
            Token currTok = _tokens[_position];
            bool commaCheck = true;
            while (true)
            {
                if (_tokens[_position++].Type == TokenType.IDENTIFIER)
                {
                    commaCheck = true;
                    toks.Add(currTok);
                }
                else if (_tokens[_position++].Type == TokenType.COMMA && commaCheck)
                {
                    commaCheck = false;
                    _position++;
                }
                else
                {
                    break;
                }
            }
            return toks;

        }

        private ASTNode Statement()
        {
            if (!_insideCodeBlock)
            {
                throw new Exception("Statement outside CODE block.");
            }
            // other parsing logic for different types of statements
            return new PlaceholderNode("PlaceholderStatement");
        }

        private ASTNode ParseVariableDeclaration(Token dataTypeToken, Token varNameToken)
        {
            return new VariableDeclarationNode(dataTypeToken.Value, varNameToken.Value);
        }
    }

    internal abstract class ASTNode { }

    internal class VariableDeclarationNode : ASTNode
    {
        public String _dataType  { get; }
        public String _varName { get; }

        public VariableDeclarationNode(string dataType, string varName)
        {
            _dataType = dataType;
            _varName = varName;
        }
    }

    internal class ProgramNode : ASTNode
    {
        public List<ASTNode> Statements { get; }

        public ProgramNode(List<ASTNode> statements)
        {
            Statements = statements;
        }
    }

    internal class PlaceholderNode : ASTNode
    {
        public string StatementType { get; }

        public PlaceholderNode(string statementType)
        {
            StatementType = statementType;
        }

        public override string ToString()
        {
            return $"PlaceholderNode: {StatementType}";
        }
    }
}
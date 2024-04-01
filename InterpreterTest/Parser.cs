using System;
using System.Collections.Generic;
using System.Linq;
using static InterpreterTest.Token;

namespace InterpreterTest
{
    internal class Parser
    {
        private readonly List<Token> _tokens;
        private int _position;

        private readonly string[] _keywords = { "INT", "BOOL", "CHAR", "FLOAT", "FALSE", "TRUE" };

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        public ASTNode Parse()
        {
            return VariableDeclare();
        }

        private ASTNode VariableDeclare()
        {
            // Expecting "INT", "BOOL", "CHAR", or "FLOAT" identifier
            Token identifierToken = ConsumeExpected(TokenType.Identifier, _keywords);

            // variable declaration for BOOL
            if (identifierToken.Value == "BOOL")
            {
                Token keywordToken = ConsumeExpected(TokenType.Keyword);
                Token operatorToken = ConsumeExpected(TokenType.Operator, "=");
                Token valueToken = ConsumeExpected(TokenType.Identifier, "TRUE", "FALSE");
                Token separatorToken = ConsumeExpected(TokenType.Separator, ";"); 
                return new DeclareNode(identifierToken, keywordToken, operatorToken, valueToken, separatorToken);
            }
            else
            {
            // variable declaration for INT, CHAR
                Token keywordToken = ConsumeExpected(TokenType.Keyword);
                Token operatorToken = ConsumeExpected(TokenType.Operator, "=");
                Token literalToken = ConsumeExpected(TokenType.Literal);
                Token separatorToken = ConsumeExpected(TokenType.Separator, ";"); 
                return new DeclareNode(identifierToken, keywordToken, operatorToken, literalToken, separatorToken);
            }
        }

        private Token ConsumeExpected(TokenType type, params string[] values)
        {
            Token token = Consume();
            if (token.Type != type || (values != null && values.Length > 0 && !values.Contains(token.Value)))
            {
                string expectedValues = values != null ? string.Join(", ", values) : "any value";
                throw new Exception($"Expected token of type {type} and one of the following values: {expectedValues}, but got {token}.");
            }
            return token;
        }


        private Token Consume()
        {
            if (_position >= _tokens.Count)
            {
                throw new Exception("Unexpected end of input.");
            }
            return _tokens[_position++];
        }
    }

    internal abstract class ASTNode { }

    internal class DeclareNode : ASTNode
    {
        public Token Identifier { get; }
        public Token Keyword { get; }
        public Token Operator { get; }
        public Token Literal { get; }
        public Token Separator { get; }

        public DeclareNode(Token identifier, Token keyword, Token @operator, Token literal,  Token separator)
        {
            Identifier = identifier;
            Keyword = keyword;
            Operator = @operator;
            Literal = literal;
            Separator = separator;
        }
    }
}

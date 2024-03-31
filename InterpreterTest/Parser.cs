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

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
        }

        public ASTNode Parse()
        {
            return Expression();
        }

        private ASTNode Expression()
        {
            ASTNode node = Term();
            while (Match(TokenType.Operator, "+", "-"))
            {
                Token op = Consume();
                ASTNode right = Term();
                node = new BinaryExpressionNode(op, node, right);
            }
            return node;
        }

        private ASTNode Term()
        {
            ASTNode node = Factor();
            while (Match(TokenType.Operator, "*", "/"))
            {
                Token op = Consume();
                ASTNode right = Factor();
                node = new BinaryExpressionNode(op, node, right);
            }
            return node;
        }

        private ASTNode Factor()
        {
            if (Match(TokenType.Literal))
            {
                return new LiteralNode(Consume());
            }
            else if (Match(TokenType.Identifier))
            {
                return new IdentifierNode(Consume());
            }
            else if (Match(TokenType.Operator, "("))
            {
                Consume(); // Consume '('
                ASTNode expression = Expression();
                if (!Match(TokenType.Operator, ")"))
                {
                    throw new Exception("Expected ')' after expression.");
                }
                Consume(); // Consume ')'
                return expression;
            }
            else
            {
                throw new Exception("Unexpected token.");
            }
        }

        // Utility methods for token manipulation

        private bool Match(TokenType type, params string[] values)
        {
            if (_position >= _tokens.Count)
            {
                return false;
            }
            Token token = _tokens[_position];
            if (token.Type == type)
            {
                foreach (string value in values)
                {
                    if (token.Value == value)
                    {
                        return true;
                    }
                }
            }
            return false;
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

    // Define the abstract syntax tree nodes
    internal abstract class ASTNode { }

    internal class BinaryExpressionNode : ASTNode
    {
        public Token Operator { get; }
        public ASTNode Left { get; }
        public ASTNode Right { get; }

        public BinaryExpressionNode(Token op, ASTNode left, ASTNode right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }
    }

    internal class LiteralNode : ASTNode
    {
        public Token Literal { get; }

        public LiteralNode(Token literal)
        {
            Literal = literal;
        }
    }

    internal class IdentifierNode : ASTNode
    {
        public Token Identifier { get; }

        public IdentifierNode(Token identifier)
        {
            Identifier = identifier;
        }
    }
}


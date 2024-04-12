﻿using System;
using System.CodeDom.Compiler;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
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
        private bool _insideCodeBlock = false;

        private readonly string[] _keywords = {
            "INT", "BOOL", "CHAR", "FLOAT", "FALSE", "TRUE", "BEGIN", "END",
            "DISPLAY", "SCAN", "IF", "ELSE", "WHILE", "AND", "OR", "NOT"
        };

        

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
            _position = 0;
            _lineCounter = 1;
        }

        public ProgramNode Parse()
        {
            Token currentToken = _tokens[_position];
            if (!(currentToken.Type == TokenType.BEGIN && Peek(1) != null && Peek(1).Type == TokenType.CODE))
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
                Console.WriteLine("Current Token: " + currentToken);
                if (currentToken.Type == TokenType.END && Peek(1) != null && Peek(1).Type == TokenType.CODE)
                {
                    _insideCodeBlock = false;
                    break;
                }

                //[DataType][Identifier] + ([Comma][Identifier])* + [Equals][Literal]

                //Check if datatype
                if (
                    currentToken.Type == TokenType.INT ||
                    currentToken.Type == TokenType.CHAR ||
                    currentToken.Type == TokenType.FLOAT ||
                    currentToken.Type == TokenType.BOOL
                   )
                {
                    Token dataType = currentToken;
                    _position++;
                    while (_position < _tokens.Count)
                    {
                        currentToken = _tokens[_position];
                        if (currentToken.Type == TokenType.IDENTIFIER)
                        {
                            //Initialize Variable
                            statements.Add(ParseVariableDeclaration(dataType, currentToken));
                            //Check if following Token is not null
                            if (Peek(1) != null)
                            {
                                //Not null, check if its equal sign
                                if (Peek(1).Type == TokenType.EQUAL)
                                {
                                    //It's equal, check if following is a literal
                                    if (IsLiteral(Peek(2)))
                                    {
                                        //It's a literal, check compatibility and add VariableAssignementNode
                                        statements.Add(TypeCompatibility(dataType, currentToken, Peek(2)));
                                        _position += 2;
                                    }
                                    else
                                    {
                                        //Not a literal, throw error
                                        throw new InvalidOperationException($"Error at line {_lineCounter}: '{dataType.Value} {currentToken.Value} {Peek(2).Value}' <--- Is invalid");
                                    }
                                }

                                //Not null, may or may not had prior assignment operation, check comma
                                if (Peek(1).Type == TokenType.COMMA)
                                {
                                    //Is comma, check if following is not null
                                    if (Peek(2) != null)
                                    {
                                        //Not null, check if following identifier
                                        if (Peek(2).Type == TokenType.IDENTIFIER)
                                        {
                                            //Is identifier,
                                            _position += 2;
                                        }
                                        else
                                        {
                                            //Not identifier, throw error
                                            throw new InvalidOperationException($"Error at line {_lineCounter}: ...'{Peek(1).Value} {Peek(2).Value}' <--- Not an identifier");
                                        }
                                    }
                                    else
                                    {
                                        //Is null, throw error
                                        throw new InvalidOperationException($"Error at line {_lineCounter}: ...'{currentToken.Value}{Peek(1).Value}' <--- No follow up");
                                    }

                                }
                                else
                                {
                                    //no comma, means end of variable declaration and/or assignment
                                    break;
                                }
                            }
                            else
                            {
                                //Is null, throw error
                                throw new InvalidOperationException($"Error at line {_lineCounter}: '{dataType.Value} {currentToken.Value}' <--- Missing code follow up, check if within CODE block");
                            }
                        }
                    }
                }
                //[Identifier][Equals]([Literal] + [Expression])
                else if (currentToken.Type == TokenType.IDENTIFIER)
                {
                    statements.Add(ParseVariableAssignment());

                    /*string idenitiferName = currentToken.Value;
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
                            throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid \"{idenitiferName}\" (Identifier) call/assignment. No succeeding literal after equals sign");
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException($"Error at line  {_lineCounter} : Invalid \"{idenitiferName}\" (Identifier) call/assignment, no succeeding equals sign (=)");
                    }*/
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
            if (_insideCodeBlock)
            {
                throw new Exception($"Error at line {_lineCounter}: Expected 'END CODE'");
            }
            return new ProgramNode(statements);
        }

        private bool IsOperator(Token token)
        {
            bool result = false;
            switch (token.Type)
            {
                case TokenType.MINUS:
                    result = true;
                    break;
                case TokenType.PLUS:
                    result = true;
                    break;
                case TokenType.SLASH:
                    result = true;
                    break;
                case TokenType.STAR:
                    result = true;
                    break;
                case TokenType.MODULO:
                    result = true;
                    break;
            }
            return result;
        }

        private ASTNode TypeCompatibility(Token dataType, Token variable, Token literal)
        {
            //Check datatype
            string errorMessage = "";
            bool error = false;
            ASTNode node = ParseVariableAssignment(variable, literal);
            switch (dataType.Type)
            {
                case TokenType.INT:
                    if (!(literal.Type == TokenType.NUMBER))
                    {
                        errorMessage = $"Invalid assignment, literal '{literal.Value}' is not INT";
                        error = true;
                    }
                    break;
                case TokenType.CHAR:
                    if (!(literal.Type == TokenType.LETTER))
                    {
                        errorMessage = $"Invalid assignment, literal '{literal.Value}' is not CHAR";
                        error = true;
                    }
                    break;
                case TokenType.FLOAT:
                    if (!(literal.Type == TokenType.DECIMAL_NUMBER))
                    {
                        errorMessage = $"Invalid assignment, literal '{literal.Value}' is not FLOAT";
                        error = true;
                    }
                    break;
                case TokenType.BOOL:
                    if (!(literal.Type == TokenType.TRUE || literal.Type == TokenType.FALSE))
                    {
                        errorMessage = $"Invalid assignment, literal '{literal.Value}' is not BOOL";
                        error = true;
                    }
                    break;
            }
            if (error)
            {
                throw new InvalidOperationException($"Error at line {_lineCounter}: " + errorMessage);
            }
            return node;
        }

        private bool IsLiteral(Token token)
        {
            if (token.Type == TokenType.IDENTIFIER ||
                token.Type == TokenType.LETTER ||
                token.Type == TokenType.NUMBER ||
                token.Type == TokenType.DECIMAL_NUMBER ||
                token.Type == TokenType.STRING)
            {
                return true;
            }
            return false;
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
            bool idenCheck = true;
            while (true)
            {
                currTok = _tokens[_position];
                //Console.WriteLine("Currently reading: " + currTok.Value); 
                if (currTok.Type == TokenType.IDENTIFIER)
                {
                    if (!idenCheck)
                    {
                        throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid multiple variable declaration unseperated by comma (,)");
                    }

                    commaCheck = true;
                    idenCheck = false;

                    toks.Add(currTok);
                }
                else if (currTok.Type == TokenType.COMMA && commaCheck)
                {
                    commaCheck = false;
                    idenCheck = true;
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
        /*
        private ASTNode ParseVariableAssignment()
        {

            Token currentToken = _tokens[_position];

            string varName = currentToken.Value;
            ExpressionNode node = new ExpressionLiteral(null, null);
            //check if following token is null
            if (Peek(1) != null)
            {
                //not null, check if token type is equal
                if (Peek(1).Type == TokenType.EQUAL)
                {
                    //equal, check if following is null
                    if (Peek(2) != null)
                    {
                        //not null, check if following is identifier
                        if (Peek(2).Type == TokenType.IDENTIFIER)
                        {
                            //add this later
                        }
                        //check if following is number
                        else if (Peek(2).Type == TokenType.NUMBER)
                        {
                            node = ParseExpressionLiteral(Peek(2));
                            if (Peek(3) != null)
                            {
                                if (IsOperator(Peek(3)))
                                {
                                    if (Peek(4) != null)
                                    {
                                        if (Peek(4).Type == TokenType.NUMBER)
                                        {
                                            node = new ExpressionBinary(ParseExpressionLiteral(Peek(2)), ParseExpressionLiteral(Peek(4)), Peek(3).Value);
                                            string lastOp = Peek(3).Value;
                                            _position += 4;

                                            while (_position < _tokens.Count)
                                            {
                                                currentToken = _tokens[_position];

                                                if (Peek(1) != null)
                                                {
                                                    if (IsOperator(Peek(1)))
                                                    {
                                                        if (Peek(2) != null)
                                                        {
                                                            if (Peek(2).Type == TokenType.NUMBER)
                                                            {
                                                                if (IsAS(lastOp) && IsMD(Peek(1).Value))
                                                                {
                                                                    Console.WriteLine("AS MD");
                                                                    if (node is ExpressionBinary n)
                                                                    {
                                                                        ExpressionNode newNode = new ExpressionBinary(n._right, ParseExpressionLiteral(Peek(2)), Peek(1).Value);
                                                                        node = new ExpressionBinary(n._left, newNode, lastOp);
                                                                        lastOp = Peek(1).Value;
                                                                    }
                                                                    else
                                                                    {
                                                                        //error
                                                                    }
                                                                }
                                                                else if (IsAS(lastOp) && IsAS(Peek(1).Value))
                                                                {
                                                                    Console.WriteLine("AS AS");
                                                                    if (node is ExpressionBinary n)
                                                                    {
                                                                        node = new ExpressionBinary(n, ParseExpressionLiteral(Peek(2)), Peek(1).Value);
                                                                        lastOp = Peek(1).Value;
                                                                    }
                                                                    else
                                                                    {
                                                                        //error
                                                                    }
                                                                }
                                                                else if (IsMD(lastOp) && IsAS(Peek(1).Value))
                                                                {
                                                                    Console.WriteLine("MD AS");
                                                                    if (node is ExpressionBinary n)
                                                                    {
                                                                        node = new ExpressionBinary(n, ParseExpressionLiteral(Peek(2)), Peek(1).Value);
                                                                        lastOp = Peek(1).Value;
                                                                    }
                                                                    else
                                                                    {
                                                                        //error
                                                                    }
                                                                }
                                                                else if (IsMD(lastOp) && IsMD(Peek(1).Value))
                                                                {
                                                                    Console.WriteLine("MD MD");
                                                                    if (node is ExpressionBinary n)
                                                                    {
                                                                        ExpressionNode newNode = new ExpressionBinary(n._right, ParseExpressionLiteral(Peek(2)), Peek(1).Value);
                                                                        node = new ExpressionBinary(n._left, newNode, lastOp);
                                                                        lastOp = Peek(1).Value;
                                                                    }
                                                                    else
                                                                    {
                                                                        //error
                                                                    }
                                                                }
                                                                _position += 2;
                                                            }
                                                            else
                                                            {
                                                                //error
                                                            }
                                                        }
                                                        else
                                                        {
                                                            //error
                                                        }
                                                    }
                                                    else if (Peek(1).Type == TokenType.LINE_SEPARATOR)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        //error
                                                    }
                                                }
                                                else
                                                {

                                                }
                                            }
                                        }
                                        else
                                        {
                                            //error
                                        }
                                    }
                                    else
                                    {
                                        //error
                                    }
                                }
                                //Not operator, check if end of line
                                else if (Peek(3).Type == TokenType.LINE_SEPARATOR)
                                {
                                    _position += 2;
                                }
                                else
                                {
                                    //error
                                }
                            }
                            else
                            {
                                //error
                            }
                        }
                        //check if following is decimal number
                        else if (Peek(2).Type == TokenType.DECIMAL_NUMBER)
                        {

                        }
                    }
                    else
                    {

                    }
                }
                else
                {
                    //not equal,
                }
            }
            else
            {
                //null, throw error
                throw new InvalidOperationException($"Error at line {_lineCounter}: '{currentToken.Value}' <--- Missing follow up");
            }
            Console.WriteLine("-------------------Variable Name: " + varName + ", Node: " + node);
            return new VariableAssignmentNode2(varName, node);
        }

        private bool IsMD(string lastOp)
        {
            bool result = false;
            switch (lastOp)
            {
                case "*":
                    result = true;
                    break;
                case "/":
                    result = true;
                    break;
            }
            return result;
        }

        private bool IsAS(string lastOp)
        {
            bool result = false;
            switch (lastOp)
            {
                case "+":
                    result = true;
                    break;
                case "-":
                    result = true;
                    break;
            }
            return result;
        }*/

        //recursive descent parsing for arithmetic expression into variable assignment with operator precedence
        // parses variable assignment part of token stream
        private ASTNode ParseVariableAssignment()
        {
            Token currentToken = _tokens[_position];
            string varName = currentToken.Value;

            ExpressionNode node = ParseExpression();

            Console.WriteLine("-------------------Variable Name: " + varName + ", Node: " + node);
            return new VariableAssignmentNode2(varName, node);
        }

        // breaking down the expression into smaller parts for parsing
        private ExpressionNode ParseExpression()
        {
            // parse first term
            ExpressionNode left = ParseTerm();


            // continue parsing other terms and create nodes
            // + and - as parents and other terms as children
            while (Peek(1) != null && (Peek(1).Value == "+" || Peek(1).Value == "-"))
            {
                Token opToken = NextToken(); 
                ExpressionNode right = ParseTerm(); 
                left = new ExpressionBinary(left, right, opToken.Value); 
            }

            return left;
        }

        // now parse terms to factors
        private ExpressionNode ParseTerm()
        {
            // parse first factor
            ExpressionNode left = ParseFactor();

            // continue parsing other factors and create nodes
            // * and / as parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "*" || Peek(1).Value == "/"))
            {
                Token opToken = NextToken(); 
                ExpressionNode right = ParseFactor(); 
                left = new ExpressionBinary(left, right, opToken.Value);
            }

            return left;
        }

        // parse factors into nodes for evaluation
        private ExpressionNode ParseFactor()
        {
            Token token = NextToken();

            // if token is a literal, create a literal node in the AST
            if (token.Type == TokenType.NUMBER || token.Type == TokenType.DECIMAL_NUMBER || token.Type == TokenType.IDENTIFIER)
            {
                return new ExpressionLiteral(token.Value, token.Type.ToString());
            }
            // if current token is open paren, parse expression inside the parenthesis
            else if (token.Type == TokenType.LEFT_PAREN)
            {
                try
                {
                    ExpressionNode expression = ParseExpression();
                    _position++;
                    Consume(TokenType.RIGHT_PAREN);
                    return expression;
                }
                catch
                {
                    throw new InvalidOperationException("Expected token of type RIGHT_PAREN, but found " + Peek(0).Type + ", Token Value: " + Peek(0).Value);
                }
            }
            // if naay equals, parse the expression on the right side of equals
            else if (token.Type == TokenType.EQUAL)
            {
                ExpressionNode rhsExpression = ParseExpression();
                return rhsExpression;
            }
            else
            {
                throw new InvalidOperationException("Invalid factor.");
            }
        }

        // utility methods for token manipulation
        // retrieves next token and advances position in the token stream
        private Token NextToken()
        {
            if (_position < _tokens.Count - 1)
            {
                _position++;
                return _tokens[_position];
            }
            else
            {
                throw new InvalidOperationException("No more tokens available.");
            }
        }

        // if next token matches argument advance position in the token stream
        private void Consume(TokenType type)
        {
            if (_position < _tokens.Count && _tokens[_position].Type == type)
            {
                _position++;
            }
            else
            {
                throw new InvalidOperationException($"Expected token of type {type}, but found {_tokens[_position].Type}.");
            }
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
            Token currToken;
            bool concatLock = true;
            DisplayNode result = new DisplayVariableNode("null");
            //handle different types of display items
            while (_position < _tokens.Count)
            {
                currToken = _tokens[_position];

                if (currToken.Type == TokenType.LINE_SEPARATOR)
                {
                    break;
                }

                if (CheckIfDisplayable(currToken) && concatLock)
                {
                    if (Peek(1) != null && Peek(1).Type == TokenType.CONCATENATE)
                    {
                        if (Peek(2) != null && CheckIfDisplayable(Peek(2)))
                        {
                            result = new DisplayConcatNode(ParseDisplayable(currToken), ParseDisplayable(Peek(2)));
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
                        result = new DisplayConcatNode(result, ParseDisplayable(Peek(1)));
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
            }
            Console.WriteLine("Result: " + result);
            _position--;
            return result;
        }

        private DisplayNode ParseDisplayable(Token token)
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
                        throw new InvalidOperationException($"Error at line {_lineCounter}: Invalid token in SCAN statement");
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
            return new VariableAssignmentNode2(variableName.Value, ParseExpressionLiteral(literal));
        }

        private ExpressionNode ParseExpressionLiteral(Token literal)
        {
            string literalType = "";
            switch (literal.Type)
            {
                case TokenType.LETTER:
                    literalType = "LETTER";
                    break;
                case TokenType.NUMBER:
                    literalType = "NUMBER";
                    break;
                case TokenType.DECIMAL_NUMBER:
                    literalType = "DECIMAL_NUMBER";
                    break;
                case TokenType.TRUE:
                    literalType = "TRUE";
                    break;
                case TokenType.FALSE:
                    literalType = "FALSE";
                    break;
            }
            return new ExpressionLiteral(literal.Value, literalType);
        }

        private string ParseExpressionLiteralType(Token literal)
        {
            string literalType = "";
            switch (literal.Type)
            {
                case TokenType.LETTER:
                    literalType = "LETTER";
                    break;
                case TokenType.NUMBER:
                    literalType = "NUMBER";
                    break;
                case TokenType.DECIMAL_NUMBER:
                    literalType = "DECIMAL_NUMBER";
                    break;
                case TokenType.TRUE:
                    literalType = "TRUE";
                    break;
                case TokenType.FALSE:
                    literalType = "FALSE";
                    break;
            }
            return literalType;
        }
    }
}
using System;
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
                if (currentToken.Type == TokenType.COMMENT)
                {
                    SkipComments();
                }
                else
                {
                    throw new Exception($"Error at line {_lineCounter}: Expected 'BEGIN CODE'");
                }
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
                    if(Peek(2) != null)
                    {
                        _lineCounter++;
                        throw new Exception($"Error at line {_lineCounter + 1}: Code Ended already at line {_lineCounter}");
                    }
                    Console.WriteLine("no end code--------------------------------------------------------------------");
                    _insideCodeBlock = false;
                    break;
                }

                //[DataType][Identifier] + ([Comma][Identifier])* + [Equals][Literal]

                if (currentToken.Type == TokenType.IF)
                {
                    statements.Add(ParseConditional());
                }
                else if (currentToken.Type == TokenType.WHILE)
                {
                    statements.Add(ParseLoop());
                }
               
                if(currentToken.Type == TokenType.COMMENT)
                {
                    SkipComments();
                    continue;
                }

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
                                        if (IsOperator(Peek(3)))
                                        {
                                            ExpressionNode value = ParseExpression();
                                            statements.Add(ParseVariableAssignment(currentToken, value));
                                        }
                                        else
                                        {
                                            //It's a literal, check compatibility and add VariableAssignementNode
                                            statements.Add(TypeCompatibility(dataType, currentToken, Peek(2)));
                                            _position += 2;
                                        }                                       
                                    }
                                    else if(IsUnaryOperator(Peek(2)))
                                    {
                                        ExpressionNode value = ParseUnary();
                                        statements.Add(ParseVariableAssignment(currentToken, value));

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
                else if (currentToken.Type == TokenType.SCAN)
                {
                    statements.Add(ParseScanStatement());
                }
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

        private void SkipComments()
        {
            while (_position < _tokens.Count && _tokens[_position].Type != TokenType.LINE_SEPARATOR)
            {
                _position++;
            }
            // Move to the next line
            _lineCounter++;
        }

            public List<ASTNode> ParseStatements()
        {
            Token currentToken = _tokens[_position];
            var statements = new List<ASTNode>();
            while (Peek(0).Type != TokenType.END)
            {
                currentToken = _tokens[_position];
                Console.WriteLine("Current Token: " + currentToken);

                if (currentToken.Type == TokenType.IF)
                {
                    statements.Add(ParseConditional());
                }
                else if (currentToken.Type == TokenType.WHILE)
                {
                    statements.Add(ParseLoop());
                }

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
                else if (currentToken.Type == TokenType.SCAN)
                {
                    statements.Add(ParseScanStatement());
                }
                else if (currentToken.Type == TokenType.IDENTIFIER)
                {
                    statements.Add(ParseVariableAssignment());
                }
                //[Display][Colon][{contents}]
                else if (currentToken.Type == TokenType.DISPLAY)
                {
                    statements.Add(ParseDisplayStatement());
                }

                else if (currentToken.Type == TokenType.LINE_SEPARATOR)
                {
                    _lineCounter++;
                }
                _position++;
            }
            return statements;
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

        private bool IsUnaryOperator(Token token)
        {
            return token.Type == TokenType.PLUS || token.Type == TokenType.MINUS;
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
            ExpressionNode expression = ParseLogical();

            return expression;
        }

        private ExpressionNode ParseLogical()
        {
            // parse first factor
            ExpressionNode left = ParseComparison();

            // continue parsing other factors and create nodes
            // & parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "AND" || Peek(1).Value == "OR" || Peek(1).Value == "NOT"))
            {
                Token opToken = GetNextToken();
                ExpressionNode right = ParseComparison();
                left = new ExpressionConcat(left, right);
            }

            return left;
        }
        
        private ExpressionNode ParseComparison()
        {
            // parse first factor
            ExpressionNode left = ParseConcat();

            // continue parsing other factors and create nodes
            // & parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "<" || Peek(1).Value == ">" ||
                                        Peek(1).Value == "<=" || Peek(1).Value == ">=" ||
                                        Peek(1).Value == "==" || Peek(1).Value == "<>"))
            {
                ExpressionNode right = ParseConcat();
                left = new ExpressionConcat(left, right);
            }

            return left;
        }

        private ExpressionNode ParseConcat()
        {
            // parse first factor
            ExpressionNode left = ParseAdditive();

            // continue parsing other factors and create nodes
            // & parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "&"))
            {
                Token opToken = GetNextToken();
                ExpressionNode right = ParseAdditive();
                left = new ExpressionConcat(left, right);
            }

            return left;
        }


        private ExpressionNode ParseAdditive()
        {
            // parse first factor
            ExpressionNode left = ParseMultiplicative();

            // continue parsing other factors and create nodes
            // *, /, and % as parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "+" || Peek(1).Value == "-"))
            {
                Token opToken = GetNextToken();
                ExpressionNode right = ParseMultiplicative();
                left = new ExpressionBinary(left, right, opToken.Value);
            }

            return left;
        }

        // now parse terms to factors
        private ExpressionNode ParseMultiplicative()
        {
            // parse first factor
            ExpressionNode left = ParseUnary();

            // continue parsing other factors and create nodes
            // *, /, and % as parents and other factors as children
            while (Peek(1) != null && (Peek(1).Value == "*" || Peek(1).Value == "/" || Peek(1).Value == "%"))
            {
                Token opToken = GetNextToken();
                ExpressionNode right = ParseUnary();
                left = new ExpressionBinary(left, right, opToken.Value);
            }

            return left;
        }

        private ExpressionNode ParseUnary()
        {
            Console.WriteLine("==UNARY FOUND==");
            if (Peek(1) != null && (Peek(1).Value == "-" || Peek(1).Value == "+"))
            {
                Console.WriteLine("==UNARY FOUND==");
                Token opToken = GetNextToken();
                PrintCurrentToken();
                ExpressionNode right = ParseFactor();
                PrintCurrentToken();
                //ExpressionNode left = new ExpressionLiteral("0", "NUMBER");
                ExpressionNode left = new ExpressionLiteral("0", TokenType.NUMBER.ToString());

                return new ExpressionBinary(left, right, opToken.Value);
            }
            return ParseFactor();
        }

        // parse factors into nodes for evaluation
        private ExpressionNode ParseFactor()
        {
            Console.WriteLine("no error encountered, gwapo ka1");
            PrintCurrentToken();
            Token token = GetNextToken();
            Console.WriteLine("token is " + token.Type);

            // if token is a literal, create a literal node in the AST
            if (token.Type == TokenType.NUMBER || token.Type == TokenType.DECIMAL_NUMBER)
            {
                Console.WriteLine("no error encountered, gwapo ka3");
                return new ExpressionLiteral(token.Value, token.Type.ToString());
            }
            // if current token is a boolean literal
            else if (token.Type == TokenType.TRUE || token.Type == TokenType.FALSE)
            {
                Console.WriteLine("no error encountered, gwapo ka4");
                return new ExpressionLiteral(token.Value, token.Type.ToString());
            }
            // if current token is an identifier, create an expression variable node
            else if (token.Type == TokenType.IDENTIFIER)
            {
                Console.WriteLine("no error encountered, gwapo ka5");
                return new ExpressionVariable(token.Value);
            }
            // if current token is open paren, parse expression inside the parenthesis
            else if (token.Type == TokenType.LEFT_PAREN)
            {
                Console.WriteLine("no error encountered, gwapo ka6");
                ExpressionNode expression = ParseExpression();
                _position++;
                Consume(TokenType.RIGHT_PAREN);
                Console.WriteLine("no error encountered, gwapo ka7");
                return expression;
            }
            // if naay equals, parse the expression on the right side of equals
            else if (token.Type == TokenType.EQUAL)
            {
                Console.WriteLine("no error encountered, gwapo ka8");
                ExpressionNode rhsExpression = ParseExpression();
                return rhsExpression;
            }
            else if (token.Type == TokenType.STRING || token.Type == TokenType.LETTER)
            {
                return new ExpressionLiteral(token.Value, "STRING");

            }
            else
            {
                throw new InvalidOperationException("Invalid factor.");
            }
        }

        // helper methods for token manipulation
        // retrieves next token and advances position in the token stream
        private Token GetNextToken()
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

        // consume tokentype
        private void Consume(TokenType type)
        {
            if (_position < _tokens.Count && _tokens[_position].Type == type)
            {

            }
            else
            {
                throw new InvalidOperationException($"Expected token of type {type}, but found {_tokens[_position].Type}.");
            }
        }

        private Token Consume()
        {
                return _tokens[_position++];
        }

        private void Consume_then_Move(TokenType type)
        {
            if (_position < _tokens.Count && _tokens[_position].Type == type)
            {
                // advance the position to consume the token
                _position++;
            }
            else
            {
                throw new InvalidOperationException($"Expected token of type {type}, but found {_tokens[_position].Type}.");
            }
        }

        private void PrintCurrentToken() {
            Console.WriteLine("Current token is " + Peek(0).Type);
        }

        private ASTNode ParseConditional()
        {
            try
            {
                if (Peek(0).Type == TokenType.IF)
                {
                    ConditionalNode returnConditionNode = new ConditionalNode(null, null, null);
                    Console.WriteLine("==IF ARRIVE==");
                    ExpressionNode condition = ParseExpression();
                    Consume_then_Move(TokenType.RIGHT_PAREN);
                    Consume_then_Move(TokenType.LINE_SEPARATOR);
                    Console.WriteLine("successful parsing of expression in conditional statement");

                    Consume_then_Move(TokenType.BEGIN);
                    Consume_then_Move(TokenType.IF);

                    List<ASTNode> ifStatements = ParseStatements();

                    Consume_then_Move(TokenType.END);
                    Consume_then_Move(TokenType.IF);
                    Consume_then_Move(TokenType.LINE_SEPARATOR);

                    returnConditionNode.Condition = condition;
                    returnConditionNode.IfStatements = ifStatements;

                    if (Peek(0).Type == TokenType.ELSE && Peek(1).Type == TokenType.IF)
                    {
                        Console.WriteLine("==ELSE IF ARRIVE==");
                        Consume_then_Move(TokenType.ELSE);
                        returnConditionNode.ElseStatements = (ConditionalNode)ParseConditional();
                        _position++;
                    }
                    else if (Peek(0).Type == TokenType.ELSE)
                    {
                        Console.WriteLine("==ELSE ARRIVE==");
                        Consume_then_Move(TokenType.ELSE);
                        Consume_then_Move(TokenType.LINE_SEPARATOR);
                        Consume_then_Move(TokenType.BEGIN);
                        Consume_then_Move(TokenType.IF);

                        List<ASTNode> elseStatements = ParseStatements();
                        ConditionalNode lastElse = new ConditionalNode(condition, elseStatements, null);
                        lastElse.isAlwaysTrue = true;
                        returnConditionNode.ElseStatements = lastElse;

                        Consume_then_Move(TokenType.END);
                        Consume_then_Move(TokenType.IF);
                    }
                    _position--;
                    Console.WriteLine("Conditional Node Parsing Finished");
                    return returnConditionNode;
                }
                else
                {
                    throw new Exception("Invalid conditional statement.");
                }
            }
            catch
            {
                throw new Exception($"Unexpected token encountered: {_tokens[_position].Type}");
            }
        }

        private ASTNode ParseLoop()
        {
            LoopNode returnLoopNode = null;
            try
            {
                    returnLoopNode = new LoopNode(null, null, null);
                    Console.WriteLine("==FIRST WHILE ARRIVE==");
                    ExpressionNode loop_condition = ParseExpression();
                    Consume_then_Move(TokenType.RIGHT_PAREN);
                    Consume_then_Move(TokenType.LINE_SEPARATOR);
                    Console.WriteLine("successful parsing of expression in loop condition");

                    Consume_then_Move(TokenType.BEGIN);
                    Consume_then_Move(TokenType.WHILE);

                    List<ASTNode> loop_statements = ParseStatements();

                    Consume_then_Move(TokenType.END);
                    Consume_then_Move(TokenType.WHILE);
                    Consume_then_Move(TokenType.LINE_SEPARATOR);

                    returnLoopNode.LoopCondition = loop_condition;
                    returnLoopNode.LoopStatements = loop_statements;
                    _position--;
                    return returnLoopNode;
            }
            catch
            {
                throw new Exception($"Unexpected token encountered: {_tokens[_position].Type}");
            }
        }

        private ASTNode ParseVariableDeclaration(Token dataTypeToken, Token varNameToken/*, ASTNode variableAssignment*/)
        {
            return new VariableDeclarationNode(dataTypeToken.Value, varNameToken.Value);
        }

        private ASTNode ParseDisplayStatement()
        {
            _position++;
            PrintCurrentToken();
            //after DISPLAY there should be a colon ':'
            if (_tokens[_position].Type != TokenType.COLON)
            {
                throw new InvalidOperationException($"Error at line {_lineCounter}: Expected ':' after DISPLAY statement");
            }
            PrintCurrentToken();
            //parse display items until end of the line
            DisplayNode displayNode = new DisplayNode();
            displayNode.toDisplay = ParseExpression();

            _position++;
            return displayNode;
        }

        /*
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
        }*/

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

                /*if (currToken.Type == TokenType.IDENTIFIER)
                {
                    //scans.Add()
                }*/
                _position++;
            }

            return new ScanStatementNode(scans);
        }

        private ASTNode ParseVariableAssignment(Token variableName, Token literal)
        {
            return new VariableAssignmentNode2(variableName.Value, ParseExpressionLiteral(literal));
        }

        private ASTNode ParseVariableAssignment(Token varname, ExpressionNode node)
        {
            return new VariableAssignmentNode2(varname.Value, node);
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
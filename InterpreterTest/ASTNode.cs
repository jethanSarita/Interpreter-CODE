using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal abstract class ASTNode {
        public String name;
    }

    internal class ProgramNode : ASTNode
    {
        public List<ASTNode> Statements { get; }

        public ProgramNode(List<ASTNode> statements)
        {
            Statements = statements;
            name = "Program";
    }
    }

    internal class VariableDeclarationNode : ASTNode
    {
        public String _dataType { get; }
        public String _varName { get; }
        //public VariableAssignmentNode2 _varAssign { get; }


        public VariableDeclarationNode(string dataType, string varName)
        {
            _dataType = dataType;
            _varName = varName;
            //_varAssign = varAssign;
            name = "VarDeclare";
    }

        public override String ToString()
        {
            return $"Data Type: {_dataType}, Variable Name: {_varName}";
        }

    }

    internal class VariableAssignmentNode2 : ASTNode
    {
        public string _varName;
        public ExpressionNode _expressionNode;

        public VariableAssignmentNode2(string varName, ExpressionNode expressionNode)
        {
            _varName = varName ?? throw new ArgumentNullException(nameof(varName));
            _expressionNode = expressionNode ?? throw new ArgumentNullException(nameof(expressionNode));
            name = "VarAssign2";
    }

        public void eval(SymbolStorage symbolStorage)
        {
            if (_expressionNode != null)
            {
                dynamic result = _expressionNode.eval(symbolStorage);
                symbolStorage.AssignVariable(_varName, result);
            }
            else
            {
                throw new InvalidOperationException("ExpressionNode is not initialized.");
            }
        }
    }

    internal class VariableAssignmentNode : ASTNode
    {
        public string _varName { get; }
        public string _literal { get; }
        public string _literalType { get; }

        public VariableAssignmentNode(string varName, string literal, string literalType)
        {
            _varName = varName;
            _literal = literal;
            _literalType = literalType;
            name = "VarAssign1";
        }

        public override string ToString()
        {
            return $"Variable Name: {_varName}, Value: {_literal}, Literal Type: {_literalType}";
        }

    }

    internal abstract class DisplayNode : ASTNode
    {
        public abstract string eval(SymbolStorage symbolStorage);

        public DisplayNode() {
            name = "Display";
        }
    }

    internal class DisplayConcatNode : DisplayNode
    {
        public DisplayNode _left;
        public DisplayNode _right;

        public DisplayConcatNode(DisplayNode left, DisplayNode right)
        {
            _left = left;
            _right = right;
            name = "DisplayConcat";
        }

        public override string eval(SymbolStorage symbolStorage)
        {
            string check = _left.eval(symbolStorage) + _right.eval(symbolStorage);
            return _left.eval(symbolStorage) + _right.eval(symbolStorage);
        }
    }

    internal class DisplayVariableNode : DisplayNode
    {
        public string _varName;

        public DisplayVariableNode(string varName)
        {
            _varName = varName;
            name = "DisplayVar";
        }
        public override string eval(SymbolStorage symbolStorage)
        {
            return "" + symbolStorage.findVariableToString(_varName);
        }
    }

    internal class DisplayStringNode : DisplayNode
    {
        public string _value;

        public DisplayStringNode(string value)
        {
            _value = value;
            name = "DisplayString";
        }
        public override string eval(SymbolStorage symbolStorage)
        {
            return _value;
        }
    }

    internal abstract class ExpressionNode : ASTNode
    {
        public abstract dynamic eval(SymbolStorage symbolStorage);
    }

    internal class ExpressionBinary : ExpressionNode
    {
        public ExpressionNode _left;
        public ExpressionNode _right;
        public string _binaryOperator;

        public ExpressionBinary(ExpressionNode left, ExpressionNode right, string binaryOperator)
        {
            _left = left;
            _right = right;
            _binaryOperator = binaryOperator;
            name = "ExpressionBinary";
        }

        public override dynamic eval(SymbolStorage symbolStorage)
        {
            dynamic result = 0;
            switch (_binaryOperator)
            {
                case "-":
                    //Subtraction
                    result = _left.eval(symbolStorage) - _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "-" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "+":
                    //Addition
                    result = _left.eval(symbolStorage) + _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "+" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "/":
                    //Divide
                    result = _left.eval(symbolStorage) / _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "/" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "*":
                    //Multiplication
                    result = _left.eval(symbolStorage) * _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "*" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "%":
                    //Modulo
                    result = _left.eval(symbolStorage) % _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "%" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "==":
                    //Is Equal
                    result = _left.eval(symbolStorage) == _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "==" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case ">":
                    //Greater than 
                    result = _left.eval(symbolStorage) > _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + ">" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "<":
                    //Less than
                    result = _left.eval(symbolStorage) < _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "<" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case ">=":
                    //Greater than or equal
                    result = _left.eval(symbolStorage) >= _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + ">=" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "<=":
                    //Less than or equal
                    result = _left.eval(symbolStorage) <= _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "<=" + _right.eval(symbolStorage) + "=" + result);
                    break;
                case "<>":
                    //Not equal
                    result = _left.eval(symbolStorage) != _right.eval(symbolStorage);
                    Console.WriteLine(_left.eval(symbolStorage) + "<>" + _right.eval(symbolStorage) + "=" + result);
                    break;
            }
            return result;
        }
    }

    internal class ExpressionVariable : ExpressionNode
    {
        public string _varName;

        public ExpressionVariable(string varName)
        {
            _varName = varName;
            name = "ExpressionVar";
        }

        public override dynamic eval(SymbolStorage symbolStorage)
        {
            return symbolStorage.findVariableToExpression(_varName);
        }
    }

    internal class ExpressionLiteral : ExpressionNode
    {
        public string _literal;
        public string _literalType;

        public ExpressionLiteral(string literal, string literalType)
        {
            _literal = literal;
            _literalType = literalType;
            name = "ExpressionLiteral";
        }

        public override dynamic eval(SymbolStorage symbolStorage)
        {
            dynamic result = null;
            switch (_literalType)
            {
                case "LETTER":
                    result = _literal[0];
                    break;
                case "NUMBER":
                    result = int.Parse(_literal);
                    break;
                case "DECIMAL_NUMBER":
                    result = float.Parse(_literal);
                    break;
                case "TRUE":
                    result = true;
                    break;
                case "FALSE":
                    result = false;
                    break;
                case "IDENTIFIER":
                    result = symbolStorage.findVariableToExpression(_literal);
                    break;
            }
            return result;
        }
    }

    /*internal class DisplayStatementsNode : DisplayNode
    {
        public readonly List<DisplayNode> _displayNodes;

        public DisplayStatementsNode(List<DisplayNode> displayNodes)
        {
            _displayNodes = displayNodes;
        }

        public override string eval(SymbolStorage symbolStorage) { return ""; }
    }*/


    /*internal class StringLiteralNode : ASTNode
    {
        public string Value { get; }

        public StringLiteralNode(string value)
        {
            Value = value;
        }
    }

    internal class NumberLiteralNode : ASTNode
    {
        public string Number { get; }

        public NumberLiteralNode(string number)
        {
            Number = number;
        }
    }*/

    internal class ScannedIdentifierNode : ASTNode
    {
        public string varName { get; }

        public ScannedIdentifierNode(string varName)
        {
            this.varName = varName;
            name = "ScannedIdentifier";
        }
    }

    /*internal abstract class ScanNode : ASTNode
    {
        public abstract void TempFunc();
    }*/

    internal class ScanStatementNode : ASTNode
    {
        public List<ASTNode> Scans { get; }

        public ScanStatementNode(List<ASTNode> scans)
        {
            Scans = scans;
            name = "ScannedStatement";
        }

    }

    internal class ConditionalNode : ASTNode
    {
        public ExpressionNode Condition { get; set; }
        public List<ASTNode> IfStatements { get; set; }
        public ConditionalNode ElseStatements { get; set; }
        public bool isAlwaysTrue = false;

        public ConditionalNode(ExpressionNode condition, List<ASTNode> ifStatements, ConditionalNode elseStatements)
        {
            Condition = condition;
            IfStatements = ifStatements;
            ElseStatements = elseStatements;
            name = "Conditional";
        }
    }

    internal class LoopNode : ASTNode
    {
        public ExpressionNode LoopCondition { get; set; }
        public List<ASTNode> LoopStatements { get; set; }
        public LoopNode NestedLoop { get; set; }

        public LoopNode(ExpressionNode loopCondition, List<ASTNode> loopStatements, LoopNode nestedLoop)
        {
            LoopCondition = loopCondition;
            LoopStatements = loopStatements;
            NestedLoop = nestedLoop;
            name = "Loop";
        }
    }

}

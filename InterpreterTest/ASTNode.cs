using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal abstract class ASTNode { }

    internal class ProgramNode : ASTNode
    {
        public List<ASTNode> Statements { get; }

        public ProgramNode(List<ASTNode> statements)
        {
            Statements = statements;
        }
    }

    internal class VariableDeclarationNode : ASTNode
    {
        public String _dataType { get; }
        public String _varName { get; }

        public VariableDeclarationNode(string dataType, string varName)
        {
            _dataType = dataType;
            _varName = varName;
        }

        public override String ToString()
        {
            return $"Data Type: {_dataType}, Variable Name: {_varName}";
        }

    }

    internal class VariableAssignmentNode : ASTNode
    {
        public String _varName { get; }
        public String _literal { get; }
        public String _literalType { get; }

        public VariableAssignmentNode(string varName, string literal, string literalType)
        {
            _varName = varName;
            _literal = literal;
            _literalType = literalType;
        }

        public override String ToString()
        {
            return $"Variable Name: {_varName}, Value: {_literal}, Literal Type: {_literalType}";
        }

    }

    internal abstract class DisplayNode : ASTNode
    {
        public abstract string eval(SymbolStorage symbolStorage);
    }

    internal class DisplayConcatNode : DisplayNode
    {
        public DisplayNode _left;
        public DisplayNode _right;

        public DisplayConcatNode(DisplayNode left, DisplayNode right)
        {
            _left = left;
            _right = right;
        }

        public override string eval(SymbolStorage symbolStorage)
        {
            return _left.eval(symbolStorage) + _right.eval(symbolStorage);
        }
    }

    internal class DisplayVariableNode : DisplayNode
    {
        public string _varName;

        public DisplayVariableNode(string varName)
        {
            _varName = varName;
        }
        public override string eval(SymbolStorage symbolStorage)
        {
            return symbolStorage.findVariable(_varName);
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
        }
    }

    internal class ScanStatementNode : ASTNode
    {
        public List<ASTNode> Scans { get; }

        public ScanStatementNode(List<ASTNode> scans)
        {
            Scans = scans;
        }

    }
}

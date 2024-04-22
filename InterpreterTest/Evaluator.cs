using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace InterpreterTest
{
    internal class Evaluator
    {
        private readonly List<ASTNode> _ast;
        private int _position;
        private SymbolStorage _symbolStorage;
        private Form1 _form1;

        public Evaluator(ProgramNode ast, SymbolStorage symbolStorage, Form1 form1)
        {
            _ast = ast.Statements;
            _symbolStorage = symbolStorage;
            _position = 0;
            _form1 = form1;
        }

        public string Evaluate()
        {
            string result = "";
            while (_position < _ast.Count)
            {
                ASTNode currNode = _ast[_position];
                if (currNode is VariableDeclarationNode variableDeclarationNode)
                {
                    HandleVariableDeclaration(variableDeclarationNode);
                }
                else if (currNode is VariableAssignmentNode2 variableAssignmentNode)
                {
                    HandleVariableAssignment(variableAssignmentNode);
                }
                else if (currNode is DisplayNode displayNode)
                {
                    result += HandleDisplayNode(displayNode);
                }
                else if (currNode is ScanStatementNode scanNode)
                {
                    HandleScanStatement(scanNode);
                }
                else if (currNode is ConditionalNode conditionalNode)
                {
                    bool conditionResult = conditionalNode.Condition.eval(_symbolStorage);

                    if (conditionResult)
                    {
                        EvaluateConditionalStatements(conditionalNode.IfStatements);
                    }
                    else
                    {
                        EvaluateConditionalStatements(conditionalNode.ElseStatements);
                    }
                }

                _position++;
            }
            return result;
        }

        // Helper method to handle variable declaration
        private void HandleVariableDeclaration(VariableDeclarationNode variableDeclarationNode)
        {
            string dataType = variableDeclarationNode._dataType;
            string varName = variableDeclarationNode._varName;
            switch (dataType)
            {
                case "INT":
                    _symbolStorage.INT.Add(varName, 0);
                    Console.WriteLine("Added " + varName + " as INT variable");
                    break;
                case "FLOAT":
                    _symbolStorage.FLOAT.Add(varName, 0.0f);
                    Console.WriteLine("Added " + varName + " as FLOAT variable");
                    break;
                case "BOOL":
                    _symbolStorage.BOOL.Add(varName, false);
                    Console.WriteLine("Added " + varName + " as BOOL variable");
                    break;
                case "CHAR":
                    _symbolStorage.CHAR.Add(varName, '\0');
                    Console.WriteLine("Added " + varName + " as CHAR variable");
                    break;
            }
        }

        // Helper method to handle variable assignment
        private void HandleVariableAssignment(VariableAssignmentNode2 variableAssignmentNode)
        {
            Console.WriteLine(variableAssignmentNode);
            variableAssignmentNode.eval(_symbolStorage);
        }

        // Helper method to handle display statement
        private string HandleDisplayNode(DisplayNode displayNode)
        {
            Console.WriteLine(displayNode);
            return displayNode.eval(_symbolStorage);
        }

        // Helper method to handle scan statement
        private void HandleScanStatement(ScanStatementNode scanNode)
        {
            int currentInput = 0;

            Console.WriteLine("Input: ");
            string input = Console.ReadLine();

            string[] inputValues = input.Split(',');

            if (inputValues.Length != scanNode.Scans.Count)
            {
                throw new InvalidOperationException(
                    $"Error: Expected {scanNode.Scans.Count} values, but received {inputValues.Length}");
            }

            foreach (ASTNode scanItem in scanNode.Scans)
            {
                if (scanItem is ScannedIdentifierNode identifierNode)
                {
                    //Console.WriteLine($"Enter value for {identifierNode.varName}: ");
                    _symbolStorage.SetValue(identifierNode.varName, inputValues[currentInput].Trim());
                    currentInput++;
                }
            }
        }

        // Helper method to evaluate conditional statements
        private void EvaluateConditionalStatements(List<ASTNode> statements)
        {
            foreach (ASTNode statement in statements)
            {
                if (statement is ConditionalNode conditionalNode)
                {
                    // Recursively evaluate nested conditional statements
                    EvaluateConditionalStatements(conditionalNode.IfStatements);
                    EvaluateConditionalStatements(conditionalNode.ElseStatements);
                }
                else
                {
                    // Handle other types of statements
                    if (statement is VariableDeclarationNode variableDeclarationNode)
                    {
                        HandleVariableDeclaration(variableDeclarationNode);
                    }
                    else if (statement is VariableAssignmentNode2 variableAssignmentNode)
                    {
                        HandleVariableAssignment(variableAssignmentNode);
                    }
                    else if (statement is DisplayNode displayNode)
                    {
                        HandleDisplayNode(displayNode);
                    }
                    else if (statement is ScanStatementNode scanNode)
                    {
                        HandleScanStatement(scanNode);
                    }
                }
            }
        }
    }
}
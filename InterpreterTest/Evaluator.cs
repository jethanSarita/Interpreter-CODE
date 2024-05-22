using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Diagnostics;

namespace InterpreterTest
{
    internal class Evaluator
    {
        private readonly List<ASTNode> _ast;
        private int _position;
        private SymbolStorage _symbolStorage;
        private Form1 _form1;
        private String result = "";

        public Evaluator(ProgramNode ast, SymbolStorage symbolStorage, Form1 form1)
        {
            _ast = ast.Statements;
            _symbolStorage = symbolStorage;
            _position = 0;
            _form1 = form1;
        }

        public string Evaluate()
        {
            long startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            result = "";

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
                    HandleDisplayNode(displayNode);
                }
                else if (currNode is ScanStatementNode scanNode)
                {
                    HandleScanStatement(scanNode);
                }
                else if (currNode is ConditionalNode conditionalNode)
                {
                    EvaluateConditional(conditionalNode);
                }
                else if (currNode is LoopNode loopNode)
                {
                    EvaluateLoop(loopNode);
                }

                _position++;
            }

            long endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            long runtime = endTime - startTime;
            string approx = runtime == 0 ? "~" : string.Empty;

            if (result == "")
            {
                return $"Program ran for {approx}{runtime} ms\nNo Error";
            }

            return $"Program ran for {approx}{runtime} ms\n{result}";
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
        private void HandleDisplayNode(DisplayNode displayNode)
        {
            Console.WriteLine(displayNode);
            result += displayNode.eval(_symbolStorage);
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

        // Helper method for evaluating conditionals
        private void EvaluateConditional(ConditionalNode conditional)
        {
            bool conditionResult = conditional.isAlwaysTrue || conditional.Condition.eval(_symbolStorage);

            if (conditionResult)
            {
                EvaluateConditionalStatements(conditional.IfStatements);
            }
            else
            {
                EvaluateConditional(conditional.ElseStatements);
            }
        }

        // Helper method for evaluating conditional statements
        private void EvaluateConditionalStatements(List<ASTNode> statements)
        {
            foreach (ASTNode statement in statements)
            {
                DebugTings.print(statement);
                Console.WriteLine("evaluating statement in conditional " + statement.name);
                if (statement is ConditionalNode conditionalNode)
                {
                    EvaluateConditionalStatements(conditionalNode.IfStatements);
                    EvaluateConditionalStatements(conditionalNode.ElseStatements.IfStatements);
                }
                else
                {
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
                        Console.WriteLine("no error encountered before - display");
                        _symbolStorage.PrintAll();
                        HandleDisplayNode(displayNode);
                        Console.WriteLine("no error encountered after - display");
                    }
                    else if (statement is ScanStatementNode scanNode)
                    {
                        HandleScanStatement(scanNode);
                    }
                }
            }
        }

        private void EvaluateLoop(LoopNode loop)
        {
            while (loop.LoopCondition.eval(_symbolStorage))
            {
                EvaluateLoopStatements(loop.LoopStatements);
            }
        }

        private void EvaluateLoopStatements(List<ASTNode> statements)
        {
            foreach (ASTNode statement in statements)
            {
                DebugTings.print(statement);
                Console.WriteLine("evaluating statement in conditional " + statement.name);
                if (statement is LoopNode loopNode)
                {
                    EvaluateLoopStatements(loopNode.LoopStatements);
                }
                else
                {
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
                        Console.WriteLine("no error encountered before - display");
                        _symbolStorage.PrintAll();
                        HandleDisplayNode(displayNode);
                        Console.WriteLine("no error encountered after - display");
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
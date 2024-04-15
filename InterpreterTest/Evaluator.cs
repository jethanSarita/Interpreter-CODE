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
                    _position++;
                }
                else if (currNode is VariableAssignmentNode2 variableAssignmentNode)
                {
                    Console.WriteLine(variableAssignmentNode);
                    variableAssignmentNode.eval(_symbolStorage);
                    _position++;
                }
                else if (currNode is DisplayNode displayNode)
                {
                    Console.WriteLine(displayNode);
                    result += displayNode.eval(_symbolStorage);
                    _position++;
                }
                else if (currNode is ScanStatementNode scanNode)
                {
                    foreach(ASTNode scanItem  in scanNode.Scans)
                    {
                        if(scanItem is ScannedIdentifierNode identifierNode)
                        {
                            //Console.WriteLine($"Enter value for {identifierNode.varName}: ");
                            string input = _form1.getText();

                            string[] inputValues = input.Split(',');

                            if(inputValues.Length != scanNode.Scans.Count)
                            {
                                throw new InvalidOperationException(
                                    $"Error: Expected {scanNode.Scans.Count} values, but received {inputValues.Length}");
                            }

                            for(int i = 0; i < scanNode.Scans.Count; i++)
                            {
                                _symbolStorage.setValue(identifierNode.varName, inputValues[i].Trim());
                            }
                        }
                    }                                    
                }
            }
            return result;
        }
    }
}

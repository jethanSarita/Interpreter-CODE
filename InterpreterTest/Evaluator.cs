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

        public Evaluator(ProgramNode ast, SymbolStorage symbolStorage)
        {
            _ast = ast.Statements;
            _symbolStorage = symbolStorage;
            _position = 0;
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
                else if (currNode is VariableAssignmentNode variableAssignmentNode)
                {
                    
                    string varName = variableAssignmentNode._varName;
                    string literal = variableAssignmentNode._literal;
                    string literalType = variableAssignmentNode._literalType;
                    switch (literalType)
                    {
                        case "NUMBER":
                            //string to int
                            _symbolStorage.INT[varName] = int.Parse(literal);
                            Console.WriteLine("Added INT " + literal + " to " + varName);
                            break;
                        case "LETTER":
                            //string to char
                            _symbolStorage.CHAR[varName] = literal[0];
                            Console.WriteLine("Added CHAR " + literal + " to " + varName);
                            break;
                        case "TRUE":
                            _symbolStorage.BOOL[varName] = true;
                            Console.WriteLine("Added BOOL " + literal + " to " + varName);
                            break;
                        case "FALSE":
                            _symbolStorage.BOOL[varName] = false;
                            Console.WriteLine("Added BOOL " + literal + " to " + varName);
                            break;
                        case "DECIMAL_NUMBER":
                            //string to float
                            _symbolStorage.FLOAT[varName] = float.Parse(literal);
                            Console.WriteLine("Added FLOAT " + literal + " to " + varName);
                            break;
                    }
                    _position++;
                }
                else if (currNode is DisplayNode displayNode)
                {
                    result = displayNode.eval(_symbolStorage);
                    _position++;
                }
            }
            return result;
        }
    }
}

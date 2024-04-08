using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class Evaluator
    {
        private readonly List<ASTNode> _ast;
        private int _position;

        private Dictionary<string, int> INT = new Dictionary<string, int>();
        private Dictionary<string, float> FLOAT = new Dictionary<string, float>();
        private Dictionary<string, bool> BOOL = new Dictionary<string, bool>();
        private Dictionary<string, char> CHAR = new Dictionary<string, char>();

        public Evaluator(ProgramNode ast)
        {
            _ast = ast.Statements;
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
                            INT.Add(varName, 0);
                            Console.WriteLine("Added " + varName + " as INT variable");
                            break;
                        case "FLOAT":
                            FLOAT.Add(varName, 0.0f);
                            Console.WriteLine("Added " + varName + " as FLOAT variable");
                            break;
                        case "BOOL":
                            BOOL.Add(varName, false);
                            Console.WriteLine("Added " + varName + " as BOOL variable");
                            break;
                        case "CHAR":
                            CHAR.Add(varName, '\0');
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
                            INT[varName] = int.Parse(literal);
                            Console.WriteLine("Added INT " + literal + " to " + varName);
                            break;
                        case "LETTER":
                            //string to char
                            CHAR[varName] = literal[0];
                            Console.WriteLine("Added CHAR " + literal + " to " + varName);
                            break;
                        case "TRUE":
                            BOOL[varName] = true;
                            Console.WriteLine("Added BOOL " + literal + " to " + varName);
                            break;
                        case "FALSE":
                            BOOL[varName] = false;
                            Console.WriteLine("Added BOOL " + literal + " to " + varName);
                            break;
                        case "DECIMAL_NUMBER":
                            //string to float
                            FLOAT[varName] = float.Parse(literal);
                            Console.WriteLine("Added FLOAT " + literal + " to " + varName);
                            break;
                    }
                    _position++;
                }
                else if (currNode is DisplayStatementNode displayStatementNode)
                {
                    
                    List<ASTNode> dispayNodes = displayStatementNode._displayNodes;
                    foreach (ASTNode displayNode in dispayNodes)
                    {
                        if (displayNode is DisplayVariableNode displayVariableNode)
                        {
                            result += INT[displayVariableNode._varName];
                        }
                    }
                    _position++;
                }
            }
            return result;
        }
    }
}

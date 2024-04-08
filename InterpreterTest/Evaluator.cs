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
                            break;
                        case "FLOAT":
                            FLOAT.Add(varName, 0.0f);
                            break;
                        case "BOOL":
                            BOOL.Add(varName, false);
                            break;
                        case "CHAR":
                            CHAR.Add(varName, '\0');
                            break;
                    }
                }
                else if (currNode is VariableAssignmentNode variableAssignmentNode)
                {
                    string varName = variableAssignmentNode._varName;
                    string literal = variableAssignmentNode._literal;
                    string literalType = variableAssignmentNode._literalType;

                    switch (literalType)
                    {
                        case "NUMBER":
                            INT[varName] = int.Parse(literal);
                            break;
                        case "LETTER":
                            CHAR[varName] = literal[0];
                            break;
                        case "TRUE":
                            BOOL[varName] = true;
                            break;
                        case "FALSE":
                            BOOL[varName] = false;
                            break;
                        case "DECIMAL_NUMBER":
                            FLOAT[varName] = float.Parse(literal);
                            break;
                    }
                }
            }
            return "";
        }
    }
}

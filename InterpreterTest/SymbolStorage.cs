using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class SymbolStorage
    {
        public Dictionary<string, int> INT;
        public Dictionary<string, float> FLOAT;
        public Dictionary<string, bool> BOOL;
        public Dictionary<string, char> CHAR;

        public SymbolStorage()
        {
            INT = new Dictionary<string, int>();
            FLOAT = new Dictionary<string, float>();
            BOOL = new Dictionary<string, bool>();
            CHAR = new Dictionary<string, char>();
        }

        internal dynamic findVariableToString(string varName)
        {
            dynamic result;
            if(INT.ContainsKey(varName))
            {
                result = INT[varName];
            }else if (FLOAT.ContainsKey(varName))
            {
                result = FLOAT[varName];
            }
            else if (BOOL.ContainsKey(varName))
            {
                if (BOOL[varName])
                {
                    result = "TRUE";
                }
                else
                {
                    result = "FALSE";
                }
            }
            else if (CHAR.ContainsKey(varName))
            {
                result = CHAR[varName];
            }
            else
            {
                throw new InvalidOperationException($"{varName} doesn't exist");
            }
            return result;
        }

        public bool IsVariable(string varName)
        {
            bool result = false;
            if (INT.ContainsKey(varName))
            {
                result = true;
            }
            else if (FLOAT.ContainsKey(varName))
            {
                result = true;
            }
            else if (BOOL.ContainsKey(varName))
            {
                result = true;
            }
            else if (CHAR.ContainsKey(varName))
            {
                result = true;
            }
            return result;
        }

        public void AssignVariable(string varName, dynamic literal)
        {
            if (IsVariable(varName))
            {
                if (literal is int)
                {
                    INT[varName] = literal;
                }
                else if (literal is float)
                {
                    FLOAT[varName] = literal;
                }
                else if (literal is char)
                {
                    CHAR[varName] = literal;
                }
                else if (literal is bool)
                {
                    BOOL[varName] = literal;
                }
            }
        }

        public void SetValue(string varName, string value)
        {
            if (!IsVariable(varName))
            {
                throw new InvalidOperationException($"Variable {varName} doesn't exist.");
            }

            if (INT.ContainsKey(varName))
            {
                if (int.TryParse(value, out int intValue))
                {
                    INT[varName] = intValue;
                }
                else
                {
                    throw new ArgumentException("Value must be of type INT");
                }
            }
            else if (FLOAT.ContainsKey(varName))
            {
                if (float.TryParse(value, out float floatValue))
                {
                    FLOAT[varName] = floatValue;
                }
                else
                {
                    throw new ArgumentException("Value must be of type FLOAT");
                }
            }
            else if (CHAR.ContainsKey(varName))
            {
                if (char.TryParse(value, out char charValue))
                {
                    CHAR[varName] = charValue;
                }
                else
                {
                    throw new ArgumentException("Value must be of type CHAR");
                }
            }
            else if (BOOL.ContainsKey(varName))
            {
                if (bool.TryParse(value, out bool boolValue))
                {
                    BOOL[varName] = boolValue;
                }
                else
                {
                    throw new ArgumentException("Value must be of type BOOL");
                }
            }
            else
            {
                throw new InvalidOperationException($"{varName} doesn't exist");
            }
        }

        public dynamic findVariableToExpression(string varName)
        {
            dynamic result;
            if (INT.ContainsKey(varName))
            {
                result = INT[varName];
            }
            else if (FLOAT.ContainsKey(varName))
            {
                result = FLOAT[varName];
            }
            else if (BOOL.ContainsKey(varName))
            {
                if (BOOL[varName])
                {
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            else if (CHAR.ContainsKey(varName))
            {
                result = CHAR[varName];
            }
            else
            {
                throw new InvalidOperationException($"{varName} doesn't exist");
            }
            return result;
        }

        public void PrintAll()
        {
            Console.WriteLine("ints: ");
            foreach (KeyValuePair<string, int> kvp in INT)
            {
                Console.WriteLine("\t" + kvp.Key + " = " + kvp.Value);
            }
            Console.WriteLine("floats: ");
            foreach (KeyValuePair<string, float> kvp in FLOAT)
            {
                Console.WriteLine("\t" + kvp.Key + " = " + kvp.Value);
            }
            Console.WriteLine("bools: ");
            foreach (KeyValuePair<string, bool> kvp in BOOL)
            {
                Console.WriteLine("\t" + kvp.Key + " = " + kvp.Value);
            }
            Console.WriteLine("chars: ");
            foreach (KeyValuePair<string, char> kvp in CHAR)
            {
                Console.WriteLine("\t" + kvp.Key + " = " + kvp.Value);
            }
        }
    }
}
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

        internal string findVariable(string varName)
        {
            string result = "";
            if(INT.ContainsKey(varName))
            {
                result += INT[varName];
            }else if (FLOAT.ContainsKey(varName))
            {
                result += FLOAT[varName];
            }
            else if (BOOL.ContainsKey(varName))
            {
                if (BOOL[varName])
                {
                    result += "TRUE";
                }
                else
                {
                    result += "FALSE";
                }
            }
            else if (CHAR.ContainsKey(varName))
            {
                result += CHAR[varName];
            }
            else
            {
                throw new InvalidOperationException($"{varName} doesn't exist");
            }
            return result;
        }

        public bool CheckVariable(string varName)
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
    }
}
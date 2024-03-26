using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterpreterTest
{
    internal class Interpreter
    {
        private Dictionary<string, dynamic> variables;

        public Interpreter()
        {
            variables = new Dictionary<string, dynamic>();
        }

        public void Interpret(string code)
        {
            string[] lines = code.Split('\n');
            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("#"))
                {
                    //End case
                    if (trimmedLine == "END CODE")
                    {
                        break;
                    }
                    // Initiator 
                    else if (trimmedLine.StartsWith("INT") || trimmedLine.StartsWith("FLOAT") || trimmedLine.StartsWith("CHAR") || trimmedLine.StartsWith("BOOL"))
                    {
                        ProcessVariableDeclaration(trimmedLine);
                    }
                    else if (trimmedLine.StartsWith("Display:"))
                    {
                        //ProcessDisplayStatement(trimmedLine);
                    }
                    else
                    {
                        //ProcessStatement(trimmedLine);
                    }
                }
            }
        }

        private void ProcessVariableDeclaration(string line)
        {
            string[] parts = line.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
            bool hasEqual = false;
            string[] variableNames = new string[parts.Length - 2];
            //string dataType = parts[0];
            if (parts.Length >= 2 && parts[parts.Length - 2] ==  "=")
            {
                //there is an equals declration
                hasEqual = true;
                Array.Copy(parts, variableNames, parts.Length - 2);
            }
            else
            {
                Array.Copy(parts, variableNames, parts.Length);
            }
            for (int i = 1; i < variableNames.Length; i++)
            {
                string variableName = variableNames[i];
                if (!variables.ContainsKey(variableName))
                {
                    string dataType = variableNames[0];
                    switch (dataType)
                    {
                        case "INT":
                            variables[variableName] = 0;
                            if (hasEqual)
                            {

                            }
                            break;
                        case "FLOAT":
                            variables[variableName] = 0.0f;
                            break;
                        case "CHAR":
                            variables[variableName] = ' ';
                            break;
                        case "BOOL":
                            variables[variableName] = false;
                            break;
                        default:
                            throw new Exception("Unsupported data type");
                    }
                }
                else
                {
                    throw new Exception($"Variable '{variableName}' is already declared.");
                }
            }
        }
    }
}

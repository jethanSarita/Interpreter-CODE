using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterpreterTest
{
    internal class DebugTings
    {
        static public void print(ASTNode tree, int tab = 0) {
            for (int i = 0; i <= tab; i++) {
                Console.Write("\t");
            }
            Console.Write(tree.name);
            if (tree is ProgramNode)
            {
                ProgramNode node = (ProgramNode)tree;
                foreach (ASTNode statement in node.Statements)
                {
                    DebugTings.print(statement, tab + 1);
                }
            }
            else if (tree is ExpressionBinary bin)
            {
                Console.Write(" " + bin._binaryOperator);
                DebugTings.print(bin._left, tab + 1);
                DebugTings.print(bin._right, tab + 1);
            }
            Console.Write("\n");
        }
    }
}

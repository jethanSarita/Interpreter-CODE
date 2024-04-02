using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InterpreterTest
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        // click button to see if lexer and parser are working fine
        private void button1_Click(object sender, EventArgs e)
        {
            string source = tbInput.Text;
            var lexer = new Lexer(source);
            List<Token> tokens = lexer.Tokenize();

            if (tokens.Count > 0)
            {
                foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }
                lblOutput.Text = "the lexer is lexing";
            }
            else
            {
                lblOutput.Text = "Lexer encountered an error.";
                return;
            }

            Parser parser = new Parser(tokens);
            ProgramNode ast = parser.Parse();

            foreach (ASTNode node in ast.Statements)
            {
                if (node is VariableDeclarationNode variableNode)
                {
                    Console.WriteLine("VariableNode: " + variableNode.ToString());
                }
            }

            /*Parser parser = new Parser(tokens);
            ASTNode ast = null;
            try
            {
                ast = parser.Parse();
                lblOutput2.Text = "the parser is parsing";
            }
            catch (Exception ex)
            {
                lblOutput2.Text = "Parser encountered an error: " + ex.Message;
                return;
            }

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }*/

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblOutput.ResetText();
            lblOutput2.ResetText();
            tbInput.ResetText();
        }

        private void lblOutput_Click(object sender, EventArgs e)
        {
            //dont change
        }

        private void lblOutput2_Click(object sender, EventArgs e)
        {
            //dont change
        }
    }
}

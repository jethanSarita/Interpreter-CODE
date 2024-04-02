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
                /*foreach (var token in tokens)
                {
                    Console.WriteLine(token);
                }*/
                Console.WriteLine("0: " + tokens[0]);
                Console.WriteLine("1: " + tokens[1]);
                Console.WriteLine("2: " + tokens[2]);
                lblOutput.Text = "the lexer is lexing";
            }
            else
            {
                lblOutput.Text = "Lexer encountered an error.";
                return;
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
    }
}

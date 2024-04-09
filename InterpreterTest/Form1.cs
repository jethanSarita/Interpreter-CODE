using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

            Console.WriteLine(source);

            try
            {
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
                lblOutput2.Text = "the parser is parsing";

                //troubleshooting
                foreach (ASTNode node in ast.Statements)
                {
                    if (node is VariableDeclarationNode variableNode)
                    {
                        Console.WriteLine("variableNode: " + variableNode.ToString());
                    }
                    else if (node is DisplayNode displanyNode)
                    {
                        Console.WriteLine("displayNode: " + displanyNode.ToString());
                    }
                    else if (node is VariableAssignmentNode assignmentNode)
                    {
                        Console.WriteLine("assignmentNode: " + assignmentNode.ToString());
                    }
                }

                SymbolStorage symbolStorage = new SymbolStorage();

                Evaluator eval = new Evaluator(ast, symbolStorage);
                string result = eval.Evaluate();
                Console.WriteLine("Result: " + result);
                tbOutput.RichTextBox.Text = result;
            }
            catch (Exception ex)
            {
                tbOutput.RichTextBox.Text = "Encountered an error:" + Environment.NewLine + ex.Message;
            }
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

        private void tbInput_TextChanged(object sender, EventArgs e)
        {
            //dont change
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            tbOutput.RichTextBox.BackColor = ColorTranslator.FromHtml("#222E33");
            tbOutput.RichTextBox.ForeColor = Color.White;
            tbOutput.RichTextBox.Font = new Font("Tahoma", 12, FontStyle.Regular);
            tbOutput.RichTextBox.BorderStyle = BorderStyle.None;
            tbOutput.BorderStyle = BorderStyle.None;
        }
    }
}

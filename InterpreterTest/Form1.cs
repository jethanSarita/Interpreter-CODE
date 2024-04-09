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
            string source = tbInput.RichTextBox.Text;

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
                    pictureBox1.Image = InterpreterTest.Properties.Resources.Coconut;
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
            catch (Exception exception)
            {
                string text = "";
                if (exception is StackOverflowException ashh)
                {
                    text += ashh.Message;
                }
                else
                {
                    text += "Encountered an error:" + Environment.NewLine;
                    text += exception.Message;
                }

                tbOutput.RichTextBox.Text = text;

                pictureBox1.Image = InterpreterTest.Properties.Resources.Papaya;
                
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lblOutput.ResetText();
            lblOutput2.ResetText();
            tbInput.RichTextBox.ResetText();
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
            tbInput.RichTextBox.BackColor = ColorTranslator.FromHtml("#222E33");
            tbInput.RichTextBox.ForeColor = Color.White;
            tbInput.RichTextBox.Font = new Font("Tahoma", 12, FontStyle.Regular);
            tbInput.RichTextBox.BorderStyle = BorderStyle.None;
            tbInput.BorderStyle = BorderStyle.None;

            tbOutput.RichTextBox.BackColor = ColorTranslator.FromHtml("#222E33");
            tbOutput.RichTextBox.ForeColor = Color.White;
            tbOutput.RichTextBox.Font = new Font("Tahoma", 12, FontStyle.Regular);
            tbOutput.RichTextBox.BorderStyle = BorderStyle.None;
            tbOutput.BorderStyle = BorderStyle.None;
        }

        bool secretButtonPressed = false;
        bool secretButtonPressed2 = false;

        private void secretButton_Click(object sender, EventArgs e)
        {
            if (!secretButtonPressed)
            {
                if (secretButtonPressed2)
                {
                    System.Windows.Forms.MessageBox.Show("Visualizer is backkk \\o/");
                    secretButtonPressed = true;
                    pictureBox1.Visible = true;
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("You found the secret code error visualizer! yayy");
                    secretButtonPressed = true;
                    pictureBox1.Visible = true;
                }
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("T_T okay no more visualizer");
                secretButtonPressed = false;
                secretButtonPressed2 = true;
                pictureBox1.Visible = false;
            }
            
        }
    }
}

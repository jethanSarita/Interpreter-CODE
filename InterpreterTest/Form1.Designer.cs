namespace InterpreterTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnRun = new System.Windows.Forms.Button();
            this.lblOutput = new System.Windows.Forms.Label();
            this.lblOutput2 = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.secretButton = new System.Windows.Forms.Button();
            this.tbInput = new InterpreterTest.LineNumberRTB();
            this.tbOutput = new InterpreterTest.LineNumberRTB();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnRun
            // 
            this.btnRun.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRun.ForeColor = System.Drawing.Color.Black;
            this.btnRun.Location = new System.Drawing.Point(878, 20);
            this.btnRun.Margin = new System.Windows.Forms.Padding(4);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(102, 60);
            this.btnRun.TabIndex = 1;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.button1_Click);
            // 
            // lblOutput
            // 
            this.lblOutput.AutoSize = true;
            this.lblOutput.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutput.Location = new System.Drawing.Point(987, 20);
            this.lblOutput.Name = "lblOutput";
            this.lblOutput.Size = new System.Drawing.Size(166, 31);
            this.lblOutput.TabIndex = 2;
            this.lblOutput.Text = "Lexer Tester";
            this.lblOutput.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOutput.Click += new System.EventHandler(this.lblOutput_Click);
            // 
            // lblOutput2
            // 
            this.lblOutput2.AutoSize = true;
            this.lblOutput2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblOutput2.Location = new System.Drawing.Point(987, 88);
            this.lblOutput2.Name = "lblOutput2";
            this.lblOutput2.Size = new System.Drawing.Size(179, 31);
            this.lblOutput2.TabIndex = 3;
            this.lblOutput2.Text = "Parser Tester";
            this.lblOutput2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblOutput2.Click += new System.EventHandler(this.lblOutput2_Click);
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClear.ForeColor = System.Drawing.Color.Black;
            this.btnClear.Location = new System.Drawing.Point(878, 88);
            this.btnClear.Margin = new System.Windows.Forms.Padding(4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(102, 60);
            this.btnClear.TabIndex = 4;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // secretButton
            // 
            this.secretButton.Location = new System.Drawing.Point(1250, 20);
            this.secretButton.Name = "secretButton";
            this.secretButton.Size = new System.Drawing.Size(22, 22);
            this.secretButton.TabIndex = 8;
            this.secretButton.UseVisualStyleBackColor = true;
            this.secretButton.Click += new System.EventHandler(this.secretButton_Click);
            // 
            // tbInput
            // 
            this.tbInput.BackColor = System.Drawing.SystemColors.Window;
            this.tbInput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbInput.Location = new System.Drawing.Point(18, 20);
            this.tbInput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(848, 430);
            this.tbInput.TabIndex = 6;
            // 
            // tbOutput
            // 
            this.tbOutput.BackColor = System.Drawing.SystemColors.Window;
            this.tbOutput.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.tbOutput.Location = new System.Drawing.Point(18, 460);
            this.tbOutput.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tbOutput.Name = "tbOutput";
            this.tbOutput.Size = new System.Drawing.Size(848, 187);
            this.tbOutput.TabIndex = 5;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::InterpreterTest.Properties.Resources.Sleep;
            this.pictureBox1.Location = new System.Drawing.Point(878, 155);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(394, 295);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 21F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1284, 661);
            this.Controls.Add(this.secretButton);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.tbOutput);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblOutput2);
            this.Controls.Add(this.lblOutput);
            this.Controls.Add(this.btnRun);
            this.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.Black;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblOutput;
        private System.Windows.Forms.Label lblOutput2;
        private System.Windows.Forms.Button btnClear;
        private LineNumberRTB tbOutput;
        private LineNumberRTB tbInput;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button secretButton;
    }
}


namespace ToString
{
    partial class ToString
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Count = new Button();
            One = new Button();
            Two = new Button();
            Three = new Button();
            SuspendLayout();
            // 
            // Count
            // 
            Count.Location = new Point(287, 200);
            Count.Name = "Count";
            Count.Size = new Size(75, 23);
            Count.TabIndex = 0;
            Count.Text = "Count";
            Count.UseVisualStyleBackColor = true;
            Count.Click += Count_Click;
            // 
            // One
            // 
            One.Location = new Point(425, 162);
            One.Name = "One";
            One.Size = new Size(75, 23);
            One.TabIndex = 1;
            One.Text = "0";
            One.UseVisualStyleBackColor = true;
            // 
            // Two
            // 
            Two.Location = new Point(425, 200);
            Two.Name = "Two";
            Two.Size = new Size(75, 23);
            Two.TabIndex = 2;
            Two.Text = "0";
            Two.UseVisualStyleBackColor = true;
            // 
            // Three
            // 
            Three.Location = new Point(425, 229);
            Three.Name = "Three";
            Three.Size = new Size(75, 23);
            Three.TabIndex = 3;
            Three.Text = "0";
            Three.UseVisualStyleBackColor = true;
            // 
            // ToString
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Three);
            Controls.Add(Two);
            Controls.Add(One);
            Controls.Add(Count);
            Name = "ToString";
            Text = "Form1";
            ResumeLayout(false);
        }

        #endregion

        private Button Count;
        private Button One;
        private Button Two;
        private Button Three;
    }
}

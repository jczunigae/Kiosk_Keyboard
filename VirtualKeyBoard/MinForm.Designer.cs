namespace VirtualKeyBoard
{
    partial class MinForm
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
            this.keyboardBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // keyboardBtn
            // 
            this.keyboardBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold);
            this.keyboardBtn.Location = new System.Drawing.Point(1, 7);
            this.keyboardBtn.Margin = new System.Windows.Forms.Padding(4);
            this.keyboardBtn.Name = "keyboardBtn";
            this.keyboardBtn.Size = new System.Drawing.Size(127, 39);
            this.keyboardBtn.TabIndex = 0;
            this.keyboardBtn.Text = "Keyboard";
            this.keyboardBtn.UseVisualStyleBackColor = true;
            this.keyboardBtn.Click += new System.EventHandler(this.KeyboardBtn_Click);
            // 
            // MinForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(136, 44);
            this.Controls.Add(this.keyboardBtn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MinForm";
            this.Opacity = 0.85D;
            this.ShowInTaskbar = false;
            this.Text = "MinForm";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.MinForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button keyboardBtn;
    }
}
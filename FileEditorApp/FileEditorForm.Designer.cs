namespace FileEditorApp
{
    partial class FileEditorForm
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
            this.textBox = new System.Windows.Forms.TextBox();
            this.openButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // textBox
            // 
            this.textBox.AcceptsReturn = true;
            this.textBox.AcceptsTab = true;
            this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBox.Multiline = true;
            this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox.Location = new System.Drawing.Point(0, 60);
            this.textBox.Size = new System.Drawing.Size(584, 361);
            this.textBox.TabIndex = 0;
            // 
            // openButton
            // 
            this.openButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.openButton.Height = 30;
            this.openButton.Location = new System.Drawing.Point(0, 0);
            this.openButton.Size = new System.Drawing.Size(584, 30);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "Open File";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Dock = System.Windows.Forms.DockStyle.Top;
            this.saveButton.Height = 30;
            this.saveButton.Location = new System.Drawing.Point(0, 30);
            this.saveButton.Size = new System.Drawing.Size(584, 30);
            this.saveButton.TabIndex = 2;
            this.saveButton.Text = "Save File";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "HTML files (*.html)|*.html|Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "HTML files (*.html)|*.html|Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin|All files (*.*)|*.*";
            // 
            // FileEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 361);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.openButton);
            this.Text = "File Editor";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}
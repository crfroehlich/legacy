﻿namespace Nbt2DImporterTester
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
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtImportDataTableName = new System.Windows.Forms.TextBox();
            this.txtDataFilePath = new System.Windows.Forms.TextBox();
            this.txtBindingsFilePath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 52);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(90, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Load Data";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(80, 239);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(90, 27);
            this.button2.TabIndex = 1;
            this.button2.Text = "Run Import";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // txtImportDataTableName
            // 
            this.txtImportDataTableName.Location = new System.Drawing.Point(12, 213);
            this.txtImportDataTableName.Name = "txtImportDataTableName";
            this.txtImportDataTableName.Size = new System.Drawing.Size(260, 20);
            this.txtImportDataTableName.TabIndex = 2;
            // 
            // txtDataFilePath
            // 
            this.txtDataFilePath.Location = new System.Drawing.Point(12, 25);
            this.txtDataFilePath.Name = "txtDataFilePath";
            this.txtDataFilePath.Size = new System.Drawing.Size(260, 20);
            this.txtDataFilePath.TabIndex = 3;
            this.txtDataFilePath.Text = "z:\\D\\temp\\rapidloaderdata.xlsx";
            // 
            // txtBindingsFilePath
            // 
            this.txtBindingsFilePath.Location = new System.Drawing.Point(12, 115);
            this.txtBindingsFilePath.Name = "txtBindingsFilePath";
            this.txtBindingsFilePath.Size = new System.Drawing.Size(260, 20);
            this.txtBindingsFilePath.TabIndex = 4;
            this.txtBindingsFilePath.Text = "z:\\D\\temp\\rapidloaderbindings.xlsx";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Data File";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 99);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Bindings File";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 197);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(123, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Import Data Table Name";
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(282, 12);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(288, 254);
            this.txtLog.TabIndex = 8;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(80, 141);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(90, 27);
            this.button3.TabIndex = 9;
            this.button3.Text = "Read Bindings";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 278);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBindingsFilePath);
            this.Controls.Add(this.txtDataFilePath);
            this.Controls.Add(this.txtImportDataTableName);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtImportDataTableName;
        private System.Windows.Forms.TextBox txtDataFilePath;
        private System.Windows.Forms.TextBox txtBindingsFilePath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.Button button3;
    }
}


﻿namespace CryptoPad
{
    partial class frmCryptoInput
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCryptoInput));
            this.lblDesc = new System.Windows.Forms.Label();
            this.btnKeyfile = new System.Windows.Forms.Button();
            this.cbKeyfile = new System.Windows.Forms.CheckBox();
            this.cbPassword = new System.Windows.Forms.CheckBox();
            this.tbKeyfile = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.OFD = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cbRSA = new System.Windows.Forms.CheckBox();
            this.btnRSA = new System.Windows.Forms.Button();
            this.lblRSAName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblDesc
            // 
            this.lblDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDesc.Location = new System.Drawing.Point(12, 15);
            this.lblDesc.Name = "lblDesc";
            this.lblDesc.Size = new System.Drawing.Size(468, 23);
            this.lblDesc.TabIndex = 0;
            this.lblDesc.Text = "Please enter the required Password :";
            // 
            // btnKeyfile
            // 
            this.btnKeyfile.Location = new System.Drawing.Point(147, 169);
            this.btnKeyfile.Name = "btnKeyfile";
            this.btnKeyfile.Size = new System.Drawing.Size(75, 23);
            this.btnKeyfile.TabIndex = 3;
            this.btnKeyfile.Text = "&Browse...";
            this.btnKeyfile.UseVisualStyleBackColor = true;
            this.btnKeyfile.Visible = false;
            this.btnKeyfile.Click += new System.EventHandler(this.btnKeyfile_Click);
            // 
            // cbKeyfile
            // 
            this.cbKeyfile.AutoSize = true;
            this.cbKeyfile.Location = new System.Drawing.Point(12, 175);
            this.cbKeyfile.Name = "cbKeyfile";
            this.cbKeyfile.Size = new System.Drawing.Size(79, 17);
            this.cbKeyfile.TabIndex = 1;
            this.cbKeyfile.Text = "Use Keyfile";
            this.cbKeyfile.UseVisualStyleBackColor = true;
            this.cbKeyfile.Visible = false;
            // 
            // cbPassword
            // 
            this.cbPassword.AutoSize = true;
            this.cbPassword.Location = new System.Drawing.Point(26, 152);
            this.cbPassword.Name = "cbPassword";
            this.cbPassword.Size = new System.Drawing.Size(94, 17);
            this.cbPassword.TabIndex = 4;
            this.cbPassword.Text = "Use Password";
            this.cbPassword.UseVisualStyleBackColor = true;
            this.cbPassword.Visible = false;
            // 
            // tbKeyfile
            // 
            this.tbKeyfile.Location = new System.Drawing.Point(58, 169);
            this.tbKeyfile.Name = "tbKeyfile";
            this.tbKeyfile.Size = new System.Drawing.Size(273, 20);
            this.tbKeyfile.TabIndex = 2;
            this.tbKeyfile.Visible = false;
            this.tbKeyfile.TextChanged += new System.EventHandler(this.tbKeyfile_TextChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(196, 12);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(273, 20);
            this.tbPassword.TabIndex = 5;
            this.tbPassword.UseSystemPasswordChar = true;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(324, 44);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(405, 44);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // cbRSA
            // 
            this.cbRSA.AutoSize = true;
            this.cbRSA.Location = new System.Drawing.Point(26, 129);
            this.cbRSA.Name = "cbRSA";
            this.cbRSA.Size = new System.Drawing.Size(69, 17);
            this.cbRSA.TabIndex = 6;
            this.cbRSA.Text = "RSA Key";
            this.cbRSA.UseVisualStyleBackColor = true;
            this.cbRSA.Visible = false;
            // 
            // btnRSA
            // 
            this.btnRSA.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRSA.Location = new System.Drawing.Point(111, 175);
            this.btnRSA.Name = "btnRSA";
            this.btnRSA.Size = new System.Drawing.Size(75, 23);
            this.btnRSA.TabIndex = 8;
            this.btnRSA.Text = "&Select...";
            this.btnRSA.UseVisualStyleBackColor = true;
            this.btnRSA.Visible = false;
            this.btnRSA.Click += new System.EventHandler(this.btnRSA_Click);
            // 
            // lblRSAName
            // 
            this.lblRSAName.AutoSize = true;
            this.lblRSAName.Location = new System.Drawing.Point(123, 130);
            this.lblRSAName.Name = "lblRSAName";
            this.lblRSAName.Size = new System.Drawing.Size(99, 13);
            this.lblRSAName.TabIndex = 7;
            this.lblRSAName.Text = "<No Key Selected>";
            this.lblRSAName.Visible = false;
            // 
            // frmCryptoInput
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(492, 79);
            this.Controls.Add(this.lblRSAName);
            this.Controls.Add(this.btnRSA);
            this.Controls.Add(this.cbRSA);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.tbKeyfile);
            this.Controls.Add(this.cbPassword);
            this.Controls.Add(this.cbKeyfile);
            this.Controls.Add(this.btnKeyfile);
            this.Controls.Add(this.lblDesc);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmCryptoInput";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Decryption requires user information";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDesc;
        private System.Windows.Forms.Button btnKeyfile;
        private System.Windows.Forms.CheckBox cbKeyfile;
        private System.Windows.Forms.CheckBox cbPassword;
        private System.Windows.Forms.TextBox tbKeyfile;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.OpenFileDialog OFD;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckBox cbRSA;
        private System.Windows.Forms.Button btnRSA;
        private System.Windows.Forms.Label lblRSAName;
    }
}
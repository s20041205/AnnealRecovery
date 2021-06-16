namespace AnnealFileRecovery
{
    partial class frmFileRecovery
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
            this.btnFolderPath = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.btnRun = new System.Windows.Forms.Button();
            this.lblToRecover = new System.Windows.Forms.Label();
            this.lblPatnID = new System.Windows.Forms.Label();
            this.cmbAdmit = new System.Windows.Forms.ComboBox();
            this.lblAdmit = new System.Windows.Forms.Label();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // btnFolderPath
            // 
            this.btnFolderPath.Location = new System.Drawing.Point(286, 24);
            this.btnFolderPath.Name = "btnFolderPath";
            this.btnFolderPath.Size = new System.Drawing.Size(31, 23);
            this.btnFolderPath.TabIndex = 0;
            this.btnFolderPath.Text = "...";
            this.btnFolderPath.UseVisualStyleBackColor = true;
            this.btnFolderPath.Click += new System.EventHandler(this.btnFolderPath_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(12, 24);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(268, 22);
            this.txtPath.TabIndex = 1;
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(263, 86);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(54, 23);
            this.btnRun.TabIndex = 2;
            this.btnRun.Text = "Recover";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // lblToRecover
            // 
            this.lblToRecover.AutoSize = true;
            this.lblToRecover.Location = new System.Drawing.Point(12, 64);
            this.lblToRecover.Name = "lblToRecover";
            this.lblToRecover.Size = new System.Drawing.Size(63, 12);
            this.lblToRecover.TabIndex = 3;
            this.lblToRecover.Text = "To Recover:";
            // 
            // lblPatnID
            // 
            this.lblPatnID.AutoSize = true;
            this.lblPatnID.Location = new System.Drawing.Point(81, 64);
            this.lblPatnID.Name = "lblPatnID";
            this.lblPatnID.Size = new System.Drawing.Size(59, 12);
            this.lblPatnID.TabIndex = 4;
            this.lblPatnID.Text = "(Patient ID)";
            // 
            // cmbAdmit
            // 
            this.cmbAdmit.FormattingEnabled = true;
            this.cmbAdmit.Location = new System.Drawing.Point(83, 88);
            this.cmbAdmit.Name = "cmbAdmit";
            this.cmbAdmit.Size = new System.Drawing.Size(124, 20);
            this.cmbAdmit.TabIndex = 5;
            // 
            // lblAdmit
            // 
            this.lblAdmit.AutoSize = true;
            this.lblAdmit.Location = new System.Drawing.Point(12, 88);
            this.lblAdmit.Name = "lblAdmit";
            this.lblAdmit.Size = new System.Drawing.Size(64, 12);
            this.lblAdmit.TabIndex = 6;
            this.lblAdmit.Text = "Time Admit:";
            // 
            // txtLog
            // 
            this.txtLog.AutoWordSelection = true;
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Location = new System.Drawing.Point(12, 117);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.txtLog.Size = new System.Drawing.Size(305, 104);
            this.txtLog.TabIndex = 7;
            this.txtLog.Text = "";
            this.txtLog.TextChanged += new System.EventHandler(this.txtLog_TextChanged);
            // 
            // frmFileRecovery
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 233);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.lblAdmit);
            this.Controls.Add(this.cmbAdmit);
            this.Controls.Add(this.lblPatnID);
            this.Controls.Add(this.lblToRecover);
            this.Controls.Add(this.btnRun);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.btnFolderPath);
            this.Name = "frmFileRecovery";
            this.Text = "AnnealFileRecovery";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmFileRecovery_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFolderPath;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label lblToRecover;
        private System.Windows.Forms.Label lblPatnID;
        private System.Windows.Forms.ComboBox cmbAdmit;
        private System.Windows.Forms.Label lblAdmit;
        private System.Windows.Forms.RichTextBox txtLog;
    }
}


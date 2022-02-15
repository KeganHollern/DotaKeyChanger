namespace DKC.UI
{
    partial class MainWin
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWin));
            this.ctrlSetSteamDIR = new System.Windows.Forms.Button();
            this.ctrlSteamDir = new System.Windows.Forms.TextBox();
            this.ctrlOverwriteBtn = new System.Windows.Forms.Button();
            this.ctrlRestoreBtn = new System.Windows.Forms.Button();
            this.ctrlMainAccount = new System.Windows.Forms.Button();
            this.ctrlMainAccountID = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.SuspendLayout();
            // 
            // ctrlSetSteamDIR
            // 
            this.ctrlSetSteamDIR.Location = new System.Drawing.Point(251, 9);
            this.ctrlSetSteamDIR.Name = "ctrlSetSteamDIR";
            this.ctrlSetSteamDIR.Size = new System.Drawing.Size(75, 23);
            this.ctrlSetSteamDIR.TabIndex = 3;
            this.ctrlSetSteamDIR.Text = "Steam DIR";
            this.ctrlSetSteamDIR.UseVisualStyleBackColor = true;
            this.ctrlSetSteamDIR.Click += new System.EventHandler(this.ctrlSetSteamDIR_Click);
            // 
            // ctrlSteamDir
            // 
            this.ctrlSteamDir.Location = new System.Drawing.Point(12, 12);
            this.ctrlSteamDir.Name = "ctrlSteamDir";
            this.ctrlSteamDir.ReadOnly = true;
            this.ctrlSteamDir.Size = new System.Drawing.Size(233, 20);
            this.ctrlSteamDir.TabIndex = 2;
            // 
            // ctrlOverwriteBtn
            // 
            this.ctrlOverwriteBtn.Enabled = false;
            this.ctrlOverwriteBtn.Location = new System.Drawing.Point(150, 53);
            this.ctrlOverwriteBtn.Name = "ctrlOverwriteBtn";
            this.ctrlOverwriteBtn.Size = new System.Drawing.Size(63, 23);
            this.ctrlOverwriteBtn.TabIndex = 12;
            this.ctrlOverwriteBtn.Text = "Overwrite";
            this.ctrlOverwriteBtn.UseVisualStyleBackColor = true;
            this.ctrlOverwriteBtn.Click += new System.EventHandler(this.ctrlOverwriteBtn_Click);
            // 
            // ctrlRestoreBtn
            // 
            this.ctrlRestoreBtn.Enabled = false;
            this.ctrlRestoreBtn.Location = new System.Drawing.Point(81, 53);
            this.ctrlRestoreBtn.Name = "ctrlRestoreBtn";
            this.ctrlRestoreBtn.Size = new System.Drawing.Size(63, 23);
            this.ctrlRestoreBtn.TabIndex = 11;
            this.ctrlRestoreBtn.Text = "Restore";
            this.ctrlRestoreBtn.UseVisualStyleBackColor = true;
            this.ctrlRestoreBtn.Click += new System.EventHandler(this.ctrlRestoreBtn_Click);
            // 
            // ctrlMainAccount
            // 
            this.ctrlMainAccount.Enabled = false;
            this.ctrlMainAccount.Location = new System.Drawing.Point(219, 53);
            this.ctrlMainAccount.Name = "ctrlMainAccount";
            this.ctrlMainAccount.Size = new System.Drawing.Size(107, 23);
            this.ctrlMainAccount.TabIndex = 13;
            this.ctrlMainAccount.Text = "Main Account";
            this.ctrlMainAccount.UseVisualStyleBackColor = true;
            this.ctrlMainAccount.Click += new System.EventHandler(this.ctrlMainAccount_Click);
            // 
            // ctrlMainAccountID
            // 
            this.ctrlMainAccountID.Location = new System.Drawing.Point(12, 35);
            this.ctrlMainAccountID.Name = "ctrlMainAccountID";
            this.ctrlMainAccountID.Size = new System.Drawing.Size(312, 15);
            this.ctrlMainAccountID.TabIndex = 14;
            this.ctrlMainAccountID.Text = "Current Account Friend ID: UNSPECIFIED";
            this.ctrlMainAccountID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(12, 53);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(63, 23);
            this.button1.TabIndex = 15;
            this.button1.Text = "Backup";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // MainWin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(336, 85);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.ctrlMainAccountID);
            this.Controls.Add(this.ctrlMainAccount);
            this.Controls.Add(this.ctrlOverwriteBtn);
            this.Controls.Add(this.ctrlRestoreBtn);
            this.Controls.Add(this.ctrlSetSteamDIR);
            this.Controls.Add(this.ctrlSteamDir);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainWin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dota 2 Keybinds Changer";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ctrlSetSteamDIR;
        private System.Windows.Forms.TextBox ctrlSteamDir;
        private System.Windows.Forms.Button ctrlOverwriteBtn;
        private System.Windows.Forms.Button ctrlRestoreBtn;
        private System.Windows.Forms.Button ctrlMainAccount;
        private System.Windows.Forms.Label ctrlMainAccountID;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    }
}
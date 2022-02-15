using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DKC.UI
{
    public partial class MainWin : Form
    {
        private TrayIcon mParent;
        public MainWin(TrayIcon parentTray)
        {
            mParent = parentTray;
            InitializeComponent();

            this.ctrlSteamDir.Text = mParent.Steam.Folder;
            OnAccountUpdated();


        }

        public void OnAccountUpdated()
        {
            if(ctrlMainAccount.InvokeRequired)
            {
                ctrlMainAccount.Invoke((MethodInvoker)delegate
                {
                    OnAccountUpdated();
                });
                return;
            }
            if (mParent.CurrentAccount == null)
            {
                ctrlMainAccount.Enabled = false;
                ctrlOverwriteBtn.Enabled = false;
                ctrlRestoreBtn.Enabled = false;
                button1.Enabled = false;
                ctrlMainAccountID.Text = $"Current Account Friend ID: UNSPECIFIED";

            }
            else
            {
                ctrlMainAccount.Enabled = true;
                if (File.Exists(TrayIcon.DESIRED_KEYBINDS_FILENAME))
                    ctrlOverwriteBtn.Enabled = true;
                ctrlRestoreBtn.Enabled = true;
                button1.Enabled = true;
                ctrlMainAccountID.Text = $"Current Account Friend ID: {mParent.CurrentAccount.FriendID}";
            }
        }

        private void ctrlMainAccount_Click(object sender, EventArgs e)
        {
            ;
            //TODO: make this account the main account
            if (mParent.SetMainAccount())
            {
                OnAccountUpdated();
                MessageBox.Show("Main account set");
            }
            else
            {
                MessageBox.Show("Failed to set main account");
            }
        }

        private void ctrlOverwriteBtn_Click(object sender, EventArgs e)
        {
            if(File.Exists(TrayIcon.DESIRED_KEYBINDS_FILENAME))
            {
                if (mParent.CurrentAccount.ImportSettings(TrayIcon.DESIRED_KEYBINDS_FILENAME))
                {
                    mParent.Logger.LogWrite($"Keybinds overwritten for {mParent.CurrentAccount.FriendID}");
                    MessageBox.Show("Keybinds imported");
                }
                else
                {
                    mParent.Logger.LogWrite($"ERR: Failed to overwrite keybinds for {mParent.CurrentAccount.FriendID}");
                    MessageBox.Show("Uh oh! Failed to import keybinds");
                }
            }
            else
            {
                MessageBox.Show("Main account not set!");
            }
        }

        private void ctrlRestoreBtn_Click(object sender, EventArgs e)
        {
            string filename = $"dota_settings_{mParent.CurrentAccount.FriendID}.zip";
            if (File.Exists(filename))
            {
                if(mParent.CurrentAccount.ImportSettings(filename))
                {
                    mParent.Logger.LogWrite($"Keybinds restored for {mParent.CurrentAccount.FriendID}");
                    MessageBox.Show("Keybinds restored from backup");
                }
                else
                {
                    mParent.Logger.LogWrite($"ERR: Failed to restore keybinds for {mParent.CurrentAccount.FriendID}");
                    MessageBox.Show("Uh oh! Failed to restore keybinds");
                }
            }
            else
            {
                mParent.Logger.LogWrite($"ERR: No backup exists for {mParent.CurrentAccount.FriendID}");
                MessageBox.Show("No backup exists for this steam account. Could not restore keybinds.");
            }
                
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (mParent.CurrentAccount.ExportSettings())
            {
                mParent.Logger.LogWrite($"Backup created for {mParent.CurrentAccount.FriendID}");
                MessageBox.Show("Keybinds backup created");
            }
            else
            {
                mParent.Logger.LogWrite($"ERR: Failed to create backup for {mParent.CurrentAccount.FriendID}");
                MessageBox.Show("Failed to create backup");
            }
        }

        private void ctrlSetSteamDIR_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string SteamDir = folderBrowserDialog1.SelectedPath;
                if (!SteamDir.EndsWith("\\"))
                    SteamDir = SteamDir + "\\";

                if (SteamDir.EndsWith("\\userdata\\"))
                    SteamDir = SteamDir.Replace("userdata\\", "");

                if (SteamDir.EndsWith("\\"))
                    SteamDir = SteamDir.Remove(SteamDir.Length - 1);

                mParent.Steam.Folder = SteamDir;
                this.ctrlSteamDir.Text = mParent.Steam.Folder;
                if (mParent.Steam.DotaAccounts.Count() == 0)
                {
                    MessageBox.Show("No Dota accounts detected. Are you sure your Steam Folder is correct?");
                }
            }
        }
    }
}

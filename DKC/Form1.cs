using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.IO.Compression;
using DKC.Backend;

namespace DKC
{
    public partial class Form1 : Form
    {
        SteamFolder Steam;
        List<DotaAccount> Accounts = new List<DotaAccount>();

        DotaAccount Source { get { return Accounts.ElementAt(ctrlMainAccountId.SelectedIndex); } }
        DotaAccount Target { get { return Accounts.ElementAt(ctrlNewAccountId.SelectedIndex); } }

        public Form1()
        {
            InitializeComponent();
            Steam = new SteamFolder();
            Steam.OnSteamFolderChanged += Steam_OnSteamFolderChanged;
            InitializeFromSettingsFile();
        }

        private void Steam_OnSteamFolderChanged(object sender, EventArgs e)
        {
            ctrlSteamDir.Text = Steam.Folder;


            Accounts = Steam.DotaAccounts.ToList();

            ReloadListboxes();

            ctrlMainAccountId.Enabled = true;
        }
        private void ReloadListboxes()
        {
            int index1 = ctrlMainAccountId.SelectedIndex;
            int index2 = ctrlNewAccountId.SelectedIndex;
            ctrlMainAccountId.Items.Clear();
            ctrlNewAccountId.Items.Clear();
            ctrlMainAccountId.Items.AddRange(Accounts.Cast<object>().ToArray());
            ctrlNewAccountId.Items.AddRange(Accounts.Cast<object>().ToArray());
            try
            {
                ctrlMainAccountId.SelectedIndex = index1;
                ctrlNewAccountId.SelectedIndex = index2;
            }
            catch
            {

            }
        }

        private void WriteSettings()
        {
            List<string> lines = new List<string>();
            lines.Add(Steam.Folder);
            try
            {
                lines.Add(Source.FriendID);
            }
            catch { lines.Add(""); }
            try
            {
                lines.Add(Target.FriendID);
            }
            catch { lines.Add(""); }
            foreach(KeyValuePair<string,string> kv in DotaAccount.AccountNicks)
            {
                lines.Add(kv.Key + "`" + kv.Value.Replace("`",""));
            }
            File.WriteAllLines("settings.ini",lines.ToArray());
        }

        //Settings functionality
        private void InitializeFromSettingsFile()
        {
            try
            {
                string[] lines = File.ReadAllLines("settings.ini");

                if (lines.Length >= 3)
                {
                    if (lines.Length > 3)
                    {
                        for (int i = 3; i < lines.Length; i++)
                        {
                            string[] parts = lines[i].Split('`');
                            DotaAccount.AccountNicks.Add(parts[0], parts[1]);
                        }
                    }

                    Steam.Folder = lines[0]; //this triggers events leading to Steam_OnSteamFolderChanged firing before the next line
                    ctrlMainAccountId.SelectedIndex = Accounts.FindIndex((dc) => { return dc.FriendID == lines[1]; }); //select our source account
                    ctrlNewAccountId.SelectedIndex = Accounts.FindIndex((dc) => { return dc.FriendID == lines[2]; }); //select our target account
                    
                }

            }
            catch { }
        }

        //Gui functionality
        private delegate void SetStatusCallback(string status);
        private void SetStatus(string status)
        {
            if(label1.InvokeRequired)
            {
                label1.Invoke(new SetStatusCallback(SetStatus), status);
                return;
            }
            label3.Text = "Status: " + status;
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                string SteamDir = folderBrowserDialog1.SelectedPath;
                if (!SteamDir.EndsWith("\\"))
                    SteamDir = SteamDir + "\\";

                if (SteamDir.EndsWith("\\userdata\\"))
                    SteamDir = SteamDir.Replace("userdata\\", "");

                if (SteamDir.EndsWith("\\"))
                    SteamDir = SteamDir.Remove(SteamDir.Length - 1);

                Steam.Folder = SteamDir;
            }
        }

        private void ctrlMainAccountId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctrlNewAccountId.Enabled = true;

            button3.Enabled = true;
            button4.Enabled = true;
            WriteSettings();
        }

        private void ctrlNewAccountId_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            label3.Text = "Status: waiting to begin...";
            WriteSettings();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            try
            {
                this.Target.CopySettingsFrom(this.Source);

                WriteSettings();
                SetStatus("Keybinds Copied!");
            }
            catch (Exception ex)
            {
                SetStatus(ex.Message);
            }
        }


        private void Form1_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true; //we dont want the help cursor, so cancel this event
            HelpDialog hd = new HelpDialog();
            hd.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Export
            if (this.Source.ExportSettings())
                SetStatus("Exported Successfully!");
            else
                SetStatus("Export Failed!");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            //Import
            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string zip_path = openFileDialog1.FileName;
                if (this.Source.ImportSettings(zip_path))
                    SetStatus("Imported Successfully!");
                else
                    SetStatus("Import Failed!");
            }
            
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string nick = Microsoft.VisualBasic.Interaction.InputBox("Enter the nickname for this account", "Nickname", "Main account");
            if (nick == "")
                return;

            if (DotaAccount.AccountNicks.ContainsKey(this.Source.FriendID))
                DotaAccount.AccountNicks[this.Source.FriendID] = nick;
            else
                DotaAccount.AccountNicks.Add(this.Source.FriendID, nick);

            WriteSettings();
            ReloadListboxes();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            
            string nick = Microsoft.VisualBasic.Interaction.InputBox("Enter the nickname for this account", "Nickname", "Alt account");
            if (nick == "")
                return;

            if (DotaAccount.AccountNicks.ContainsKey(this.Target.FriendID))
                DotaAccount.AccountNicks[this.Target.FriendID] = nick;
            else
                DotaAccount.AccountNicks.Add(this.Target.FriendID, nick);
            WriteSettings();
            ReloadListboxes();
        }
    }
}

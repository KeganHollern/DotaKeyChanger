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

namespace DKC
{
    public partial class Form1 : Form
    {
        string SteamDir;
        List<string> account_folder_names = new List<string>();
        string main_account;
        string new_account;

        public Form1()
        {
            InitializeComponent();
            try
            {
                string[] lines = File.ReadAllLines("settings.ini");

                if(lines.Length == 3)
                {
                    SteamDir = lines[0];
                    main_account = lines[1];
                    new_account = lines[2];
                    UpdateSteamDir();
                    int err = 0;
                    try
                    {
                        ctrlMainAccountId.SelectedIndex = ctrlMainAccountId.FindString($"[U:1:{main_account}]");
                        if(ctrlMainAccountId.SelectedIndex != -1)
                        {
                            ctrlNewAccountId.Enabled = true;
                        }
                    } catch
                    {
                        err++;
                    }
                    try
                    {
                        ctrlNewAccountId.SelectedIndex = ctrlNewAccountId.FindString($"[U:1:{new_account}]");
                    }
                    catch { err++; }
                    if(err == 0)
                    {
                        button2.Enabled = true;
                    }

                }

            } catch { }
        }
        public delegate void SetStatusCallback(string status);
        public void SetStatus(string status)
        {
            if(label1.InvokeRequired)
            {
                label1.Invoke(new SetStatusCallback(SetStatus), status);
                return;
            }
            label3.Text = "Status: " + status;
        }
        private void UpdateSteamDir()
        {
            ctrlSteamDir.Text = SteamDir;

            try
            {
                foreach (string dir in Directory.EnumerateDirectories(SteamDir + "userdata"))
                {
                    account_folder_names.Add(dir.ToLower().Replace(SteamDir.ToLower() + "userdata\\", ""));
                }

                ctrlMainAccountId.Items.AddRange(account_folder_names.Select((s) => { return $"[U:1:{s}]"; }).ToArray());
                ctrlNewAccountId.Items.AddRange(account_folder_names.Select((s) => { return $"[U:1:{s}]"; }).ToArray());
                ctrlMainAccountId.Enabled = true;
            }
            catch
            {
                SetStatus("Steam path incorrect?");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                SteamDir = folderBrowserDialog1.SelectedPath;
                if (!SteamDir.EndsWith("\\"))
                    SteamDir = SteamDir + "\\";

                if (SteamDir.EndsWith("\\userdata\\"))
                    SteamDir = SteamDir.Replace("userdata\\", "");

                UpdateSteamDir();
            }
        }

        private void ctrlMainAccountId_SelectedIndexChanged(object sender, EventArgs e)
        {
            ctrlNewAccountId.Enabled = true;
            main_account = account_folder_names.ElementAt(ctrlMainAccountId.SelectedIndex);
        }

        private void ctrlNewAccountId_SelectedIndexChanged(object sender, EventArgs e)
        {
            button2.Enabled = true;
            new_account = account_folder_names.ElementAt(ctrlNewAccountId.SelectedIndex);
            label3.Text = "Status: waiting to begin...";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            label3.Text = "Status: starting keybind transfer...";
            new Task(() =>
            {

                string main_folder = SteamDir + @"userdata\" + main_account + @"\570";
                string new_folder = SteamDir + @"userdata\" + new_account + @"\570";

                if(!Directory.Exists(main_folder))
                {
                    SetStatus("Main folder does not exist?");
                    return;
                }
                if (!Directory.Exists(new_folder))
                {
                    SetStatus("New folder does not exist?");
                    return;
                }

                try
                {

                    Copy(main_folder, new_folder);

                    File.WriteAllLines("settings.ini", new string[] { SteamDir, main_account, new_account });

                    SetStatus("Keybind copy complete.");
                } catch
                {

                    SetStatus("Failed to copy keybinds.");
                }
            }).Start();
        }


        public void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        public void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
                SetStatus($"Copying {target.FullName}\\{fi.Name}");
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }

            // Copy each subdirectory using recursion.
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                    target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
    }
}

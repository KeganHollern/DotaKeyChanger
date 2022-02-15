using DKC.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DKC.Backend;
using System.IO;
using System.Diagnostics;

namespace DKC.UI
{
    public class TrayIcon : ApplicationContext
    {
        public const string DESIRED_KEYBINDS_FILENAME = "DesiredKeybinds.zip";

        private NotifyIcon trayIcon;
        private MenuItem mSteamId;
        private MainWin mWindow;

        public SteamFolder Steam;
        public DotaAccount CurrentAccount;
        public LogWriter Logger;
        public string MainAccountID = "";
        public bool Autodetect = false;

        public TrayIcon()
        {
            Logger = new LogWriter();


            Steam = new SteamFolder();
            Steam.OnSteamFolderChanged += Steam_OnSteamFolderChanged;
            InitializeFromSettingsFile();
            mWindow = new MainWin(this);




            MenuItem mExit = new MenuItem("Exit", Exit);
           // MenuItem mAutosync = new MenuItem("Autodetect Changes", ToggleAutodetect);
           // mAutosync.Checked = this.Autodetect;
            MenuItem mStartup = new MenuItem("Run at Startup", ToggleStartup);
            mStartup.Checked = RegistryUtils.GetStartup();
            MenuItem mShowWindow = new MenuItem("Show Window", ShowWindow);
            mSteamId = new MenuItem("Steam: ?");
            MenuItem mSplitter = new MenuItem("-");

            trayIcon = new NotifyIcon()
            {
                Icon = Resources.DotaIcon,
                ContextMenu = new ContextMenu(new MenuItem[]
                {
                    mSteamId,
                    mSplitter,
                    mShowWindow,
                    //mAutosync,
                    mStartup,
                    mExit
                }),
                Visible = true
            };


            Task.Run(() => MonitorKeybinds());

            if(!File.Exists(DESIRED_KEYBINDS_FILENAME))
            {
                ShowWindow(null, null);
            }
        }

        #region Settings
        private void WriteSettings()
        {
            List<string> lines = new List<string>();
            lines.Add(Steam.Folder);
            lines.Add(MainAccountID);
            lines.Add(Autodetect.ToString());
            File.WriteAllLines("settings.ini", lines.ToArray());
        }
        private void InitializeFromSettingsFile()
        {
            try
            {
                string[] lines = File.ReadAllLines("settings.ini");

                if (lines.Length >= 3)
                {
                    Autodetect = bool.Parse(lines[2]);
                    MainAccountID = lines[1]; 
                    Steam.Folder = lines[0];// race condition. on folder set, settings are re-written. need to set this item last
                }
            }
            catch {
                Steam.Folder = @"C:\Program Files (x86)\Steam\";
            }
        }
        #endregion

        #region Actions
        public bool SetMainAccount()
        {
            if (CurrentAccount == null) return false;

            MainAccountID = CurrentAccount.FriendID;
            WriteSettings();

            if (!CurrentAccount.ExportSettings(DESIRED_KEYBINDS_FILENAME))
            {
                MessageBox.Show("Failed to export keybinds for this account. Have you launched dota on this account yet?");
                return false;
            }
            return true;
        }
        #endregion

        #region Events


        private void Steam_OnSteamFolderChanged(object sender, EventArgs e)
        {
            Logger.LogWrite($"Steam Folder Changed");

            string steamID = RegistryUtils.GetCurrentSteam();
            CurrentAccount = Steam.DotaAccounts.FirstOrDefault((account) => account.FriendID == steamID);
            WriteSettings();
            OnAccountChanged();
        }
        

        // handles what to do when we notice a steam account has been changed (either via configuring our steam folder or by swapping steam accounts)
        void OnAccountChanged()
        {
            if(mWindow != null && !mWindow.IsDisposed)
                mWindow.OnAccountUpdated();

            if (CurrentAccount == null)
            {
                Logger.LogWrite($"WARN: Dota account not found. Has steam directory been configured?");
                return; // maybe closed steam / logged out?
            }

            // backup existing settings if one does not already exists
            if (!File.Exists($"dota_settings_{CurrentAccount.FriendID}.zip"))
            {
                Logger.LogWrite("Backing Up Old Settings");
                if (!CurrentAccount.ExportSettings())
                {
                    Logger.LogWrite($"ERR: Failed to backup keybinds.");
                    if (MessageBox.Show("uh oh! failed to create a backup of this account's keybinds! Would you like to overwrite them with your preffered keybinds?", "Uh", MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.No)
                    {
                        return; // moving forward we'll not try to export or import keybinds until the user relogs or forces
                    }
                }
            }

            // import desired settings if they exist
            if (File.Exists(DESIRED_KEYBINDS_FILENAME))
            {
                Logger.LogWrite("Importing Desired Keybinds");
                if (!CurrentAccount.ImportSettings(DESIRED_KEYBINDS_FILENAME))
                {
                    Logger.LogWrite($"ERR: Failed to import desired keybinds.");
                    MessageBox.Show("Uh oh! failed to import desired keybinds! you'll have to import them by clicking the button!");
                }
            }

        }


        void ShowWindow(object sender, EventArgs e)
        {
            mWindow.ShowDialog();
        }
        void ToggleAutodetect(object sender, EventArgs e)
        {
            ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
            this.Autodetect = ((MenuItem)sender).Checked;
            WriteSettings();
        }
        void ToggleStartup(object sender, EventArgs e)
        {
            ((MenuItem)sender).Checked = !((MenuItem)sender).Checked;
            RegistryUtils.SetStartup(((MenuItem)sender).Checked);
        }
        void Exit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            Application.Exit();
        }
        #endregion

        #region Thread

        // background thread to write keybinds to steam accounts as we swap
        void MonitorKeybinds()
        {
            bool was_dota_open = false;
            string last_steamID = CurrentAccount?.FriendID;
            while (true)
            {
                string steamID = RegistryUtils.GetCurrentSteam();
                if (steamID == "") 
                    mSteamId.Text = $"Steam: Not Detected";
                else 
                    mSteamId.Text = $"Steam: {steamID}";



                if (steamID != last_steamID)
                {
                    Logger.LogWrite($"Detected steam login. Friend ID: {steamID}");
                    last_steamID = steamID;

                    // update current steam account
                    CurrentAccount = Steam.DotaAccounts.FirstOrDefault((account) => account.FriendID == steamID);
                    OnAccountChanged();
                }


                if (this.Autodetect)
                {
                    if (!was_dota_open && Process.GetProcessesByName("dota2").Length > 0)
                    {
                        was_dota_open = true;
                    }
                    if (was_dota_open && Process.GetProcessesByName("dota2").Length == 0)
                    {
                        CurrentAccount.ExportSettings(DESIRED_KEYBINDS_FILENAME); // main account just closed dota, store keybind changes to 
                        was_dota_open = false;
                    }
                }


                Task.Delay(1000).Wait();
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKC.Backend
{
    class DotaAccount
    {
        public static Dictionary<string, string> AccountNicks = new Dictionary<string, string>();

        private SteamFolder Source;
        public string FriendID { get; set; }
        public string SettingsPath { get { return Source.UserDataFolder + @"\" + FriendID + @"\570"; } }
        public string Nickname {
            get {
                if (!AccountNicks.ContainsKey(FriendID))
                    return ("Friend ID: " + FriendID);

                return AccountNicks[FriendID];
            }
        }

        public DotaAccount(SteamFolder source, string friend_id)
        {
            this.Source = source;
            this.FriendID = friend_id;
        }

        public override string ToString()
        {
            return Nickname;
        }


        public bool ExportSettings(string zip_path = "")
        {
            try
            {
                if (File.Exists($"dota_settings_{FriendID}.zip"))
                    File.Delete($"dota_settings_{FriendID}.zip");
                ZipFile.CreateFromDirectory(SettingsPath, $"dota_settings_{FriendID}.zip");

                return true;
            }
            catch
            {
                return false;
            }
        }
        public bool ImportSettings(string zip_path)
        {
            bool valid_backup = false;
            string backup_path = Path.GetTempPath() + "dkc_backups\\" + FriendID;
            try
            {
                //clean our previous backup
                Directory.Delete(backup_path, true);
                //create a backup
                Directory.Move(SettingsPath, backup_path);
                valid_backup = true;
                //extract our exported files
                ZipFile.ExtractToDirectory(zip_path, SettingsPath);//does this replace?
                return true;
            }
            catch
            {
                //incase the extract corrupts something, replace the corrupt files w/ our backup
                if (valid_backup)
                {
                    if (Directory.Exists(SettingsPath))
                    {
                        Directory.Delete(SettingsPath, true);
                        Directory.Move(backup_path, SettingsPath);
                    }
                }
                return false;
            }
        }

        public void CopySettingsFrom(DotaAccount source_account)
        {
            if (!Directory.Exists(source_account.SettingsPath))
                throw new DirectoryNotFoundException("Source directory does not exist.");

            if (!Directory.Exists(this.SettingsPath))
                throw new DirectoryNotFoundException($"Target directory does not exist.");

            Copy(source_account.SettingsPath, this.SettingsPath);
        }


        private void Copy(string sourceDirectory, string targetDirectory)
        {
            DirectoryInfo diSource = new DirectoryInfo(sourceDirectory);
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);

            CopyAll(diSource, diTarget);
        }

        private void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);

            // Copy each file into the new directory.
            foreach (FileInfo fi in source.GetFiles())
            {
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

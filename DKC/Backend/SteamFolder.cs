using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DKC.Backend
{
    class SteamFolder
    {
        public string Folder {
            get {
                return _Folder;
            }
            set {
                _Folder = value;
                FolderChanged(_Folder);
            }
        }
        private string _Folder;
        public string UserDataFolder {
            get {
                if (Folder.EndsWith(@"\"))
                {
                    return Folder + "userdata";
                }
                else
                {
                    return Folder + @"\userdata";
                }
            }
        }

        private IEnumerable<string> UserAccounts;
        public IEnumerable<DotaAccount> DotaAccounts {
            get {
                return UserAccounts.Select((account_id) =>
                {
                    return new DotaAccount(this, account_id);

                }).Where((dc) => 
                {
                    return Directory.Exists(dc.SettingsPath);
                });
            }
        }


        public event EventHandler<EventArgs> OnSteamFolderChanged;

        protected void FolderChanged(string path)
        {
            UserAccounts = Directory.EnumerateDirectories(UserDataFolder).Select((s) => { return s.ToLower().Replace(UserDataFolder.ToLower() + "\\", ""); }) ;
            OnSteamFolderChanged?.Invoke(this, new EventArgs());
        }

    }
}

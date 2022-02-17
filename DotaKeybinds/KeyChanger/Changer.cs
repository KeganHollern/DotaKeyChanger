using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaKeybinds.KeyChanger
{
    public class Changer
    {
        private Worker _worker;

        public string CurrentSteamId { get; set; } = "";
        public bool DotaRunning { get; set; } = false;

        // triggers when the current active steam account changes. Will be "" if the user logged out.
        public event EventHandler<AccountChangedEventArgs>? OnAccountChanged;
        // triggers when dota 2 launches or exits.
        public event EventHandler<DotaChangedEventArgs>? OnDotaChanged;


        public void CreateBackup()
        {
            // create backup
            if (CurrentSteamId == "") return;
            if(!FileUtils.AppData.CreateBackup(CurrentSteamId, FileUtils.Steam.GetDotaSettings(CurrentSteamId)))
            {
                throw new Exception("Failed to create backup");
            }

        }
        public void RestoreBackup()
        {
            // import backup
            if (CurrentSteamId == "") return;
            if(!FileUtils.AppData.ExtractBackup(CurrentSteamId, FileUtils.Steam.GetDotaSettings(CurrentSteamId)))
            {
                throw new Exception("Failed to restore from backup");
            }

        }
        public void WriteKeybinds()
        {
            // import desired keybinds
            if (CurrentSteamId == "") return;
            if (!FileUtils.AppData.HasKeybinds()) return;
            FileUtils.AppData.Extract(CurrentSteamId, FileUtils.AppData.GetKeybindsPath(), FileUtils.Steam.GetDotaSettings(CurrentSteamId));
        }
        public void CreateKeybinds()
        {
            // export desired keybinds
            if (CurrentSteamId == "") return;
            CreateBackup();
            if (!FileUtils.AppData.SetKeybinds(CurrentSteamId))
            {
                throw new Exception("Failed to set primary keybinds");
            }

        }



        public Changer() 
        { 
            _worker = new Worker();
            _worker.OnAccountChanged += _worker_OnAccountChanged;
            _worker.OnDotaChanged += _worker_OnDotaChanged;
            _worker.Run();
        }


        private void _worker_OnDotaChanged(object? sender, DotaChangedEventArgs e)
        {
            DotaRunning = e.IsRunning;
            OnDotaChanged?.Invoke(this, e);

            if (!e.IsRunning)
            {
                // user just closed dota
                // TODO: if this is the "primary" steam account, should we overwrite the prefered keybinds? 
            }
            else
            {
                // user just launched dota
                // probably not gonna do anything
                WriteKeybinds();
            }
        }

        private void _worker_OnAccountChanged(object? sender, AccountChangedEventArgs e)
        {
            CurrentSteamId = e.SteamID;
            OnAccountChanged?.Invoke(this, e);

            if (e.SteamID != "")
            {
                // user logged in
                // need to wait a few seconds otherwise our keybinds are overwritten again?!
                Task.Delay(5000).ContinueWith(t =>
                {
                    if (!FileUtils.AppData.HasBackup(CurrentSteamId))
                        CreateBackup();

                    WriteKeybinds();
                });
            }
        }
    }
}

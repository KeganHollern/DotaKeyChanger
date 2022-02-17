using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaKeybinds.KeyChanger
{
    public class AccountChangedEventArgs : EventArgs
    {
        public string SteamID { get; set; } = "";
    }
    public class DotaChangedEventArgs : EventArgs
    {
        public bool IsRunning { get; set; } = false;
    }
    internal class Worker
    {
        private string currentUserId { get; set; } = "";
        private bool currentDotaStatus { get; set; } = false;

        // triggers when the current active steam account changes. Will be "" if the user logged out.
        public event EventHandler<AccountChangedEventArgs>? OnAccountChanged;
        // triggers when dota 2 launches or exits.
        public event EventHandler<DotaChangedEventArgs>? OnDotaChanged;

        private Task? tActiveThread = null;

        public Task? Run()
        {
            if(tActiveThread == null)
            {
                tActiveThread = Task.Run(() => thread());
            }
            return tActiveThread;
        }


        void accountChanged()
        {
            // steam account changed! (login/logout)
            OnAccountChanged?.Invoke(this, new AccountChangedEventArgs()
            {
                SteamID = currentUserId,
            });
        }
        void dotaChanged()
        {
            // dota running changed
            OnDotaChanged?.Invoke(this, new DotaChangedEventArgs()
            {
                IsRunning = currentDotaStatus,
            });
        }

        private void thread()
        {
            try
            {
                bool was_dota_running = false;
                string last_user_id = "";
                RegistryUtils.Steam.GetInstallPath(); // this throws exception if steam isn't installed
                if (!RegistryUtils.Steam.HasDotaInstalled()) throw new Exception("Dota 2 not installed");
                while (true)
                {
                    currentDotaStatus = RegistryUtils.Steam.IsDotaRunning();
                    if(was_dota_running != currentDotaStatus)
                    {
                        was_dota_running = currentDotaStatus;

                        dotaChanged();
                    }

                    // update current user id
                    if (!RegistryUtils.Steam.IsSteamRunning())
                    {
                        currentUserId = "";
                    }
                    else if (!RegistryUtils.Steam.IsLoggedIn())
                    {
                        currentUserId = "";
                    }
                    else
                    {
                        currentUserId = RegistryUtils.Steam.GetSteamId();
                    }
                    if (last_user_id != currentUserId)
                    {
                        last_user_id = currentUserId;

                        accountChanged();
                    }

                    Task.Delay(100).Wait();
                }
            } catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
                Application.Exit();
            }
        }
    }
}

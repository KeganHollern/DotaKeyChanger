using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DotaKeybinds.RegistryUtils
{
    internal class Steam
    {
        const string REGISTRY_STEAM_PATH = @"SOFTWARE\Valve\Steam";
        const string REGISTRY_DOTA_PATH = @"SOFTWARE\Valve\Steam\Apps\570";
        const string REGISTRY_ACTIVEPROCESS_PATH = @"SOFTWARE\Valve\Steam\ActiveProcess";

        const string REGISTRY_STEAM_INSTALLPATH_DEFAULT = "c:/program files (x86)/steam";

        public static string GetInstallPath()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_STEAM_PATH, false);
            if (rk == null) throw new Exception("Could not find 'Steam' path. Is Steam Installed?");
            string? result = (string?)rk.GetValue("SteamPath", REGISTRY_STEAM_INSTALLPATH_DEFAULT);
            if (result == null) return REGISTRY_STEAM_INSTALLPATH_DEFAULT;
            return result;
        } 
        public static bool HasDotaInstalled()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_DOTA_PATH, false);
            if (rk == null) return false;
            int? installed = (int?)rk.GetValue("Installed", 0);
            if (installed == null || installed == 0) return false;
            return true;
        }
        public static bool IsDotaRunning()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_DOTA_PATH, false);
            if (rk == null) return false;
            int? installed = (int?)rk.GetValue("Running", 0);
            if (installed == null || installed == 0) return false;
            return true;
        }
        public static bool IsSteamRunning()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_ACTIVEPROCESS_PATH, false);
            if (rk == null) return false;
            int pid = (int?)rk.GetValue("pid", 0) ?? 0;
            if (pid == 0) return false;
            return true;
        }
        public static bool IsLoggedIn()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_ACTIVEPROCESS_PATH, false);
            if (rk == null) return false;
            int activeUser = (int?)rk.GetValue("ActiveUser") ?? 0;
            if (activeUser == 0) return false;
            return true;
        }
        public static string GetSteamId()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_ACTIVEPROCESS_PATH, false);
            if (rk == null) return "";
            int activeUser = (int?)rk.GetValue("ActiveUser") ?? 0;
            if (activeUser == 0) return "";
            return activeUser.ToString();
        }
    }
}

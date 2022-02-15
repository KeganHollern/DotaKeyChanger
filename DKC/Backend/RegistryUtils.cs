using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace DKC.Backend
{
    class RegistryUtils
    {
        public static bool GetStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            if ((string)rk.GetValue("Dota 2 Keybinds Changer", "") != "") return true;
            return false;
        }
        public static void SetStartup(bool runAtStart)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if(runAtStart)
            {
                rk.SetValue("Dota 2 Keybinds Changer", Application.ExecutablePath);
            }
            else
            {
                rk.DeleteValue("Dota 2 Keybinds Changer", false);
            }
        }
        public static string GetCurrentSteam()
        {
            const int default_value = 0;
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Valve\\Steam\\ActiveProcess", false);
            int value = (int)rk.GetValue("ActiveUser", default_value);
            if (value == default_value)
            {
                return "";
            }
            return value.ToString();
        }
    }
}

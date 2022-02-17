using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace DotaKeybinds.RegistryUtils
{
    internal class Startup
    {
        const string RUN_ENTRY_NAME = "Dota2KeybindsChanger";
        const string REGISTRY_RUN_PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";
        const string REGISTRY_STARTUP_PATH = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Explorer\\StartupApproved\\Run";
        static readonly byte[] STARTUP_VALUE_DISABLED = new byte[] { 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
        static readonly byte[] STARTUP_VALUE_ENABLED = new byte[] { 0x02, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        public static void Add()
        {
            RegistryKey?  rk = Registry.CurrentUser.OpenSubKey(REGISTRY_RUN_PATH, true);
            if (rk == null) throw new Exception("Could not find 'Run' entry in Registry! Corrupt Windows?");
            rk.SetValue(RUN_ENTRY_NAME, $"\"{Application.ExecutablePath}\" -autostart");

            rk = Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, true);
            if (rk == null) throw new Exception("Could not find 'Run' entry in Registry! Corrupt Windows?");
            if(rk.GetValue(RUN_ENTRY_NAME) == null)
            {
                rk.SetValue(RUN_ENTRY_NAME, new byte[]{ 0x03});
            }
        }
        public static void Enable(bool value)
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, true);
            if (rk == null) throw new Exception("Could not find 'Run' entry in Registry! Corrupt Windows?");
            if(!value)
                rk.SetValue(RUN_ENTRY_NAME, STARTUP_VALUE_DISABLED);
            else
                rk.SetValue(RUN_ENTRY_NAME, STARTUP_VALUE_ENABLED);

        }
        public static bool IsEnabled()
        {
            RegistryKey? rk = Registry.CurrentUser.OpenSubKey(REGISTRY_STARTUP_PATH, false);
            if (rk == null) throw new Exception("Could not find 'Run' entry in Registry! Corrupt Windows?");
            byte[]? value = (byte[]?)rk.GetValue(RUN_ENTRY_NAME, STARTUP_VALUE_DISABLED);
            if (value == null || value[0] == STARTUP_VALUE_DISABLED[0]) return false;
            return true;
        }
    }
}

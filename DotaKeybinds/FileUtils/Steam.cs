using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotaKeybinds.FileUtils
{
    internal class Steam
    {
        //TODO: tools for reading/writing/zipping keybinds for dota

        public static string GetDotaSettings(string steamid)
        {
            string SteamPath = RegistryUtils.Steam.GetInstallPath().Replace("/",@"\");
            string Dota2Path = Path.Combine(SteamPath, "userdata", steamid, "570");
            if(!Directory.Exists(Dota2Path))
                Directory.CreateDirectory(Dota2Path);

            return Dota2Path;
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;

namespace DotaKeybinds.FileUtils
{
    internal class AppData
    {
        const string APPLICATION_DATA_FOLDER = "DotaKeybindChanger";
        const string WORKING_DIRECTORY = "Temp";
        const string KEYBIND_FILE_NAME = "UserKeybinds";
        const string BACKUP_FILE_PREFIX = "KeybindsBackup";

        public static string GetAppFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), APPLICATION_DATA_FOLDER);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string GetBackupPath(string steamid)
        {
            return Path.Combine(GetAppFolder(), $"{BACKUP_FILE_PREFIX}_{steamid}.zip");
        }
        public static string GetKeybindsPath()
        {
            return Path.Combine(GetAppFolder(), $"{KEYBIND_FILE_NAME}.zip");
        }
        public static string GetWorkingPath()
        {
            string path = Path.Combine(GetAppFolder(), WORKING_DIRECTORY);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }


        public static bool HasKeybinds()
        {
            return File.Exists(GetKeybindsPath());
        }
        public static bool HasBackup(string steamid)
        {
            return File.Exists(GetBackupPath(steamid));
        }

        //extracts the settings for user w/ steamid to the destination folder
        public static bool ExtractBackup(string steamid, string destination)
        {
            if (!HasBackup(steamid)) return false;
            string source = GetBackupPath(steamid);

            return Extract(steamid, source, destination);
        }
        public static bool Extract(string steamid, string source, string destination)
        {
            if (!Directory.Exists(destination))
                Directory.CreateDirectory(destination);

            string backup_path = Path.Combine(GetWorkingPath(), steamid);
            // remove old extract backup
            if (Directory.Exists(backup_path))
            {
                Directory.Delete(backup_path, true);
            }

            Directory.Move(destination, backup_path);

            try
            {
                ZipFile.ExtractToDirectory(source, destination);
            }
            catch (Exception ex)
            {
                // restore our keybinds from the backup we took
                Directory.Delete(destination, true);
                // BUGFIX: cannot Directory.Move across drives... (why would these ever be in different drives? idk!)
                DirectoryCopy(backup_path, destination, true);
                Directory.Delete(backup_path, true);

                // throw exception & wrap the exception we actually had
                throw new Exception("Failed to extract backup", ex);
            }
            return true;
        }
        public static bool CreateBackup(string steamid, string source)
        {
            if (HasBackup(steamid))
            {
                File.Delete(GetBackupPath(steamid));
            }

            ZipFile.CreateFromDirectory(source, GetBackupPath(steamid));
            return true;
        }
        public static bool SetKeybinds(string steamid)
        {
            if (!HasBackup(steamid)) return false;

            File.Copy(GetBackupPath(steamid), GetKeybindsPath(), true);

            return true;
        }

        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

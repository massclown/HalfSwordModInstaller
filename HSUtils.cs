using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Security.Principal;

namespace HalfSwordModInstaller
{
    internal static class HSUtils
    {
        public const int HSSteamAppID = 2642680;
        // These strings are installation-dependent and may be broken
        public static string HSInstallPath = null;
        public static string HSBinaryPath = null;
        public static string HSLogicModsPath = null;
        public static string HSUE4SSModsTxt = null;
        public static string HSUE4SSlog = null;
        public const string BPMLName = "BPModLoaderMod";

        // These strings should exist always
        public static string HSModInstallerDirPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HalfSwordModInstaller");

        public static string HSModInstallerLogFilePath = Path.Combine(
                    HSModInstallerDirPath,
                    "installer.log");

        public static string GetGameInstallPath(int appId)
        {
            // Find the Steam install directory from the registry
            string steamPathHKCU = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            string steamPathHKLM64 = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
            string steamPathHKLM = (string)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam", "InstallPath", null);

            if (steamPathHKCU == null && steamPathHKLM == null && steamPathHKLM64 == null)
            {
                throw new InvalidOperationException("[ERROR] Steam is not installed on this machine. No valid registry paths found.");
            }

            // Replace slashes to match the system's format for HKCU path that has format c:/program files (x86)/steam
            // Or, if that one was not found, use HKLM path instead
            string firstSteamPathFound = (steamPathHKCU != null) ? steamPathHKCU.Replace("/", "\\") : (steamPathHKLM != null) ? steamPathHKLM : steamPathHKLM64;

            // Path to the libraryfolders.vdf which contains paths to all Steam libraries
            string libraryFoldersPath = Path.Combine(firstSteamPathFound, "steamapps", "libraryfolders.vdf");
            if (!File.Exists(libraryFoldersPath))
            {
                throw new InvalidOperationException($"[ERROR] Steam installation broken, cannot find \"libraryfolders.vdf\" at \"{libraryFoldersPath}\"");
            }
            string libraryFoldersContent = File.ReadAllText(libraryFoldersPath);

            // Use regex to find all library paths
            var matches = Regex.Matches(libraryFoldersContent, "\"path\"\\s+\"(.+?)\"");

            foreach (Match match in matches)
            {
                string libraryPath = match.Groups[1].Value.Replace("\\\\", "\\");
                string appManifestPath = Path.Combine(libraryPath, "steamapps", $"appmanifest_{appId}.acf");

                // Check if the appmanifest file exists which indicates the game is installed
                if (File.Exists(appManifestPath))
                {
                    // Read the contents of the appmanifest file that has all we need
                    string appManifestContent = File.ReadAllText(appManifestPath);

                    Match installDirMatch = Regex.Match(appManifestContent, "\"installdir\"\\s+\"(.+?)\"");

                    if (installDirMatch.Success)
                    {
                        // Construct the full path to the game's install directory
                        return Path.Combine(libraryPath, "steamapps", "common", installDirMatch.Groups[1].Value);
                    }
                }
            }

            // If we didn't find anything in *.ACF files, try also checking the registry "Installed" DWORD key in
            // Computer\HKEY_CURRENT_USER\Software\Valve\Steam\Apps\2642680 
            // Computer\HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam\Apps\2642680
            int installedHKCU = (int)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam\Apps\" + appId.ToString(), "Installed", 0);
            int installedHKLM = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Valve\Steam\Apps\" + appId.ToString(), "Installed", 0);
            int installedHKLM64 = (int)Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam\Apps\" + appId.ToString(), "Installed", 0);

            if (installedHKCU == 1)
            {
                HSUtils.Log($"[WARNING] Valid Steam game entry present in registry [HKCU], but no valid appmanifest on disk was found.");
            }
            if (installedHKLM == 1)
            {
                HSUtils.Log($"[WARNING] Valid Steam game entry present in registry [HKLM], but no valid appmanifest on disk was found.");
            }
            if (installedHKLM64 == 1)
            {
                HSUtils.Log($"[WARNING] Valid Steam game entry present in registry [HKLM 64-bit], but no valid appmanifest on disk was found.");
            }

            throw new InvalidOperationException($"[ERROR] Steam game with App ID {appId} is not installed or could not be found.");
        }

        public static void Log(string message)
        {
            try
            {
                var logDirectory = Path.GetDirectoryName(HSModInstallerLogFilePath);
                if (!Directory.Exists(logDirectory))
                {
                    Directory.CreateDirectory(logDirectory);
                }

                using (StreamWriter sw = File.AppendText(HSModInstallerLogFilePath))
                {
                    sw.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while logging to file: " + ex.Message);
            }
        }

        public static void ForceExtractToDirectory(string archiveFileName, string destinationDirectoryName)
        {
            using (ZipArchive archive = ZipFile.OpenRead(archiveFileName))
            {
                archive.ExtractToDirectory(destinationDirectoryName, true); // Set to 'true' to overwrite
            }
        }

        public static bool IsProcessRunning(string processName)
        {
            Process[] processes = Process.GetProcessesByName(processName);
            return processes.Length > 0;
        }

        public static bool IsAnotherInstallerRunning()
        {
            var processes = Process.GetProcesses();
            var thisProcessName = Process.GetCurrentProcess().ProcessName;

            var filteredProcesses = processes
                .Where(p => p.ProcessName.ToLower().Contains("HalfSwordModInstaller".ToLower())
                || p.ProcessName.ToLower().Contains(thisProcessName.ToLower())
                )
                .ToList();

            if (filteredProcesses.Count > 1)
            {
                HSUtils.Log($"[WARNING] Another installer is running!");
                return true;
            }
            return false;
        }

        public static bool IsHalfSwordRunning()
        {
            var isHalfSwordRunningFlag = IsProcessRunning("HalfSwordUE5-Win64-Shipping");
            if (isHalfSwordRunningFlag)
            {
                HSUtils.Log($"[WARNING] Half Sword game is running!");
            }
            return isHalfSwordRunningFlag;
        }

        public static bool IsRunningAsAdmin()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        static HSUtils()
        {
            if (!Directory.Exists(HSModInstallerDirPath))
            {
                Directory.CreateDirectory(HSModInstallerDirPath);
            }

            try
            {
                HSInstallPath = GetGameInstallPath(HSSteamAppID);
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] Exception while finding Half Sword Demo:\n{ex.Message}");
                HSUtils.Log(ex.StackTrace);
                HSInstallPath = null;
                return;
            }

            HSBinaryPath = Path.Combine(HSInstallPath, "HalfSwordUE5\\Binaries\\Win64");
            HSLogicModsPath = Path.GetFullPath(Path.Combine(HSBinaryPath, "..\\..\\Content\\Paks\\LogicMods"));
            HSUE4SSModsTxt = Path.Combine(HSBinaryPath, "Mods", "mods.txt");
            HSUE4SSlog = Path.Combine(HSBinaryPath, "UE4SS.log");
        }

    }

    public static class ZipFileExtensions
    {
        public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
        {
            string destinationDirectoryFullPath = Path.GetFullPath(destinationDirectoryName);
            if (!overwrite)
            {
                archive.ExtractToDirectory(destinationDirectoryName);
                return;
            }

            foreach (ZipArchiveEntry file in archive.Entries)
            {
                string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryName, file.FullName));
                string directory = Path.GetDirectoryName(completeFileName);

                if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
                {
                    throw new IOException("Trying to extract file outside of destination directory, aborting.");
                }

                // if (!Directory.Exists(directory))
                // {
                Directory.CreateDirectory(directory);
                // }

                if (file.Name != "")
                {
                    file.ExtractToFile(completeFileName, true);
                }
            }
        }
    }

}

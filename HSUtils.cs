using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HalfSwordModInstaller
{
    internal class HSUtils
    {
        public const int HSSteamAppID = 2642680;
        public static string HSInstallPath = GetGameInstallPath(HSSteamAppID);
        public static string HSBinaryPath = Path.Combine(HSInstallPath, "HalfSwordUE5\\Binaries\\Win64");
        public static string HSLogicModsPath = Path.Combine(HSBinaryPath, "\\..\\..\\Content\\Paks\\LogicMods");
        public static string HSUE4SSModsTxt = Path.Combine(HSBinaryPath, "Mods", "mods.txt");

        public static string GetGameInstallPath(int appId)
        {
            // Find the Steam install directory from the registry
            string steamPath = (string)Registry.GetValue(@"HKEY_CURRENT_USER\Software\Valve\Steam", "SteamPath", null);
            if (steamPath == null)
            {
                throw new InvalidOperationException("Steam is not installed on this machine.");
            }

            // Replace slashes to match the system's format
            steamPath = steamPath.Replace("/", "\\");

            // Path to the libraryfolders.vdf which contains paths to all Steam libraries
            string libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");

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

            throw new InvalidOperationException($"Steam game with App ID {appId} is not installed or not found.");
        }
        public static string HSModInstallerDirPath = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HalfSwordModInstaller");

        public static string HSModInstallerLogFilePath = Path.Combine(
                    HSModInstallerDirPath,
                    "installer.log");

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

        public HSUtils()
        {
            if (!Directory.Exists(HSModInstallerDirPath))
            {
                Directory.CreateDirectory(HSModInstallerDirPath);
            }
        }

    }
}

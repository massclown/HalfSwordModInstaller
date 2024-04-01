using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;

namespace HalfSwordModInstaller
{
    public class HSMod
    {
        // This is a name without spaces
        public string Name { get; set; }
        // Github repo URL
        public string Url { get; set; }
        // A downloaded ZIP file of the mod stored locally
        protected string LocalZipPath;

        protected bool _isDownloaded = false;
        public bool IsDownloaded
        {
            get
            {
                if (string.IsNullOrEmpty(LocalZipPath))
                {
                    if (!string.IsNullOrEmpty(Version))
                    {
                        string releaseZip = $"{Name}_{Version}.zip";
                        string downloadsFolder = Path.Combine(HSUtils.HSModInstallerDirPath, "downloads", Name, Version);
                        string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);

                        if (File.Exists(releaseZipPath))
                        {
                            _isDownloaded = true;
                            return _isDownloaded;
                        }
                    }
                }
                else
                {
                    if (File.Exists(LocalZipPath))
                    {
                        _isDownloaded = true;
                        return _isDownloaded;
                    }
                }
                _isDownloaded = false;
                return _isDownloaded;
            }
            set
            {
                _isDownloaded |= value;
            }
        }

        protected bool _isInstalled = false;
        public bool IsInstalled
        {
            get
            {
                if (Directory.Exists(RelativePath))
                {
                    if (File.Exists(Path.Combine(RelativePath, "scripts\\main.lua")))
                    {
                        if (HasLogicMods)
                        {
                            if (Directory.Exists(HSUtils.HSLogicModsPath))
                            {
                                // TODO compare files with distribution ZIP maybe?
                                _isInstalled = true;
                            }
                            else
                            {
                                // TODO broken install, LogicMods folder does not exist
                                // For now return as if not installed
                                _isInstalled = false;
                            }
                        }
                        else
                        {
                            _isInstalled = true;
                        }
                    }
                    else
                    {
                        // TODO broken install, folder exists but the actual mod not there
                        // For now return as if not installed
                        _isInstalled = false;
                    }
                }
                return _isInstalled;
            }

            set
            {
                _isInstalled |= value;
            }
        }
        protected bool _isEnabled = false;
        public bool IsEnabled
        {
            get
            {
                if (File.Exists(HSUtils.HSUE4SSModsTxt))
                {
                    var lines = File.ReadAllLines(HSUtils.HSUE4SSModsTxt).ToList();
                    int thisModIndex = lines.FindIndex(line => line.Contains(Name));
                    if (thisModIndex < 0)
                    {
                        _isEnabled = false;
                    }
                    else
                    {
                        int thisModIndexEnabled = lines.FindIndex(line => line.Trim().Equals($"{Name} : 1"));
                        if (thisModIndexEnabled < 0)
                        {
                            _isEnabled = false;
                        }
                        else
                        {
                            _isEnabled = true;
                        }
                    }
                }
                else
                {
                    _isEnabled = false;
                }
                return _isEnabled;
            }

            set
            {
                _isEnabled |= value;
            }
        }
        // Relative to HSBinaryPath, so includes Mods/
        protected string RelativePath = string.Empty;
        protected bool HasLogicMods = false;
        public string Version = string.Empty;
        // For rare cases when we can infer installed version from disk
        public string InstalledVersion = string.Empty;

        // TODO implement the proper state machine for the mod
        /*
         *                                          +----------------------------------+
         *                                          |                                  |
         *                                          V                                  |
         *   Unknown -----> VersionKnown -----> Downloaded <-----> Installed -----> Enabled
         *                       |                  | ^              |  |             ^ |
         *                       |                  | |              |  |             | |
         *                       |                  | |        +-----+  |             | |
         *                       |                  | |       /         V             | |
         *                       |                  | +----------- Disabled <---------+ |
         *                       |                  |       /          |                |
         *                       |                  | +----+           |                |
         *                       |                  | |                |                |
         *                       |                  V V                V                |
         *                       +--------------> Broken <-------------+----------------+
         **/
        public enum ModState
        {
            Unknown,        // Latest version is unknown yet, nothing on disk
            VersionKnown,   // Retrieved the version, but not downloaded the mod successfully yet
            Downloaded,     // Downloaded successfully, but not installed
            Installed,      // Installed, but not enabled (no mods.txt record)
            Enabled,        // Installed and enabled
            Disabled,       // Installed and disabled (record in mods.txt)
            Broken          // Unusable, broken (downloaded and can't be installed, or installed and unusable)
        }

        public HSMod(string name, string url, bool hasLogicMods)
        {
            Name = name;
            Url = url;
            HasLogicMods = hasLogicMods;
            RelativePath = $"Mods\\{Name}";
            Version = GetLatestVersion();
        }

        public void LogMe()
        {
            HSUtils.Log($"Mod=\"{Name}\", Url=\"{Url}\", Version=\"{Version}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, Enabled={IsEnabled}, " +
                $"HasLogicMods={HasLogicMods}");
        }

        public static string GetRedirectUrl(string url)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
            request.AllowAutoRedirect = false;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                return response.Headers["Location"];
            }
        }
        public string GetLatestVersion()
        {
            string latestUrl = Url + "/releases/latest";
            string releaseUrl = GetRedirectUrl(latestUrl);
            string tag = releaseUrl.Substring(releaseUrl.LastIndexOf("/") + 1);
            return tag;
        }

        public void Download()
        {
            string tag = GetLatestVersion();
            Download(tag);
        }

        public void Download(string tag)
        {
            // Example:
            // HalfSwordTrainerMod_v0.8.zip
            string releaseZip = $"{Name}_{tag}.zip";
            // Example:
            // https://github.com/massclown/HalfSwordTrainerMod/releases/download/v0.8/HalfSwordTrainerMod_v0.8.zip
            string downloadUrl = $"{Url}/releases/download/{tag}/{releaseZip}";

            string downloadsFolder = Path.Combine(HSUtils.HSModInstallerDirPath, "downloads", Name, tag);
            if (!Directory.Exists(downloadsFolder))
            {
                Directory.CreateDirectory(downloadsFolder);
            }
            string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);

            using (var client = new WebClient())
            {
                client.DownloadFile(downloadUrl, releaseZipPath);
                LocalZipPath = releaseZipPath;
                _isDownloaded = true;
                Version = tag;
                HSUtils.Log($"Downloaded mod \"{Name}\" from \"{downloadUrl}\" to \"{releaseZipPath}\"");
            }
        }

        public void Install(string tag)
        {
            Download(tag);
            Install();
        }


        public void Install()
        {
            if (!IsDownloaded)
            {
                Download();
            }
            string unzipFolder = Path.Combine(HSUtils.HSBinaryPath, "Mods");
            ZipFile.ExtractToDirectory(LocalZipPath, unzipFolder);
            // Clean up by removing readme and license, yes, this is bad. Sorry.
            string readmePath = Path.Combine(unzipFolder, "README.md");
            File.Delete(readmePath);
            string licensePath = Path.Combine(unzipFolder, "LICENSE");
            File.Delete(licensePath);
            if (HasLogicMods)
            {
                // This is horrible.
                Directory.Move(
                    Path.Combine(unzipFolder, "LogicMods"),
                    HSUtils.HSLogicModsPath
                    );
            }
            InstalledVersion = Version;
            HSUtils.Log($"Installed mod \"{Name}\" from \"{LocalZipPath}\" to \"{unzipFolder}\"");
        }

        public void Uninstall()
        {
            throw new NotImplementedException();
        }

        public void Update()
        {
            // Download latest and install it
            throw new NotImplementedException();
        }

        public void SetEnabled(bool isEnabled = true)
        {
            EditModsTxt(Name, isEnabled);
            if (HasLogicMods)
            {
                EditModsTxt("BPModLoaderMod", isEnabled);
            }
            if (isEnabled)
            {
                HSUtils.Log($"Enabled \"{Name}\"");
            }
            else
            {
                HSUtils.Log($"Disabled \"{Name}\"");
            }
        }

        public bool GetEnabled()
        {
            return IsEnabled;
        }

        public static void EditModsTxt(string modName, bool enabled)
        {
            string value = enabled ? "1" : "0";

            var lines = File.ReadAllLines(HSUtils.HSUE4SSModsTxt).ToList();
            int keybindsIndex = lines.FindIndex(line => line.Contains("Keybinds"));
            int keyIndex = lines.FindIndex(line => line.StartsWith(modName));

            // If the key already exists, just edit it
            if (keyIndex != -1)
            {
                lines[keyIndex] = modName + " : " + value;
            }
            else
            {
                // If the key doesn't exist, add it before existing "Keybinds" to make sure the hotkeys work
                if (keybindsIndex != -1)
                {
                    lines.Insert(keybindsIndex, "");
                    lines.Insert(keybindsIndex, modName + " : " + value);
                    lines.Insert(keybindsIndex, "");
                }
                else
                {
                    // Add missing Keybinds at the end, they are mandatory for hotkeys
                    lines.Add(modName + " : " + value);
                    lines.Add("");
                    lines.Add("Keybinds : 1");
                }
            }
            File.WriteAllLines(HSUtils.HSUE4SSModsTxt, lines);
        }
    }
}
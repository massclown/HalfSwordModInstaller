using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;

namespace HalfSwordModInstaller
{
    public class HSMod : HSInstallable
    {
        public override bool IsInstalled
        {
            get
            {
                if (Directory.Exists(Path.Combine(HSUtils.HSBinaryPath, RelativePath)))
                {
                    if (File.Exists(Path.Combine(HSUtils.HSBinaryPath, RelativePath, "scripts\\main.lua")))
                    {
                        if (HasLogicMods)
                        {
                            if (Directory.Exists(HSUtils.HSLogicModsPath))
                            {
                                // TODO compare the actual files with distribution ZIP maybe?
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
                else
                {
                    _isInstalled = false;
                }   
                return _isInstalled;
            }

            set
            {
                _isInstalled |= value;
            }
        }
        public override bool IsEnabled
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
        protected bool HasLogicMods = false;

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

        public HSMod(string name, string url, bool hasLogicMods) : base(name, url)
        {
            HasLogicMods = hasLogicMods;
            RelativePath = $"Mods\\{Name}";
        }

        public new void LogMe()
        {
            HSUtils.Log($"Mod=\"{Name}\", Url=\"{Url}\", Version=\"{Version}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, Enabled={IsEnabled}, " +
                $"HasLogicMods={HasLogicMods}");
        }

        public override void Install()
        {
            if (!IsDownloaded)
            {
                Download();
            }
            string unzipFolder = Path.Combine(HSUtils.HSBinaryPath, "Mods");
            HSUtils.ForceExtractToDirectory(LocalZipPath, unzipFolder);
            // Clean up by removing readme and license, yes, this is bad. Sorry.
            string readmePath = Path.Combine(unzipFolder, "README.md");
            File.Delete(readmePath);
            string licensePath = Path.Combine(unzipFolder, "LICENSE");
            File.Delete(licensePath);
            // TODO this does not account for which exact files this mod has in LogicMods
            if (HasLogicMods)
            {
                var tempLogicMods = Path.Combine(unzipFolder, "LogicMods");
                // TODO This is all horrible and must be rewritten
                Directory.CreateDirectory(HSUtils.HSLogicModsPath);
                foreach (var file in new DirectoryInfo(tempLogicMods).GetFiles())
                {
                    file.MoveTo(Path.Combine(HSUtils.HSLogicModsPath, file.Name));
                }
                Directory.Delete(tempLogicMods);
            }
            InstalledVersion = Version;
            HSUtils.Log($"Installed mod \"{Name}\" from \"{LocalZipPath}\" to \"{unzipFolder}\"");
        }

        public override void Uninstall()
        {
            if (IsEnabled)
            {
                SetEnabled(false);
            }
            Directory.Delete(Path.Combine(HSUtils.HSBinaryPath, RelativePath), true);
            // TODO DANGER this does not account for which exact files this mod has in LogicMods
            if (HasLogicMods)
            {
                foreach (FileInfo file in new DirectoryInfo(HSUtils.HSLogicModsPath).EnumerateFiles())
                {
                    file.Delete();
                }
            }
        }

        public override void SetEnabled(bool isEnabled = true)
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

        public override bool GetEnabled()
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
                    var insertIndex = keybindsIndex - 1;
                    lines.Insert(insertIndex, "");
                    lines.Insert(insertIndex, modName + " : " + value);
                    lines.Insert(insertIndex, "");
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
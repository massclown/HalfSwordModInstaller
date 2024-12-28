using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace HalfSwordModInstaller
{
    public class HSMod : HSInstallable
    {
        public override bool IsInstalled
        {
            get
            {
                var modInstallPath = Path.Combine(HSUtils.HSUE4SSPath, RelativePath);
                if (Directory.Exists(modInstallPath))
                {
                    // TODO this is a very crude way of detecting a development setup with symlinks, but works for now
                    var attrs = File.GetAttributes(modInstallPath);
                    if ((attrs & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                    {
                        //HSUtils.Log($"[WARNING] Development setup with symlinks detected for mod \"{this.Name}\"");
                        _developmentSetup = true;
                    }

                    var mainLua = Path.Combine(modInstallPath, "scripts\\main.lua");
                    if (File.Exists(mainLua))
                    {
                        // Proper version detection in the lua variable mod_version
                        var luaTextLines = File.ReadLines(mainLua).ToList();
                        var versionLine = luaTextLines.Find(line => line.StartsWith("local mod_version"));
                        if (versionLine != null)
                        {
                            foreach (Match match in Regex.Matches(versionLine, @"local mod_version\s+=\s+""(\d+\.\d+)"""))
                            {
                                if (match.Success && match.Groups.Count > 0)
                                {
                                    InstalledVersion = "v" + match.Groups[1].Value;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // Fallback method, parse the first line of main.lua to get the version from the comment e.g. "-- lolmod v0.1"
                            var firstLine = luaTextLines.First();
                            foreach (Match match in Regex.Matches(firstLine, @"\s(v\d+(\.\d+)+)"))
                            {
                                if (match.Success && match.Groups.Count > 0)
                                {
                                    InstalledVersion = match.Groups[1].Value;
                                    break;
                                }
                            }
                        }
                        if (HasLogicMods)
                        {
                            if (Directory.Exists(HSUtils.HSLogicModsPath))
                            {
                                // TODO properly compare the actual files with distribution ZIP maybe?
                                if (Directory.GetFiles(HSUtils.HSLogicModsPath, "*", SearchOption.AllDirectories).Length == 0)
                                {
                                    // No files in LogicMods means a broken installation == no installation
                                    HSUtils.Log($"[WARNING] Empty LogicMods folder for mod \"{this.Name}\", assuming it is not installed");
                                    _isInstalled = false;
                                }
                                else
                                {
                                    _isInstalled = true;
                                }
                            }
                            else
                            {
                                // TODO broken install, as LogicMods folder does not exist
                                // For now return as if not installed
                                HSUtils.Log($"[WARNING] Missing LogicMods folder for mod \"{this.Name}\", assuming it is not installed");
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
                        HSUtils.Log($"[WARNING] Empty mod folder for mod \"{this.Name}\", assuming it is not installed");
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
                _isInstalled = value;
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
                            if (HasLogicMods)
                            {
                                int BPMLIndexEnabled = lines.FindIndex(line => line.Trim().Equals($"{HSUtils.BPMLName} : 1"));
                                if (BPMLIndexEnabled < 0)
                                {
                                    // Lua part of our mod is enabled, but BPModLoaderMod is not enabled,
                                    // so we consider our mod to be not enabled as well
                                    _isEnabled = false;
                                }
                                else
                                {
                                    _isEnabled = true;
                                }
                            }
                            else
                            {
                                _isEnabled = true;
                            }
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
                _isEnabled = value;
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
        /*
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
        */

        // Development setup for mods is done with symlinks from the game mods folder to somewhere else (e.g. a git repo)
        protected bool _developmentSetup = false;
        public bool DevelopmentSetup
        {
            get
            {
                return _developmentSetup;
            }
            set
            {
                _developmentSetup |= value;
            }
        }

        public HSMod(string name, string url, HSUtils.HSGameType compatibleGameType, bool hasLogicMods, List<HSInstallable> dependencyGraph) : base(name, url, compatibleGameType, dependencyGraph)
        {
            this.HasLogicMods = hasLogicMods;
            // TODO handle game type/isExperimental
            this.RelativePath = $"Mods\\{Name}";
        }

        public new void LogMe()
        {
            HSUtils.Log($"Mod=\"{Name}\", Url=\"{Url}\", LatestVersion=\"{LatestVersion}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, " +
                $"InstalledVersion={(string.IsNullOrEmpty(InstalledVersion) ? "null" : "\"" + InstalledVersion + "\"")}, " +
                $"Experimental={IsExperimental}, " +
                $"CompatibleGameType=\"{CompatibleGameType}\", " +
                $"Enabled={IsEnabled}, " +
                $"HasLogicMods={HasLogicMods}");
        }
        public override void Install()
        {
            Install(false);
        }
        public override void InstallAll()
        {
            Install(true);
        }
        public override void Install(bool forceInstallDependencies = false)
        {
            HSUtils.Log($"Installing mod \"{Name}\" from \"{LocalZipPath}\"...");
            if (this.dependencyGraph?.Count > 0)
            {
                // TODO should we throw an exception here? Or should we simply install the dependency?
                foreach (var dependency in this.dependencyGraph)
                {
                    if (forceInstallDependencies)
                    {
                        dependency.Install(true);
                    }
                    else if (!dependency.IsInstalled)
                    {
                        HSUtils.Log($"[ERROR] Cannot install mod \"{Name}\", dependency \"{dependency.Name}\" is not installed.");
                        return;
                    }
                }
            }
            if (!IsDownloaded)
            {
                Download();
            }
            string unzipFolder = Path.Combine(HSUtils.HSUE4SSPath, "Mods");
            if (!Directory.Exists(unzipFolder))
            {
                // TODO the installation is broken. Should probably bail out.
                HSUtils.Log($"[ERROR] Cannot install mod \"{Name}\", missing \"Mods\" folder");
                return;
            }

            try
            {
                HSUtils.ForceExtractToDirectory(LocalZipPath, unzipFolder);
                // Clean up by removing readme and license, yes, this is bad. Sorry.
                string readmePath = Path.Combine(unzipFolder, "README.md");
                if (File.Exists(readmePath)) { File.Delete(readmePath); }

                string licensePath = Path.Combine(unzipFolder, "LICENSE");
                if (File.Exists(licensePath)) { File.Delete(licensePath); }

                // TODO this does not account for which exact files this mod has in LogicMods
                if (HasLogicMods)
                {
                    var tempLogicMods = Path.Combine(unzipFolder, "LogicMods");
                    // TODO This is all horrible and must be rewritten
                    Directory.CreateDirectory(HSUtils.HSLogicModsPath);
                    foreach (var file in new DirectoryInfo(tempLogicMods).GetFiles())
                    {
                        var newFname = Path.Combine(HSUtils.HSLogicModsPath, file.Name);
                        if (File.Exists(newFname))
                        {
                            HSUtils.Log($"[WARNING] Overwriting file {newFname}");
                            File.Delete(newFname);
                        }
                        file.MoveTo(newFname);
                    }
                    Directory.Delete(tempLogicMods);
                }
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while installing mod \"{Name}\": {ex.Message}");
                HSUtils.Log(ex.StackTrace);
                return;
            }
            InstalledVersion = LatestVersion;
            HSUtils.Log($"Installed mod \"{Name}\" from \"{LocalZipPath}\" to \"{unzipFolder}\"");
        }

        public override void Uninstall()
        {
            var uninstallPath = Path.Combine(HSUtils.HSUE4SSPath, RelativePath);
            HSUtils.Log($"Uninstalling mod \"{Name}\" from \"{uninstallPath}\"...");
            try
            {
                // TODO not sure if we need to erase the mod line from mods.txt or not at this moment
                // Only disable it for now, leave the line in mods.txt
                if (IsEnabled)
                {
                    SetEnabled(false);
                }
                Directory.Delete(uninstallPath, true);
                // TODO DANGER this does not account for which exact files this mod has in LogicMods
                // TODO DANGER We just delete all of them
                if (HasLogicMods)
                {
                    foreach (FileInfo file in new DirectoryInfo(HSUtils.HSLogicModsPath).EnumerateFiles())
                    {
                        file.Delete();
                    }
                }
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while uninstalling mod \"{Name}\": {ex.Message}");
                HSUtils.Log(ex.StackTrace);
                return;
            }
            HSUtils.Log($"Uninstalled mod \"{Name}\"");
        }

        public override void Download()
        {
            if (IsExperimental)
            {
                DownloadLatestBranch();
            }
            else
            {
                base.Download();
            }
        }

        public override void SetEnabled(bool isEnabled = true)
        {
            EditModsTxt(Name, isEnabled);
            if (HasLogicMods)
            {
                // We force-enable the BPModLoaderMod
                EditModsTxt(HSUtils.BPMLName, isEnabled);
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

            if (!File.Exists(HSUtils.HSUE4SSModsTxt))
            {
                // TODO should probably abort hard here, installation is broken
                HSUtils.Log($"[ERROR] Mods.txt is missing, cannot {(enabled ? "enable" : "disable")} mod {modName}");
                return;
            }

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
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Policy;

namespace HalfSwordModInstaller
{
    public class HSUE4SS : HSInstallable
    {
        public HSUE4SS() : base("UE4SS", "https://github.com/UE4SS-RE/RE-UE4SS")
        {
        }

        public new const string RelativePath = "";

        public new void LogMe()
        {
            HSUtils.Log($"UE4SS=\"{Name}\", Url=\"{Url}\", Version=\"{Version}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, " +
                $"InstalledVersion={(string.IsNullOrEmpty(InstalledVersion) ? "null" : "\"" + InstalledVersion + "\"")}, " +
                $"Enabled={IsEnabled}"
                );
        }

        public override void Install(bool forceInstallDependencies = false)
        {
            // We ignore dependencies as UE4SS is the first and has no dependencies
            Install();
        }

        public override void Install()
        {
            // TODO this is bad, we should probably fix it in another way
            if (IsInstalled)
            {
                Uninstall();
            }

            if (!IsDownloaded)
            {
                Download();
            }

            string unzipFolder = HSUtils.HSBinaryPath;
            HSUtils.ForceExtractToDirectory(LocalZipPath, unzipFolder);

            // patch "GraphicsAPI = opengl" in UE4SS-settings.ini
            var UE4SSSettingsIni = Path.Combine(HSUtils.HSBinaryPath, "UE4SS-settings.ini");
            var lines = File.ReadAllLines(UE4SSSettingsIni).ToList();
            int keyIndex = lines.FindIndex(line => line.StartsWith("GraphicsAPI = "));

            if (keyIndex != -1)
            {
                lines[keyIndex] = "GraphicsAPI = d3d11";
                File.WriteAllLines(UE4SSSettingsIni, lines);
            }
            // We probably need to refresh the detected values of IsInstalled, IsEnabled and also use the actual version somehow
            // TODO this is horrible, we are forcing the order of functions so that the version is detected before enabled status, etc.
            var temp1 = IsInstalled;
            var temp2 = IsEnabled;

            HSUtils.Log($"Installed UE4SS from \"{LocalZipPath}\" to \"{unzipFolder}\" {(temp1 && temp2 ? "successfully" : "with errors")}");
        }

        // We actually delete only the files that may affect the game's execution
        // Readmes and other things are left alone
        public override void Uninstall()
        {
            string[] filePaths =
            {
                "UE4SS.log",
                "README.md",
                "Changelog.md",
                "xinput1_3.dll",
                "dwmapi.dll",
                "UE4SS.dll",
                "xinput1_3.dll.bak",
                "dwmapi.dll.bak",
                "UE4SS.dll.bak",
                "UE4SS-settings.ini",
            };
            string[] dirPaths =
            {
                "Mods",
                "Cache"
            };
            foreach (var filePath in filePaths)
            {
                var fullFilePath = Path.Combine(HSUtils.HSBinaryPath, filePath);
                if (File.Exists(fullFilePath))
                {
                    try
                    {
                        File.Delete(fullFilePath);
                        HSUtils.Log($"File deleted: {fullFilePath}");
                    }
                    catch (IOException ioExp)
                    {
                        HSUtils.Log($"[ERROR] An error occurred while deleting the file [{fullFilePath}]: {ioExp.Message}");
                    }
                }
            }
            foreach (var dirPath in dirPaths)
            {
                var fullDirPath = Path.Combine(HSUtils.HSBinaryPath, dirPath);
                if (Directory.Exists(fullDirPath))
                {
                    try
                    {
                        // recursive delete
                        Directory.Delete(fullDirPath, true);
                        HSUtils.Log($"Folder deleted: {fullDirPath}");
                    }
                    catch (IOException ioExp)
                    {
                        HSUtils.Log($"[ERROR] An error occurred while deleting the folder [{fullDirPath}]: {ioExp.Message}");
                    }
                }
            }
            HSUtils.Log($"Uninstalled UE4SS");
        }

        public override void Update()
        {
            Uninstall();
            Install();
            HSUtils.Log($"Updated UE4SS");
        }

        public override bool IsInstalled
        {
            get
            {
                // xinput1_3.dll => UE4SS 2.5.2
                // dwmapi.dll + UE4SS.dll => UE4SS 3.*
                if (File.Exists(Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll"))
                    || File.Exists(Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll.bak")))
                {
                    _isInstalled = true;
                    InstalledVersion = "v2.5.2";
                    Version = InstalledVersion;
                }
                else if ((
                    File.Exists(Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll"))
                    || File.Exists(Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll.bak"))
                    )
                    && File.Exists(Path.Combine(HSUtils.HSBinaryPath, "UE4SS.dll")))
                {
                    _isInstalled = true;
                    // TODO dangerous, should probably detect a real version somehow
                    InstalledVersion = "v3.x.x";
                }
                else
                {
                    _isInstalled = false;
                    InstalledVersion = null;
                }

                return _isInstalled;
            }
            set
            {
                _isInstalled |= value;
            }
        }

        public override bool IsBroken 
        { 
            get
            {
                if (File.Exists(Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll"))
                    &&
                    (
                        File.Exists(Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll"))
                        || File.Exists(Path.Combine(HSUtils.HSBinaryPath, "UE4SS.dll"))
                    ))
                {
                    HSUtils.Log($"[ERROR] two versions of UE4SS are installed at the same time!");
                    InstalledVersion = null;
                    // TODO UE4SS installation is broken, both 2.5.2 and 3.x.x are present
                    // Must remove one of them somehow, and/or report that none are installed?
                    _isBroken = true;
                }

                return _isBroken;
            }
            
            set => _isBroken = value; 
        }

        public override bool IsEnabled
        {
            get
            {
                string dllBak, dllOK;

                if (Version == "v2.5.2")
                {
                    dllOK = Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll");
                    dllBak = Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll.bak");

                }
                else
                {
                    dllOK = Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll");
                    dllBak = Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll.bak");
                }

                if (File.Exists(dllOK))
                {
                    _isEnabled = true;
                }
                else if (File.Exists(dllBak))
                {
                    _isEnabled = false;
                }
                else
                {
                    // TODO what if not installed? Currently we assume that means not enabled, either
                    _isEnabled = false;
                }
                return _isEnabled;
            }
            set
            {
                _isEnabled |= value;
            }
        }

        public override void SetEnabled(bool isEnabled)
        {
            string dllBak, dllOK;

            if (!IsInstalled)
            {
                throw new Exception("UE4SS must be installed first!");
            }

            if (Version == "v2.5.2")
            {
                dllOK = Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll");
                dllBak = Path.Combine(HSUtils.HSBinaryPath, "xinput1_3.dll.bak");

            }
            else
            {
                dllOK = Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll");
                dllBak = Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll.bak");
            }
            if (isEnabled)
            {
                // Rename the backup filename into working filename
                if (File.Exists(dllBak) && !File.Exists(dllOK))
                {
                    File.Move(dllBak, dllOK);
                }
                else if (File.Exists(dllOK) && !File.Exists(dllBak))
                {
                    // OK, already enabled, nothing to do
                }
                else
                {
                    throw new Exception("UE4SS installation is broken!");
                }
                HSUtils.Log($"Enabled UE4SS");
            }
            else
            {
                if (File.Exists(dllBak) && !File.Exists(dllOK))
                {
                    // OK, already disabled, nothing to do
                }
                else if (File.Exists(dllOK) && !File.Exists(dllBak))
                {
                    File.Move(dllOK, dllBak);
                }
                else
                {
                    throw new Exception("UE4SS installation is broken!");
                }
                HSUtils.Log($"Disabled UE4SS");
            }
            _isEnabled = isEnabled;
        }

    }
}
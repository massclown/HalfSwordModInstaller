using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HalfSwordModInstaller
{
    public abstract class HSInstallable //: INotifyPropertyChanged
    {
        /* 
        // This is way overkill for what we need. Nope.
        // let's just refresh the datagridview when needed
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        */

        // This is a name without spaces
        public string Name { get; set; }
        // Github repo URL
        public string Url { get; set; }
        // A downloaded ZIP file of the mod stored locally
        protected string LocalZipPath;

        // The logic around Downloaded/Installed/Enabled is to enforce detection of the actual state on disk
        // The user may manually manipulate mods and we have to reflect that outside changes.
        // Downloaded means "latest known version downloaded"
        // TODO implement downloading of other versions
        protected bool _isDownloaded = false;
        public bool IsDownloaded
        {
            get
            {
                if (string.IsNullOrEmpty(LocalZipPath))
                {
                    if (!string.IsNullOrEmpty(LatestVersion))
                    {
                        string releaseZip = $"{Name}_{LatestVersion}.zip";
                        string downloadsFolder = Path.Combine(HSUtils.HSModInstallerDirPath, "downloads", Name, LatestVersion);
                        string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);

                        if (File.Exists(releaseZipPath))
                        {
                            _isDownloaded = true;
                            LocalZipPath = releaseZipPath;
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
                //OnPropertyChanged(nameof(IsDownloaded));
            }
        }
        protected bool _isInstalled = false;
        public virtual bool IsInstalled { get; set; }

        protected bool _isBroken = false;
        public virtual bool IsBroken { get; set; }

        protected bool _isEnabled = false;
        public virtual bool IsEnabled { get; set; }

        protected string RelativePath = string.Empty;
        // Latest known version from the internet
        public string LatestVersion { get; set; }
        // For rare cases when we can infer installed version from disk
        public string InstalledVersion { get; set; }

        public List<HSInstallable> dependencyGraph;

        public HSInstallable(string name, string url, List<HSInstallable> dependencyGraph = null)
        {
            this.Name = name;
            this.Url = url;
            this.LatestVersion = GetLatestVersion();
            this.dependencyGraph = dependencyGraph;
        }

        public void LogMe()
        {
            HSUtils.Log($"Installable object=\"{Name}\", Url=\"{Url}\", Version=\"{LatestVersion}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, Enabled={IsEnabled}"
                );
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
            try
            {
                string releaseUrl = GetRedirectUrl(latestUrl);
                string tag = releaseUrl.Substring(releaseUrl.LastIndexOf("/") + 1);
                return tag;
            }
            catch (Exception e)
            {
                HSUtils.Log($"[ERROR] Checking version of mod \"{Name}\" from \"{latestUrl}\" failed:");
                HSUtils.Log(e.Message);
                HSUtils.Log(e.StackTrace);
                return null;
            }
        }

        // Always retrieves latest version and downloads it
        public void Download()
        {
            string tag = GetLatestVersion();
            if (tag != null)
            {
                Download(tag);
            }
            else
            {
                HSUtils.Log($"[ERROR] Aborting download of mod \"{Name}\", unknown version");
            }
        }

        // Download specific version by known tag
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
                try
                {
                    client.DownloadFile(downloadUrl, releaseZipPath);
                    LocalZipPath = releaseZipPath;
                    _isDownloaded = true;
                    LatestVersion = tag;
                    HSUtils.Log($"Downloaded mod \"{Name}\" from \"{downloadUrl}\" to \"{releaseZipPath}\"");
                }
                catch (Exception e)
                {
                    HSUtils.Log($"[ERROR] Downloading of mod \"{Name}\" from \"{downloadUrl}\" to \"{releaseZipPath}\" failed:");
                    HSUtils.Log(e.Message);
                    HSUtils.Log(e.StackTrace);
                }
            }
        }

        public abstract void Install();
        public virtual void InstallAll()
        {
            Install(true);
        }

        public abstract void Install(bool forceInstallDependencies = false);
        public void Install(string tag)
        {
            Download(tag);
            Install();
        }

        public virtual void Uninstall()
        {
            throw new NotImplementedException();
        }

        public virtual void Update()
        {
            throw new NotImplementedException();
        }

        public virtual void SetEnabled(bool isEnabled = true)
        {
            throw new NotImplementedException();
        }

        public virtual bool GetEnabled()
        {
            throw new NotImplementedException();
        }

    }
}

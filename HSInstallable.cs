using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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

        public string ReleaseArtifactName;

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
                        string downloadsFolder = Path.Combine(HSUtils.HSModInstallerDirPath, "downloads", Name, LatestVersion);
                        if (!IsExperimental)
                        {
                            string releaseZip = $"{Name}_{LatestVersion}.zip";
                            string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);

                            if (File.Exists(releaseZipPath))
                            {
                                _isDownloaded = true;
                                LocalZipPath = releaseZipPath;
                                return _isDownloaded;
                            }
                        }
                        else // experimental
                        {
                            if (string.IsNullOrEmpty(ReleaseArtifactName))
                            {
                                // very rough guess
                                if (Directory.Exists(downloadsFolder) && Directory.GetFiles(downloadsFolder).Length > 0)
                                {
                                    _isDownloaded = true;
                                    LocalZipPath = Directory.GetFiles(downloadsFolder).First();
                                    return _isDownloaded;
                                }
                            }
                            else
                            {
                                string releaseZip = ReleaseArtifactName;
                                string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);
                                if (File.Exists(releaseZipPath))
                                {
                                    _isDownloaded = true;
                                    LocalZipPath = releaseZipPath;
                                    return _isDownloaded;
                                }
                            }
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

        protected HSUtils.HSGameType _compatibleGameType;
        // TODO add validation here
        public virtual HSUtils.HSGameType CompatibleGameType
        {
            get
            {
                return _compatibleGameType;
            }
            set
            {
                _compatibleGameType = value;
                if (value == HSUtils.HSGameType.Playtest)
                {
                    // technically this only applies to UE4SS, not mods?
                    //_isExperimental = true;
                }
                else if (value == HSUtils.HSGameType.Demo)
                {
                    //_isExperimental = false;
                }
            }
        }

        protected bool _isExperimental = false;
        public virtual bool IsExperimental {
            get
            {
                return _isExperimental;
            }
            set {
                var forceRefresh = false;
                if (value != _isExperimental)
                {
                    forceRefresh = true;
                    IsDownloaded = false;
                    IsInstalled = false;
                    IsEnabled = false;
                }
                _isExperimental |= value;
                LatestVersion = GetLatestVersion();
            }
        }
        // This is supposed to be a relative path from the game binary path
        // For mods in old UE4SS this was Mods/<ModName>,
        // for new UE4SS this is ue4ss/Mods/<ModName>.
        // For UE4SS this is always empty as we unzip directly to game binary directory.
        protected string RelativePath = string.Empty;
        // Latest known version from the internet
        public string LatestVersion { get; set; }
        // For rare cases when we can infer installed version from disk
        public string InstalledVersion { get; set; }

        public List<HSInstallable> dependencyGraph;

        public HSInstallable(string name, string url, HSUtils.HSGameType compatibleGameType, List<HSInstallable> dependencyGraph = null)
        {
            this.Name = name;
            this.Url = url;
            this.CompatibleGameType = compatibleGameType;
            this.LatestVersion = GetLatestVersion();
            this.dependencyGraph = dependencyGraph;
        }

        public void LogMe()
        {
            HSUtils.Log($"Installable object=\"{Name}\", Url=\"{Url}\", Version=\"{LatestVersion}\", " +
                $"Experimental=\"{IsExperimental}\", " +
                $"CompatibleGameType=\"{CompatibleGameType}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, " +
                $"InstalledVersion={(string.IsNullOrEmpty(InstalledVersion) ? "null" : "\"" + InstalledVersion + "\"")}, " +
                $"Enabled={IsEnabled}"
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
            try
            {
                // Using the internal property _isExperimental as this gets called in the setter of IsExperimental, yes, this is bad
                if (_isExperimental)
                {
                    string sha = GetLatestCommit(Url);
                    if (sha != null)
                    {
                        string tag = sha.Substring(0, 7);
                        return tag;
                    }
                    return "error";
                }
                else
                {
                    string latestUrl = Url + "/releases/latest";
                    string releaseUrl = GetRedirectUrl(latestUrl);
                    string tag = releaseUrl.Substring(releaseUrl.LastIndexOf("/") + 1);
                    return tag;
                }
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] Checking version of mod \"{Name}\" from \"{Url}\" failed:");
                HSUtils.Log(ex.Message);
                HSUtils.Log(ex.StackTrace);
                return null;
            }
        }

        public virtual string GetLatestCommit(string repoUrl)
        {
            HttpClient client = new HttpClient();

            // Extract the repo owner and name from the URL
            var uri = new Uri(repoUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                HSUtils.Log($"[ERROR] Invalid GitHub repository URL \"{repoUrl}\"");
                return null;
            }

            string repoOwner = segments[1].TrimEnd('/');
            string repoName = segments[2].TrimEnd('/');

            string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/commits";

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var serializer = new JavaScriptSerializer();
                var json = serializer.Deserialize<dynamic>(responseBody);

                var latestCommit = json[0];
                var commitSha = latestCommit["sha"];
//                var commitMessage = latestCommit["commit"]["message"];
//                var commitDate = latestCommit["commit"]["committer"]["date"];

                return commitSha;
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] Checking latest commit version from \"{repoUrl}\" failed:");
                HSUtils.Log(ex.Message);
                HSUtils.Log(ex.StackTrace);
                return null;
            }
        }


        // Always retrieves latest version and downloads it
        public virtual void Download()
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

        // Download specific release version by known tag
        public void Download(string tag)
        {
            // Example:
            // HalfSwordTrainerMod_v0.8.zip
            string releaseZip = $"{Name}_{tag}.zip";
            ReleaseArtifactName = releaseZip;
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
                catch (Exception ex)
                {
                    HSUtils.Log($"[ERROR] Downloading of mod \"{Name}\" from \"{downloadUrl}\" to \"{releaseZipPath}\" failed:");
                    HSUtils.Log(ex.Message);
                    HSUtils.Log(ex.StackTrace);
                }
            }
        }

        // Download latest main branch, set tag to latest truncated commit hash
        public void DownloadLatestBranch()
        {
            // Example:
            // main.zip
            string releaseZip = $"main.zip";
            ReleaseArtifactName = releaseZip;
            string tag = GetLatestVersion();
            // Example:
            // https://github.com/UE4SS-RE/RE-UE4SS/archive/refs/heads/main.zip
            string downloadUrl = $"{Url}/archive/refs/heads/{releaseZip}";

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
                catch (Exception ex)
                {
                    HSUtils.Log($"[ERROR] Downloading of mod \"{Name}\" from \"{downloadUrl}\" to \"{releaseZipPath}\" failed:");
                    HSUtils.Log(ex.Message);
                    HSUtils.Log(ex.StackTrace);
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

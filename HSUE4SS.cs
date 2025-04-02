using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Policy;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net;
using System.Collections.Generic;
using System.Windows.Forms;

namespace HalfSwordModInstaller
{
    public class HSUE4SS : HSInstallable
    {
        public override HSUtils.HSGameType CompatibleGameType
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
                    // technically this only applies to UE4SS
                    _isExperimental = true;
                }
                else if (value == HSUtils.HSGameType.Demo)
                {
                    _isExperimental = false;
                }
                else if (value == HSUtils.HSGameType.Demo04)
                {
                    _isExperimental = true;
                }
            }
        }

        public HSUE4SS() : base("UE4SS", "https://github.com/UE4SS-RE/RE-UE4SS", HSUtils.HSGameType.Demo04)
        {
        }

        public HSUE4SS(HSUtils.HSGameType compatibleGameType) : base("UE4SS", "https://github.com/UE4SS-RE/RE-UE4SS", compatibleGameType)
        {
            // This handles the basic distinction between the two versions of UE4SS for demo (stable UE4SS) and playtest (experimental UE4SS)
        }


        public HSUE4SS(string name, string url, HSUtils.HSGameType compatibleGameType, List<HSInstallable> dependencyGraph = null) : base (name, url, compatibleGameType, dependencyGraph )
        {
            // just a full constructor for whatever reason
        }

        public new const string RelativePath = "";

        public bool IsDevBuild { 
            get
            {
                return IsDevBuildInstalled();
            }
            set
            {
                IsDevBuild = value;
            }
        }

        public new void LogMe()
        {
            HSUtils.Log($"UE4SS=\"{Name}\", Url=\"{Url}\", LatestVersion=\"{LatestVersion}\", " +
                $"Experimental=\"{IsExperimental}\", " +
                $"CompatibleGameType=\"{CompatibleGameType}\", " +
                $"Downloaded={IsDownloaded}, Installed={IsInstalled}, " +
                $"InstalledVersion={(string.IsNullOrEmpty(InstalledVersion) ? "null" : "\"" + InstalledVersion + "\"")}, " +
                $"DevBuild=\"{IsDevBuild}\", " +
                $"Enabled={IsEnabled}"
                );
        }

        public void DownloadLatestExperimental()
        {
            // Example: Latest commit on main: g4fc8691
            // Example: https://github.com/UE4SS-RE/RE-UE4SS/releases/download/experimental/UE4SS_v3.0.1-234-g4fc8691.zip
            // Latest workflow run result at: 
            // https://api.github.com/repos/UE4SS-RE/RE-UE4SS/actions/workflows/experimental.yml/runs?status=success&per_page=1
            // 
            var commitSha = GetCommitHashOfLatestSuccessfulRun(Url);
            var shortSha = commitSha.Substring(0, 7);
            HttpClient client = new HttpClient();
            try
            {
                var uri = new Uri(Url);
                var segments = uri.Segments;
                if (segments.Length < 3)
                {
                    HSUtils.Log($"[ERROR] Invalid GitHub repository URL \"{Url}\"");
                    return;
                }

                string repoOwner = segments[1].TrimEnd('/');
                string repoName = segments[2].TrimEnd('/');
                string releaseTag = "experimental";
                string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/releases/tags/{releaseTag}";

                client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                response.EnsureSuccessStatusCode();

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var serializer = new JavaScriptSerializer();
                var json = serializer.Deserialize<dynamic>(responseBody);

                var assets = json["assets"];
                foreach (var asset in assets)
                {
                    var fileName = asset["name"];
                    Match assetNameMatch = Regex.Match(fileName, $@"^{(IsDevBuild ? "zDEV-":"")}UE4SS_v(.*)-g{shortSha}.zip$");

                    if (assetNameMatch.Success)
                    {
                        var downloadUrl = asset["browser_download_url"];
                        HSUtils.Log($"File found: {fileName}\nDownload URL: {downloadUrl}");
                        var url = new Uri(downloadUrl);

                        var releaseZip = fileName;

                        string downloadsFolder = Path.Combine(HSUtils.HSModInstallerDirPath, "downloads", Name, shortSha);
                        if (!Directory.Exists(downloadsFolder))
                        {
                            Directory.CreateDirectory(downloadsFolder);
                        }
                        string releaseZipPath = Path.Combine(downloadsFolder, releaseZip);
                        using (var dclient = new WebClient())
                        {
                            try
                            {
                                dclient.DownloadFile(downloadUrl, releaseZipPath);
                                LocalZipPath = releaseZipPath;
                                _isDownloaded = true;
                                LatestVersion = shortSha;
                                ReleaseArtifactName = releaseZip;
                                HSUtils.Log($"Downloaded UE4SS experimental from \"{downloadUrl}\" to \"{releaseZipPath}\"");
                            }
                            catch (Exception ex)
                            {
                                HSUtils.Log($"[ERROR] Downloading of UE4SS experimental from \"{downloadUrl}\" to \"{releaseZipPath}\" failed:");
                                HSUtils.Log(ex.Message);
                                HSUtils.Log(ex.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                HSUtils.Log($"[ERROR] A HTTP request error occurred while obtaining experimental UE4SS build in: {e.Message}");
                HSUtils.Log(e.StackTrace);
            }
        }

        public static string GetCommitHashOfLatestSuccessfulRun(string repoUrl)
        {
            HttpClient client = new HttpClient();
            string workflowPath = "experimental.yml";

            var uri = new Uri(repoUrl);
            var segments = uri.Segments;
            if (segments.Length < 3)
            {
                HSUtils.Log($"[ERROR] Invalid GitHub repository URL \"{repoUrl}\"");
                return null;
            }

            string repoOwner = segments[1].TrimEnd('/');
            string repoName = segments[2].TrimEnd('/');

            string apiUrl = $"https://api.github.com/repos/{repoOwner}/{repoName}/actions/workflows/{workflowPath}/runs?status=success&per_page=1";

            client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("CSharpApp", "1.0"));
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                if (response.StatusCode == HttpStatusCode.Forbidden && response.ReasonPhrase == "rate limit exceeded")
                {
                    HSUtils.Log($"[ERROR] GitHub API rate limit exceeded, please wait at least 1 hour and try again.");
                    MessageBox.Show("GitHub API rate limit exceeded, please wait at least 1 hour and try again", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
                response.EnsureSuccessStatusCode();

                string responseBody = response.Content.ReadAsStringAsync().Result;
                var serializer = new JavaScriptSerializer();
                var json = serializer.Deserialize<dynamic>(responseBody);

                var workflowRun = json["workflow_runs"][0];
                //var runId = workflowRun["id"];
                //var conclusion = workflowRun["conclusion"];
                //var runDate = workflowRun["created_at"];
                //var run_number = workflowRun["run_number"];
                var head_sha = workflowRun["head_sha"];
                return head_sha;
            }
            catch (HttpRequestException e)
            {
                HSUtils.Log($"[ERROR] A HTTP request error occurred while obtaining experimental UE4SS build in: {e.Message}");
                HSUtils.Log(e.StackTrace);
                return null;
            }
            catch (Exception e)
            {
                HSUtils.Log($"[ERROR] An error occurred while obtaining experimental UE4SS build in: {e.Message}");
                HSUtils.Log(e.StackTrace);
                return null;
            }
        }

        public override string GetLatestCommit(string repoUrl)
        {
            return GetCommitHashOfLatestSuccessfulRun(repoUrl);
        }

        public override void Download()
        {
            if (IsExperimental)
            {
                DownloadLatestExperimental();
            }
            else
            {
                base.Download();
            }
        }

        public override void Install(bool forceInstallDependencies = false)
        {
            // We ignore dependencies as UE4SS is the first and has no dependencies
            Install();
        }

        public override void Install()
        {
            HSUtils.Log($"Installing UE4SS...");
            // TODO this is bad, we should probably fix it in another way
            if (IsInstalled)
            {
                HSUtils.Log($"UE4SS already installed, will uninstall first");
                Uninstall();
            }

            if (!IsDownloaded)
            {
                Download();
            }

            string unzipFolder = HSUtils.HSBinaryPath;
            try
            {
                HSUtils.ForceExtractToDirectory(LocalZipPath, unzipFolder);
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while installing UE4SS from \"{LocalZipPath}\" to \"{unzipFolder}\": {ex.Message}");
                HSUtils.Log(ex.StackTrace);
                return;
            }
            // patch "GraphicsAPI = opengl" into d3d11 in UE4SS-settings.ini\
            // No idea which is the correct one, d3d11 or dx11, but opengl surely leads to a white screen in UE4SS, so patching
            var UE4SSSettingsIni = HSUtils.HSUE4SSSettingsIni;
            try
            {
                var lines = File.ReadAllLines(UE4SSSettingsIni).ToList();
                int keyIndex = lines.FindIndex(line => line.StartsWith("GraphicsAPI = "));

                if (keyIndex != -1)
                {
                    lines[keyIndex] = "GraphicsAPI = d3d11";
                    File.WriteAllLines(UE4SSSettingsIni, lines);
                }
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while configuring UE4SS in \"{UE4SSSettingsIni}\": {ex.Message}");
                HSUtils.Log(ex.StackTrace);
                return;
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
            HSUtils.Log($"Uninstalling UE4SS...");
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
                "ue4ss", // new version of UE4SS, wipes mods too as they are in ue4ss/Mods
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
            HSUtils.Log($"Updating UE4SS...");
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
                    // Version = InstalledVersion;
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
                    // Version = InstalledVersion;
                }
                else if ((
                    File.Exists(Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll"))
                    || File.Exists(Path.Combine(HSUtils.HSBinaryPath, "dwmapi.dll.bak"))
                    )
                    && Directory.Exists(Path.Combine(HSUtils.HSBinaryPath, "ue4ss"))
                    && File.Exists(Path.Combine(HSUtils.HSBinaryPath, "ue4ss", "UE4SS.dll"))
                )
                {
                    // This is a new version of UE4SS, where the DLL is in a subfolder
                    _isInstalled = true;
                    // TODO dangerous, should probably detect a real version somehow
                    InstalledVersion = "v3.x.x";
                    // Version = InstalledVersion;
                    // TODO validate against isExperimental / compatibleGameType
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

        public bool IsDevBuildInstalled()
        {
            return File.Exists(Path.Combine(HSUtils.HSBinaryPath, "UE4SS.pdb"));
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
                        || File.Exists(Path.Combine(HSUtils.HSBinaryPath, "ue4ss", "UE4SS.dll"))
                    )
                )
                {
                    HSUtils.Log($"[ERROR] two versions of UE4SS are installed at the same time!");
                    InstalledVersion = null;
                    // TODO UE4SS installation is broken, both 2.5.2 and 3.x.x are present
                    // Must remove one of them somehow, and/or report that none are installed?
                    _isBroken = true;
                }

                return _isBroken;
            }

            set
            {
                _isBroken = value;
            }
        }

        public override bool IsEnabled
        {
            get
            {
                string dllBak, dllOK;

                if (InstalledVersion == "v2.5.2")
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
                _isEnabled = value;
            }
        }

        public override void SetEnabled(bool isEnabled)
        {
            string dllBak, dllOK;

            if (!IsInstalled)
            {
                throw new Exception("UE4SS must be installed first!");
            }

            if (InstalledVersion == "v2.5.2")
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
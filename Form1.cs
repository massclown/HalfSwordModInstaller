using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HalfSwordModInstaller
{
    public partial class Form1 : Form
    {
        public BindingList<HSInstallable> mods;
        private List<DataGridViewColumn> dataGridView1ColumnsBackup;

        public Form1()
        {
            InitializeComponent();
            // We set the assembly title and version during build in msbuild's "BeforeBuild" target inside *.csproj to a msbuild command-line property
            // (see https://stackoverflow.com/a/58631756 for reference)
            // These variables are set to sane defaults in the begining of *.csproj to ensure MSVS can build the project from the GUI
            // and later overriden in our github actions to contain github tag (we build on a new tag == release)
            // so for a tag "v0.2" we can have "Half Sword Mod Installer v0.2" as the titles and version "0.2.0.0" (dotnet versioning must be x.x.x.x)
            var assemblyTitle = System.Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>().Take(1).FirstOrDefault().Title;
            this.Text = assemblyTitle;
        }

        // This uninstalls everything, starting from UE4SS
        private void EasyUninstall()
        {
            try
            {
                var UE4SS = (HSUE4SS)mods.SingleOrDefault(elem => elem.Name == "UE4SS");
                UE4SS.Uninstall();

            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while easy-uninstalling: {ex.Message}");
                HSUtils.Log(ex.StackTrace);
            }
        }

        private void populateMods()
        {
            RestoreDataGridView1Columns();
            // TODO have the list of mods downloaded from somewhere. Hardcoding it for now
            // TODO define the dependencies by name instead of by actual HSInstallable object

            mods = new BindingList<HSInstallable>();
            HSUE4SS UE4SS = new HSUE4SS(HSUtils.ChosenGameType);
            mods.Add(UE4SS);
            UE4SS.LogMe();

            if (HSUtils.ChosenGameType == HSUtils.HSGameType.Demo)
            {
                HSMod HSTM = new HSMod("HalfSwordTrainerMod", "https://github.com/massclown/HalfSwordTrainerMod", HSUtils.HSGameType.Demo, true, new List<HSInstallable>() { UE4SS });
                mods.Add(HSTM);
                HSTM.LogMe();
                HSMod HSSSM = new HSMod("HalfSwordSplitScreenMod", "https://github.com/massclown/HalfSwordSplitScreenMod", HSUtils.ChosenGameType, false, new List<HSInstallable>() { UE4SS });
                mods.Add(HSSSM);
                HSSSM.LogMe();
            }
            else if (HSUtils.ChosenGameType == HSUtils.HSGameType.Playtest)
            {
                HSMod HSTM = new HSMod("HalfSwordTrainerMod", "https://github.com/massclown/HalfSwordTrainerMod-playtest", HSUtils.HSGameType.Playtest, false, new List<HSInstallable>() { UE4SS });
                mods.Add(HSTM);
                HSTM.LogMe();
            }

            bindingSource1.DataSource = mods;

            var firstButtonColumnIndex = 3;

            var downloadButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "downloadButton",
                HeaderText = "Download?",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Download"
                },
                ToolTipText = "Download the latest version of this mod"
            };
            this.dataGridView1.Columns.Insert(firstButtonColumnIndex, downloadButtonColumn);

            var installButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "installButton",
                HeaderText = "Install?",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Install/Uninstall"
                },
                ToolTipText = "Install or uninstall this mod"
            };
            this.dataGridView1.Columns.Insert(firstButtonColumnIndex + 2, installButtonColumn);

            var enableButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "enableButton",
                HeaderText = "Enable?",
                UseColumnTextForButtonValue = false,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Enable/Disable"
                },
                ToolTipText = "Enable or disable this mod"
            };
            this.dataGridView1.Columns.Insert(firstButtonColumnIndex + 4, enableButtonColumn);

            //bindingSource1?.ResetBindings(false);
            //dataGridView1.Refresh();
            StatusStipTextUpdate();

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BackupDataGridView1Columns();
            // By this time, HSUtils should have detected the installed games and asked the user to select the correct one in case of doubt
            this.radioButton1demo.Checked = HSUtils.ChosenGameType == HSUtils.HSGameType.Demo;
            this.radioButton2playtest.Checked = HSUtils.ChosenGameType == HSUtils.HSGameType.Playtest;
            // TODO this is bad, but we don't have a better path to extract the Steam and Half Sword installation state
            string HSPath = HSUtils.HSBinaryPath;
            if (HSPath == null)
            {
                MessageBox.Show("Could not find Steam or Half Sword Demo, exiting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                // Not really useful, but still
                return;
            }
            else
            {
                HSUtils.Log($"Steam install path found for Half Sword: \"{HSPath}\"");
            }

            if (HSUtils.IsRunningAsAdmin())
            {
                HSUtils.Log($"[WARNING] Installer running as admin!");
            }

            if (HSUtils.IsHalfSwordRunning())
            {
                MessageBox.Show($"Half Sword is running, please exit the game and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            if (HSUtils.IsAnotherInstallerRunning())
            {
                MessageBox.Show($"Another mod installer is running, please exit that installer and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            populateMods();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // This is to handle click on checkboxes for IsExperimental
            bool skipRefresh = false;
            if (e.RowIndex < dataGridView1.RowCount && e.RowIndex >= 0 &&
                e.ColumnIndex < dataGridView1.ColumnCount && e.ColumnIndex >= 0)
            {
                var currentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                var row = dataGridView1.Rows[e.RowIndex];
                var mod = (HSInstallable)row.DataBoundItem;

                if (currentCell is DataGridViewButtonCell buttonCell)
                {
                    if (HSUtils.IsRunningAsAdmin())
                    {
                        if (MessageBox.Show($"You ran the installer as administrator.\nAre you sure you want that?", "Confirm",
                            MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                        {
                            return;
                        }
                    }

                    if (HSUtils.IsHalfSwordRunning())
                    {
                        MessageBox.Show($"Half Sword is running, please exit the game and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (HSUtils.IsAnotherInstallerRunning())
                    {
                        MessageBox.Show($"Another mod installer is running, please exit that installer and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }


                    switch (dataGridView1.Columns[e.ColumnIndex].Name)
                    {
                        case "downloadButton":
                            if (mod.IsDownloaded)
                            {
                                // TODO re-download or not? Just re-download for now.
                                backgroundWorker1.RunWorkerAsync(new Action(mod.Download));
                            }
                            else
                            {
                                backgroundWorker1.RunWorkerAsync(new Action(mod.Download));
                            }
                            break;
                        case "installButton":
                            if (mod.IsInstalled)
                            {
                                // TODO re-install or not? For now, uninstall.
                                if (MessageBox.Show($"Really uninstall {mod.Name}?", "Confirm uninstallation",
                                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                                    MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
                                {
                                    backgroundWorker1.RunWorkerAsync(new Action(mod.Uninstall));
                                }
                            }
                            else
                            {
                                if (mod.dependencyGraph?.Count > 0)
                                {
                                    // TODO should we throw an exception here? Or should we simply install the dependency?
                                    foreach (var dependency in mod.dependencyGraph)
                                    {
                                        if (!dependency.IsInstalled)
                                        {
                                            MessageBox.Show($"This mod needs {dependency.Name} to be able to run.\nPlease install {dependency.Name} first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                            continue;
                                        }
                                    }
                                }

                                backgroundWorker1.RunWorkerAsync(new Action(mod.Install));
                            }
                            break;
                        case "enableButton":
                            if (!mod.IsInstalled)
                            {
                                // TODO Ask to install it first and abort.
                                MessageBox.Show($"Please install {mod.Name} first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            else
                            {
                                if (mod.IsEnabled)
                                {
                                    mod.SetEnabled(false);
                                }
                                else
                                {
                                    mod.SetEnabled(true);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
                else if (currentCell is DataGridViewLinkCell linkCell)
                {
                    var cellUrl = (string)linkCell.Value;
                    if (Uri.IsWellFormedUriString(cellUrl, UriKind.Absolute))
                    {
                        Process.Start(cellUrl);
                    }
                }
                else if (currentCell is DataGridViewCheckBoxCell checkboxCell)
                {
                    //skipRefresh = true; 
                }
            }
            if (!skipRefresh)
            {
                bindingSource1?.ResetBindings(false);
                dataGridView1.Refresh();
                StatusStipTextUpdate();
            }
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (var i = e.RowIndex; i <= e.RowIndex + e.RowCount - 1; i++)
            {
                var row = dataGridView1.Rows[i];
                var item = row.DataBoundItem as HSInstallable;
                if (item == null) continue;
                for (var j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    if (dataGridView1.Columns[j].DataPropertyName.StartsWith("Is"))
                    {
                        if (dataGridView1.Columns[j].DataPropertyName == "IsExperimental")
                        {
                            row.Cells[j].Style.BackColor = ((bool)row.Cells[j].Value) ? Color.PaleVioletRed : Color.LightGreen;
                        }
                        else
                        {
                            row.Cells[j].Style.BackColor = ((bool)row.Cells[j].Value) ? Color.LightGreen : Color.PaleVioletRed;
                        }
                    }
                }
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;
            Action action = (Action)e.Argument;
            action.Invoke();
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bindingSource1?.ResetBindings(false);
            dataGridView1.Refresh();
            Cursor.Current = Cursors.Default;
        }

        // This is the easy install procedure for UE4SS and trainer mod
        private void buttonEasyInstall_Click(object sender, EventArgs e)
        {
            HSUtils.Log($"Running easy install...");

            var oldText = buttonEasyInstall.Text;
            try
            {
                if (HSUtils.IsRunningAsAdmin())
                {
                    if (MessageBox.Show($"You ran the installer as administrator.\nAre you sure you want that?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        return;
                    }
                }

                buttonEasyInstall.Text = "Working, please wait...";

                if (HSUtils.IsHalfSwordRunning())
                {
                    MessageBox.Show($"Half Sword is running, please exit the game and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonEasyInstall.Text = oldText;
                    return;
                }

                if (HSUtils.IsAnotherInstallerRunning())
                {
                    MessageBox.Show($"Another mod installer is running, please exit that installer and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    buttonEasyInstall.Text = oldText;
                    return;
                }

                var HSUE4SS = (HSUE4SS)mods.SingleOrDefault(elem => elem.Name == "UE4SS");
                if (HSUE4SS.IsBroken)
                {
                    if (MessageBox.Show($"Broken UE4SS installation detected.\nRepair UE4SS?", "Confirm installation repair",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        buttonEasyInstall.Text = oldText;
                        return;
                    }
                    HSUE4SS.Uninstall();
                    HSUE4SS.IsBroken = false;
                    // We don't reinstall UE4SS here as the mod logic will install it anyway later
                }

                var HSTM = (HSMod)mods.SingleOrDefault(elem => elem.Name == "HalfSwordTrainerMod");
                if (HSTM.IsInstalled)
                {
                    // TODO: should probably show the installed and new versions
                    if (MessageBox.Show($"Really update the mod?", "Confirm update",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        buttonEasyInstall.Text = oldText;
                        return;
                    }

                }
                Cursor.Current = Cursors.WaitCursor;

                HSTM.Download();
                HSTM.InstallAll();
                HSTM.SetEnabled(true);

                buttonEasyInstall.Text = oldText;
                StatusStipTextUpdate();
                bindingSource1?.ResetBindings(false);
                dataGridView1.Refresh();
                Cursor.Current = Cursors.Default;
                HSUtils.Log($"Easy install finished");
                MessageBox.Show($"Half Sword Trainer Mod successfully installed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                HSUtils.Log($"[ERROR] An error occurred while easy-installing: {ex.Message}");
                HSUtils.Log(ex.StackTrace);
                // TODO: maybe ask the user to upload the log file somewhere?
                buttonEasyInstall.Text = oldText;
                MessageBox.Show($"Sorry, something went wrong.\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUninstallAll_Click(object sender, EventArgs e)
        {
            HSUtils.Log($"Easy uninstall started");
            if (HSUtils.IsHalfSwordRunning())
            {
                MessageBox.Show($"Half Sword is running, please exit the game and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (HSUtils.IsAnotherInstallerRunning())
            {
                MessageBox.Show($"Another mod installer is running, please exit that installer and try again!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (HSUtils.IsRunningAsAdmin())
            {
                if (MessageBox.Show($"You ran the installer as administrator.\nAre you sure you want that?", "Confirm",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
            }

            if (MessageBox.Show($"Really uninstall everything?", "Confirm uninstallation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {

                EasyUninstall();
                bindingSource1?.ResetBindings(false);
                dataGridView1.Refresh();
                StatusStipTextUpdate();
                HSUtils.Log($"Easy uninstall finished");
            }
        }

        private void buttonInstallerLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(HSUtils.HSModInstallerLogFilePath))
            {
                Process.Start(HSUtils.HSModInstallerLogFilePath);
            }
            else
            {
                MessageBox.Show($"File does not exist yet", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonUE4SSLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(HSUtils.HSUE4SSlog))
            {
                Process.Start(HSUtils.HSUE4SSlog);
            }
            else
            {
                MessageBox.Show($"File does not exist yet", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonCopyInstallerLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(HSUtils.HSModInstallerLogFilePath))
            {
                Clipboard.SetText(HSUtils.HSModInstallerLogFilePath);
            }
            else
            {
                MessageBox.Show($"File does not exist yet", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonCopyUE4SSLog_Click(object sender, EventArgs e)
        {
            if (File.Exists(HSUtils.HSUE4SSlog))
            {
                Clipboard.SetText(HSUtils.HSUE4SSlog);
            }
            else
            {
                MessageBox.Show($"File does not exist yet", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void toolStripStatusLabel_Click(object sender, EventArgs e)
        {

        }

        public void StatusStipTextUpdate()
        {
            this.toolStripStatusLabel.Text = "";
            foreach (var mod in this.mods)
            {
                if (mod.IsInstalled)
                {
                    this.toolStripStatusLabel.Text += $"{mod.Name} version {mod.InstalledVersion} installed and {(mod.IsEnabled ? "" : "not ")}enabled. ";
                }
            }
            if (this.toolStripStatusLabel.Text.Length == 0)
            {
                this.toolStripStatusLabel.Text = "Half Sword " + (HSUtils.ChosenGameType == HSUtils.HSGameType.Demo? "Demo" : "Playtest") + " ready, no mods";
            }
        }

        public void ConfirmAndRepair()
        {
            var HSUE4SS = mods.SingleOrDefault(elem => elem.Name == "UE4SS");
            if (HSUE4SS.IsBroken)
            {
                if (MessageBox.Show($"Broken UE4SS installation detected.\nRepair UE4SS?", "Confirm installation repair",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                {
                    return;
                }
                HSUE4SS.Uninstall();
                HSUE4SS.Download();
                HSUE4SS.Install();
                HSUE4SS.IsBroken = false;
                StatusStipTextUpdate();
            }
        }

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            ConfirmAndRepair();
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            var image = this.pictureBox1.Image;
            image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            this.pictureBox1.Image = image;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < dataGridView1.RowCount && e.RowIndex >= 0 &&
                e.ColumnIndex < dataGridView1.ColumnCount && e.ColumnIndex >= 0)
            {
                var currentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                var row = dataGridView1.Rows[e.RowIndex];
                var mod = (HSInstallable)row.DataBoundItem;

                if (currentCell is DataGridViewCheckBoxCell checkboxCell)
                {
                    bindingSource1?.ResetBindings(false);
                    dataGridView1.Refresh();
                    StatusStipTextUpdate();
                }
            }
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            handleGameTypeChange(sender, e);
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            handleGameTypeChange(sender, e);
        }

        private void handleGameTypeChange(object sender, EventArgs e)
        {
            if (radioButton1demo.Checked)
            {
                HSUtils.ChosenGameType = HSUtils.HSGameType.Demo;
            }
            else
            {
                HSUtils.ChosenGameType = HSUtils.HSGameType.Playtest;
            }
            // TODO make sure HSUtils internals are updated
            HSUtils.Log($"Game type changed to {HSUtils.ChosenGameType}");
            HSUtils.recalculateGamePaths();
            this.populateMods();
            StatusStipTextUpdate();
        }

        private void BackupDataGridView1Columns()
        {
            dataGridView1ColumnsBackup = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                dataGridView1ColumnsBackup.Add(column);
            }
        }

        private void RestoreDataGridView1Columns()
        {
            if (dataGridView1ColumnsBackup == null) return;

            dataGridView1.Columns.Clear();
            foreach (var column in dataGridView1ColumnsBackup)
            {
                dataGridView1.Columns.Add(column);
            }
        }
    }
}

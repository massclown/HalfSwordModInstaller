using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HalfSwordModInstaller
{
    public partial class Form1 : Form
    {
        public BindingList<HSInstallable> mods;

        public Form1()
        {
            InitializeComponent();
        }

        private void EasyUninstall()
        {
            var UE4SS = mods.SingleOrDefault(elem => elem.Name == "UE4SS");
            UE4SS.Uninstall();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                string HSPath = HSUtils.HSBinaryPath;
                HSUtils.Log($"Steam install path found for Half Sword: \"{HSPath}\"");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find Steam or Half Sword Demo, exiting.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            // toolStripStatusLabel.Text = $"Half Sword demo found in {HSPath}";

            mods = new BindingList<HSInstallable>();
            HSUE4SS UE4SS = new HSUE4SS();
            mods.Add(UE4SS);
            UE4SS.LogMe();

            HSMod HSTM = new HSMod("HalfSwordTrainerMod", "https://github.com/massclown/HalfSwordTrainerMod", true, new List<HSInstallable>() { UE4SS });
            mods.Add(HSTM);
            HSTM.LogMe();

            HSMod HSSSM = new HSMod("HalfSwordSplitScreenMod", "https://github.com/massclown/HalfSwordSplitScreenMod", false, new List<HSInstallable>() { UE4SS });
            mods.Add(HSSSM);
            HSSSM.LogMe();

            bindingSource1.DataSource = mods;

            var downloadButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "downloadButton",
                HeaderText = "Download?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Download"
                },
                ToolTipText = "Download the latest version of this mod"
            };
            this.dataGridView1.Columns.Insert(2, downloadButtonColumn);

            var installButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "installButton",
                HeaderText = "Install?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Install/Uninstall"
                },
                ToolTipText = "Install or uninstall this mod"
            };
            this.dataGridView1.Columns.Insert(4, installButtonColumn);

            var enableButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "enableButton",
                HeaderText = "Enable?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Enable/Disable"
                },
                ToolTipText = "Enable or disable this mod"
            };
            this.dataGridView1.Columns.Insert(6, enableButtonColumn);

            //bindingSource1.ResetBindings(false);
            //dataGridView1.Refresh();
            StatusStipTextUpdate();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < dataGridView1.RowCount && e.RowIndex >= 0 &&
                e.ColumnIndex < dataGridView1.ColumnCount && e.ColumnIndex >= 0)
            {
                var currentCell = dataGridView1[e.ColumnIndex, e.RowIndex];
                if (currentCell is DataGridViewButtonCell buttonCell)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    var mod = (HSInstallable)row.DataBoundItem;
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
            }
            bindingSource1.ResetBindings(false);
            StatusStipTextUpdate();
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
                        row.Cells[j].Style.BackColor = ((bool)row.Cells[j].Value) ? Color.LightGreen : Color.PaleVioletRed;
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
            // TODO close the popup????
            // TODO refresh styles of datagridview ???
            bindingSource1?.ResetBindings(false);
            Cursor.Current = Cursors.Default;
        }

        private void buttonEasyInstall_Click(object sender, EventArgs e)
        {
            var oldText = buttonEasyInstall.Text;
            try
            {
                buttonEasyInstall.Text = "Working, please wait...";
                var HSTM = mods.SingleOrDefault(elem => elem.Name == "HalfSwordTrainerMod");
                if (HSTM.IsInstalled)
                {
                    if (MessageBox.Show($"Really update the mod?", "Confirm update",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                        MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.No)
                    {
                        buttonEasyInstall.Text = oldText;
                        return;
                    }

                }
                HSTM.Download();
                HSTM.InstallAll();
                HSTM.SetEnabled(true);
                buttonEasyInstall.Text = oldText;
                StatusStipTextUpdate();
                MessageBox.Show($"Half Sword Trainer Mod successfully installed!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                buttonEasyInstall.Text = oldText;
                MessageBox.Show($"Sorry, something went wrong.\n{exc.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonUninstallAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Really uninstall everything?", "Confirm uninstallation",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2) == System.Windows.Forms.DialogResult.Yes)
            {

                EasyUninstall();
                bindingSource1?.ResetBindings(false);
                dataGridView1.Refresh();
                StatusStipTextUpdate();
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
                    this.toolStripStatusLabel.Text += $"{mod.Name} version {mod.InstalledVersion} installed and {(mod.IsEnabled?"":"not ")}enabled. ";                 
                }
            }
            if (this.toolStripStatusLabel.Text.Length == 0)
            {
                this.toolStripStatusLabel.Text = "Half Sword demo found, no mods installed";
            }
        }

    }
}

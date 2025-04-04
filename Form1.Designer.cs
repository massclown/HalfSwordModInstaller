﻿namespace HalfSwordModInstaller
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonEasyInstall = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.buttonUE4SSLog = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.NameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VersionColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.InstalledVersion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsDownloadedColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsInstalledColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.IsEnabledColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ModWebURL = new System.Windows.Forms.DataGridViewLinkColumn();
            this.IsExperimentalColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.buttonCopyUE4SSLog = new System.Windows.Forms.Button();
            this.buttonCopyInstallerLog = new System.Windows.Forms.Button();
            this.buttonInstallerLog = new System.Windows.Forms.Button();
            this.buttonUninstallAll = new System.Windows.Forms.Button();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton1demo = new System.Windows.Forms.RadioButton();
            this.radioButton2playtest = new System.Windows.Forms.RadioButton();
            this.checkBoxUE4SSDevBuild = new System.Windows.Forms.CheckBox();
            this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.radioButton3demo04 = new System.Windows.Forms.RadioButton();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.tabPage1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 731);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1382, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 16);
            this.toolStripStatusLabel.Click += new System.EventHandler(this.toolStripStatusLabel_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1382, 731);
            this.tabControl1.TabIndex = 1;
            this.tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(this.tabControl1_Selected);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.tableLayoutPanel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 25);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1374, 702);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Simple";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Controls.Add(this.buttonEasyInstall, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.pictureBox1, 0, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(1368, 696);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // buttonEasyInstall
            // 
            this.buttonEasyInstall.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonEasyInstall.Font = new System.Drawing.Font("Comic Sans MS", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEasyInstall.Location = new System.Drawing.Point(459, 235);
            this.buttonEasyInstall.Name = "buttonEasyInstall";
            this.buttonEasyInstall.Size = new System.Drawing.Size(450, 226);
            this.buttonEasyInstall.TabIndex = 0;
            this.buttonEasyInstall.Text = "Get the latest trainer mod";
            this.toolTip1.SetToolTip(this.buttonEasyInstall, "Download, install/update and enable the latest Trainer Mod");
            this.buttonEasyInstall.UseVisualStyleBackColor = true;
            this.buttonEasyInstall.Click += new System.EventHandler(this.buttonEasyInstall_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox1.Image = global::HalfSwordModInstaller.Properties.Resources.stool;
            this.pictureBox1.Location = new System.Drawing.Point(3, 235);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(450, 226);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            this.toolTip1.SetToolTip(this.pictureBox1, "The icon is \"Stool\" by artworkbean from the Noun Project (CC BY 3.0)");
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tableLayoutPanel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1374, 702);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Advanced";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 6;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.6675F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66583F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.6675F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66583F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.6675F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66583F));
            this.tableLayoutPanel1.Controls.Add(this.buttonUE4SSLog, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.dataGridView1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.buttonCopyUE4SSLog, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonCopyInstallerLog, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonInstallerLog, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.buttonUninstallAll, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1368, 696);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // buttonUE4SSLog
            // 
            this.buttonUE4SSLog.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonUE4SSLog.AutoSize = true;
            this.buttonUE4SSLog.Location = new System.Drawing.Point(495, 651);
            this.buttonUE4SSLog.Name = "buttonUE4SSLog";
            this.buttonUE4SSLog.Size = new System.Drawing.Size(147, 42);
            this.buttonUE4SSLog.TabIndex = 2;
            this.buttonUE4SSLog.Text = "Open UE4SS log";
            this.toolTip1.SetToolTip(this.buttonUE4SSLog, "Open the log file of UE4SS");
            this.buttonUE4SSLog.UseVisualStyleBackColor = true;
            this.buttonUE4SSLog.Click += new System.EventHandler(this.buttonUE4SSLog_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.NameColumn,
            this.VersionColumn,
            this.InstalledVersion,
            this.IsDownloadedColumn,
            this.IsInstalledColumn,
            this.IsEnabledColumn,
            this.ModWebURL,
            this.IsExperimentalColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dataGridView1, 6);
            this.dataGridView1.DataSource = this.bindingSource1;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 109);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(1362, 536);
            this.dataGridView1.TabIndex = 0;
            this.toolTip1.SetToolTip(this.dataGridView1, "List of all mods and their status");
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick);
            this.dataGridView1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellValueChanged);
            this.dataGridView1.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView1_RowsAdded);
            // 
            // NameColumn
            // 
            this.NameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.NameColumn.DataPropertyName = "Name";
            this.NameColumn.HeaderText = "Mod Name";
            this.NameColumn.MinimumWidth = 6;
            this.NameColumn.Name = "NameColumn";
            this.NameColumn.ReadOnly = true;
            this.NameColumn.Width = 200;
            // 
            // VersionColumn
            // 
            this.VersionColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.VersionColumn.DataPropertyName = "LatestVersion";
            this.VersionColumn.HeaderText = "Latest";
            this.VersionColumn.MinimumWidth = 6;
            this.VersionColumn.Name = "VersionColumn";
            this.VersionColumn.ReadOnly = true;
            this.VersionColumn.Width = 70;
            // 
            // InstalledVersion
            // 
            this.InstalledVersion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.InstalledVersion.DataPropertyName = "InstalledVersion";
            this.InstalledVersion.HeaderText = "Installed";
            this.InstalledVersion.MinimumWidth = 6;
            this.InstalledVersion.Name = "InstalledVersion";
            this.InstalledVersion.ReadOnly = true;
            this.InstalledVersion.Width = 70;
            // 
            // IsDownloadedColumn
            // 
            this.IsDownloadedColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.IsDownloadedColumn.DataPropertyName = "IsDownloaded";
            this.IsDownloadedColumn.HeaderText = "Downloaded?";
            this.IsDownloadedColumn.MinimumWidth = 95;
            this.IsDownloadedColumn.Name = "IsDownloadedColumn";
            this.IsDownloadedColumn.ReadOnly = true;
            this.IsDownloadedColumn.Width = 95;
            // 
            // IsInstalledColumn
            // 
            this.IsInstalledColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.IsInstalledColumn.DataPropertyName = "IsInstalled";
            this.IsInstalledColumn.HeaderText = "Installed?";
            this.IsInstalledColumn.MinimumWidth = 75;
            this.IsInstalledColumn.Name = "IsInstalledColumn";
            this.IsInstalledColumn.ReadOnly = true;
            this.IsInstalledColumn.Width = 75;
            // 
            // IsEnabledColumn
            // 
            this.IsEnabledColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.IsEnabledColumn.DataPropertyName = "IsEnabled";
            this.IsEnabledColumn.HeaderText = "Enabled?";
            this.IsEnabledColumn.MinimumWidth = 75;
            this.IsEnabledColumn.Name = "IsEnabledColumn";
            this.IsEnabledColumn.ReadOnly = true;
            this.IsEnabledColumn.Width = 75;
            // 
            // ModWebURL
            // 
            this.ModWebURL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.ModWebURL.DataPropertyName = "Url";
            this.ModWebURL.HeaderText = "URL";
            this.ModWebURL.MinimumWidth = 6;
            this.ModWebURL.Name = "ModWebURL";
            this.ModWebURL.ReadOnly = true;
            this.ModWebURL.TrackVisitedState = false;
            this.ModWebURL.Width = 43;
            // 
            // IsExperimentalColumn
            // 
            this.IsExperimentalColumn.DataPropertyName = "IsExperimental";
            this.IsExperimentalColumn.FalseValue = "false";
            this.IsExperimentalColumn.HeaderText = "Experimental?";
            this.IsExperimentalColumn.IndeterminateValue = "false";
            this.IsExperimentalColumn.MinimumWidth = 6;
            this.IsExperimentalColumn.Name = "IsExperimentalColumn";
            this.IsExperimentalColumn.TrueValue = "true";
            this.IsExperimentalColumn.Width = 125;
            // 
            // buttonCopyUE4SSLog
            // 
            this.buttonCopyUE4SSLog.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonCopyUE4SSLog.AutoSize = true;
            this.buttonCopyUE4SSLog.Location = new System.Drawing.Point(723, 651);
            this.buttonCopyUE4SSLog.Name = "buttonCopyUE4SSLog";
            this.buttonCopyUE4SSLog.Size = new System.Drawing.Size(147, 42);
            this.buttonCopyUE4SSLog.TabIndex = 5;
            this.buttonCopyUE4SSLog.Text = "Copy UE4SS log\r\nfilename to clipboard";
            this.toolTip1.SetToolTip(this.buttonCopyUE4SSLog, "Copy the filename of the UE4SS log file to clipboard, so you can find it later");
            this.buttonCopyUE4SSLog.UseVisualStyleBackColor = true;
            this.buttonCopyUE4SSLog.Click += new System.EventHandler(this.buttonCopyUE4SSLog_Click);
            // 
            // buttonCopyInstallerLog
            // 
            this.buttonCopyInstallerLog.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonCopyInstallerLog.AutoSize = true;
            this.buttonCopyInstallerLog.Location = new System.Drawing.Point(268, 651);
            this.buttonCopyInstallerLog.Name = "buttonCopyInstallerLog";
            this.buttonCopyInstallerLog.Size = new System.Drawing.Size(147, 42);
            this.buttonCopyInstallerLog.TabIndex = 4;
            this.buttonCopyInstallerLog.Text = "Copy installer log\r\nfilename to clipboard";
            this.toolTip1.SetToolTip(this.buttonCopyInstallerLog, "Copy the filename of the installer log file to clipboard, so you can find it late" +
        "r");
            this.buttonCopyInstallerLog.UseVisualStyleBackColor = true;
            this.buttonCopyInstallerLog.Click += new System.EventHandler(this.buttonCopyInstallerLog_Click);
            // 
            // buttonInstallerLog
            // 
            this.buttonInstallerLog.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonInstallerLog.AutoSize = true;
            this.buttonInstallerLog.Location = new System.Drawing.Point(40, 651);
            this.buttonInstallerLog.Name = "buttonInstallerLog";
            this.buttonInstallerLog.Size = new System.Drawing.Size(147, 42);
            this.buttonInstallerLog.TabIndex = 1;
            this.buttonInstallerLog.Text = "Open installer log";
            this.toolTip1.SetToolTip(this.buttonInstallerLog, "Open the log file of this installer");
            this.buttonInstallerLog.UseVisualStyleBackColor = true;
            this.buttonInstallerLog.Click += new System.EventHandler(this.buttonInstallerLog_Click);
            // 
            // buttonUninstallAll
            // 
            this.buttonUninstallAll.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.buttonUninstallAll.AutoSize = true;
            this.buttonUninstallAll.BackColor = System.Drawing.Color.Transparent;
            this.buttonUninstallAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUninstallAll.Location = new System.Drawing.Point(1179, 651);
            this.buttonUninstallAll.Name = "buttonUninstallAll";
            this.buttonUninstallAll.Size = new System.Drawing.Size(147, 42);
            this.buttonUninstallAll.TabIndex = 3;
            this.buttonUninstallAll.Text = "Uninstall all mods";
            this.toolTip1.SetToolTip(this.buttonUninstallAll, "Uninstall all mods, including UE4SS");
            this.buttonUninstallAll.UseVisualStyleBackColor = false;
            this.buttonUninstallAll.Click += new System.EventHandler(this.buttonUninstallAll_Click);
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 6);
            this.flowLayoutPanel1.Controls.Add(this.groupBox1);
            this.flowLayoutPanel1.Controls.Add(this.checkBoxUE4SSDevBuild);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1362, 100);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // groupBox1
            // 
            this.groupBox1.AutoSize = true;
            this.groupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBox1.Controls.Add(this.radioButton3demo04);
            this.groupBox1.Controls.Add(this.radioButton1demo);
            this.groupBox1.Controls.Add(this.radioButton2playtest);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(349, 114);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Target game to mod:";
            // 
            // radioButton1demo
            // 
            this.radioButton1demo.AutoSize = true;
            this.radioButton1demo.Location = new System.Drawing.Point(6, 21);
            this.radioButton1demo.Name = "radioButton1demo";
            this.radioButton1demo.Size = new System.Drawing.Size(337, 20);
            this.radioButton1demo.TabIndex = 6;
            this.radioButton1demo.TabStop = true;
            this.radioButton1demo.Text = "Half Sword Demo (\"dark, endless abyss\", Nov 2023)";
            this.radioButton1demo.UseVisualStyleBackColor = true;
            this.radioButton1demo.CheckedChanged += new System.EventHandler(this.radioButton1_CheckedChanged);
            // 
            // radioButton2playtest
            // 
            this.radioButton2playtest.AutoSize = true;
            this.radioButton2playtest.Location = new System.Drawing.Point(6, 47);
            this.radioButton2playtest.Name = "radioButton2playtest";
            this.radioButton2playtest.Size = new System.Drawing.Size(294, 20);
            this.radioButton2playtest.TabIndex = 7;
            this.radioButton2playtest.TabStop = true;
            this.radioButton2playtest.Text = "Half Sword Playtest (\"castle courtyard\", 2024)";
            this.radioButton2playtest.UseVisualStyleBackColor = true;
            this.radioButton2playtest.CheckedChanged += new System.EventHandler(this.radioButton2_CheckedChanged);
            // 
            // checkBoxUE4SSDevBuild
            // 
            this.checkBoxUE4SSDevBuild.AutoSize = true;
            this.checkBoxUE4SSDevBuild.Enabled = false;
            this.checkBoxUE4SSDevBuild.Location = new System.Drawing.Point(358, 3);
            this.checkBoxUE4SSDevBuild.Name = "checkBoxUE4SSDevBuild";
            this.checkBoxUE4SSDevBuild.Size = new System.Drawing.Size(141, 20);
            this.checkBoxUE4SSDevBuild.TabIndex = 9;
            this.checkBoxUE4SSDevBuild.Text = "UE4SS Dev Build?";
            this.toolTip1.SetToolTip(this.checkBoxUE4SSDevBuild, "Download a development build of UE4SS");
            this.checkBoxUE4SSDevBuild.UseVisualStyleBackColor = true;
            this.checkBoxUE4SSDevBuild.CheckedChanged += new System.EventHandler(this.checkBoxUE4SSDevBuild_CheckedChanged);
            // 
            // backgroundWorker1
            // 
            this.backgroundWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker1_DoWork);
            this.backgroundWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker1_RunWorkerCompleted);
            // 
            // radioButton3demo04
            // 
            this.radioButton3demo04.AutoSize = true;
            this.radioButton3demo04.Location = new System.Drawing.Point(6, 73);
            this.radioButton3demo04.Name = "radioButton3demo04";
            this.radioButton3demo04.Size = new System.Drawing.Size(199, 20);
            this.radioButton3demo04.TabIndex = 8;
            this.radioButton3demo04.TabStop = true;
            this.radioButton3demo04.Text = "Half Sword Demo v0.4 (2025)";
            this.radioButton3demo04.UseVisualStyleBackColor = true;
            this.radioButton3demo04.CheckedChanged += new System.EventHandler(this.radioButton3demo04_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1382, 753);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(1024, 600);
            this.Name = "Form1";
            this.Text = "Half Sword Mod Installer";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.tabPage1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.Windows.Forms.Button buttonEasyInstall;
        private System.Windows.Forms.Button buttonInstallerLog;
        private System.Windows.Forms.Button buttonUninstallAll;
        private System.Windows.Forms.Button buttonUE4SSLog;
        private System.Windows.Forms.Button buttonCopyUE4SSLog;
        private System.Windows.Forms.Button buttonCopyInstallerLog;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.DataGridViewTextBoxColumn NameColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn VersionColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn InstalledVersion;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsDownloadedColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsInstalledColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn IsEnabledColumn;
        private System.Windows.Forms.DataGridViewLinkColumn ModWebURL;
        private System.Windows.Forms.DataGridViewCheckBoxColumn IsExperimentalColumn;
        private System.Windows.Forms.RadioButton radioButton1demo;
        private System.Windows.Forms.RadioButton radioButton2playtest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.CheckBox checkBoxUE4SSDevBuild;
        private System.Windows.Forms.RadioButton radioButton3demo04;
    }
}


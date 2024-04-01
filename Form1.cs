using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HalfSwordModInstaller
{
    public partial class Form1 : Form
    {
        public List<HSMod> mods;

        public Form1()
        {
            InitializeComponent();
        }

        private void EasyInstall()
        {
            var UE4SS = mods.Find(elem => elem.Name == "UE4SS");
            UE4SS.Download();
            UE4SS.Install();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string HSPath = HSUtils.HSBinaryPath;
            HSUtils.Log($"Steam install path found for HalfSword: \"{HSPath}\"");

            mods = new List<HSMod>();
            HSUE4SS UE4SS = new HSUE4SS();
            mods.Add(UE4SS);
            UE4SS.LogMe();

            HSMod HSTM = new HSMod("HalfSwordTrainerMod", "https://github.com/massclown/HalfSwordTrainerMod", true);
            mods.Add(HSTM);
            HSTM.LogMe();

            HSMod HSSSM = new HSMod("HalfSwordSplitScreenMod", "https://github.com/massclown/HalfSwordSplitScreenMod", false);
            mods.Add(HSSSM);
            HSSSM.LogMe();

            var downloadButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "downloadButton",
                HeaderText = "Download?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Download"
                }
            };
            this.dataGridView1.Columns.Insert(2, downloadButtonColumn);

            var installButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "installButton",
                HeaderText = "Install?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Install"
                }
            };
            this.dataGridView1.Columns.Insert(4, installButtonColumn);

            var enableButtonColumn = new DataGridViewButtonColumn()
            {
                Name = "enableButton",
                HeaderText = "Enable?",
                UseColumnTextForButtonValue = false,
                DefaultCellStyle = new DataGridViewCellStyle()
                {
                    NullValue = "Enable"
                }
            };
            this.dataGridView1.Columns.Insert(6, enableButtonColumn);

            foreach (var mod in mods)
            {
                this.dataGridView1.Rows.Add(mod.Name, mod.Version);
            }

        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < dataGridView1.RowCount && e.RowIndex >= 0 &&
                e.ColumnIndex < dataGridView1.ColumnCount && e.ColumnIndex >= 0)
            {
                if (dataGridView1[e.ColumnIndex, e.RowIndex] is DataGridViewButtonCell cell)
                {
                    switch (dataGridView1.Columns[e.ColumnIndex].Name)
                    {
                        case "downloadButton":
                            break;
                        case "installButton":
                            break;
                        case "enableButton":
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

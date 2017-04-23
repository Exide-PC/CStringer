using CStringer.Utils;
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

namespace CStringer
{
    public partial class BackupForm : Form
    {
        BackupMaster backupMaster = null;
        iniSettings settings = null;

        public bool BackupRestored { get; private set; } = false;

        public BackupForm(BackupMaster backupMaster, iniSettings settings)
        {
            InitializeComponent();

            this.backupMaster = backupMaster;
            this.settings = settings;
            checkBox1.Checked = settings.BackupsOnSave;
            numericUpDown1.Value = settings.MaxBackupCount;
                    
            FileInfo backupInfo = new FileInfo(backupMaster.FullPath);
            label2.Text = $"Backups for {backupInfo.Name}";
            
            UpdateList();              
        }

        private void UpdateList()
        {
            string[] backups = backupMaster.Backups;
            listBox1.Items.Clear();

            for (int i = 0; i < backups.Length; i++)
            {
                string writeTime = File.GetCreationTime(backups[i]).ToString("d/M/yyyy  H:mm");
                string item = $"{i + 1}. {writeTime}";

                listBox1.Items.Add(item);
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            settings.BackupsOnSave = checkBox1.Checked;
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            string[] backups = backupMaster.Backups;
            int selectedIndex = listBox1.SelectedIndex;

            if (selectedIndex == -1)
                return;

            string backupPath = backups[selectedIndex];
            backupMaster.DeleteBackup(backupPath);

            UpdateList();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            backupMaster.CreateBackup();
            UpdateList();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string[] backups = backupMaster.Backups;
            int selectedIndex = listBox1.SelectedIndex;

            if (selectedIndex == -1)
                return;

            string backupPath = backups[selectedIndex];
            backupMaster.RestoreBackup(backupPath);

            this.BackupRestored = true;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            settings.MaxBackupCount = (int) numericUpDown1.Value;
            backupMaster.SetMaxCount(settings.MaxBackupCount);

            UpdateList();
        }

        private void BackupForm_Load(object sender, EventArgs e)
        {

        }
    }
}

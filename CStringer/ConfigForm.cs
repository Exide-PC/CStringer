using CStringer.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CStringer
{
    public partial class ConfigForm : Form
    {
        //Locale locale;
        LocaleConfig currentConfig;
        //iniSettings settings;

        public bool IsExecutionRequested { get; private set; }
        public string LocalePath { get; set; }
        public LocaleConfig Config { get; private set; }

        public ConfigForm(LocaleConfig localeConfig)
        {
            InitializeComponent();

            //this.locale = locale;
            this.currentConfig = localeConfig;
            this.Config = null;
            //this.settings = settings;
            this.IsExecutionRequested = false;

            this.Update();
        }
        
        private new void Update()
        {
            label2.Text = currentConfig.CustomEntries.Count.ToString();
            listBox1.Items.Clear();

            foreach (string value in currentConfig.CustomEntries.Values)
                listBox1.Items.Add(value);
        }

        private void ConfigForm_Load(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            // Подгрузить конфиг, но только в форму
            openFileDialog1.Filter = "CStringer Configs (*.CSC)|*.csc";
            openFileDialog1.FileName = "";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog1.FileName;
                LocaleConfig openedConfig = LocaleConfig.Read(filePath);

                currentConfig.MergeWith(openedConfig);
                Update();
            }            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // Закинуть конфиг в основной экземпляр локализации

            // И изменить набор цветов основной формы
            //settings.ColorSet = this.currentConfig.CustomColors;

            Dictionary<string, string> configEntries = 
                new Dictionary<string, string>(currentConfig.CustomEntries);
            int[] configColors = currentConfig.CustomColors;
            
            this.Config = new LocaleConfig(configEntries, configColors);
            this.IsExecutionRequested = true;
        }       

        private void button3_Click(object sender, EventArgs e)
        {
            // Сохранить конфиг, используя данные из формы
            string name = "config";

            if (this.LocalePath != null) // If exact path is determined
            {
                FileInfo localeInfo = new FileInfo(this.LocalePath);

                name = localeInfo.Name
                    .Replace("csgo_", "")
                    .Replace(".txt", "");
            }           

            saveFileDialog1.FileName = $"CStringer_{name}";
            saveFileDialog1.AddExtension = true;
            saveFileDialog1.DefaultExt = "csc";

            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog1.FileName;
                this.currentConfig.Save(filePath);
            }
        }       
    }
}

using CStringer.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace CStringer
{
    /*
     * Изменение сразу всех строк с одинаковым значением
     */

    public partial class Form1 : Form
    {
        const int NUMBER_COLUMN = 0;
        const int NAME_COLUMN = 1;
        const int STRING_COLUMN = 2;
                
        string[] localesPaths;

        Locale currentLocale = null;
        BackupMaster backupMaster = null;
        iniSettings iniSet = null;
        List<QuickCategory> categories = null;

        public Form1()
        {
            InitializeComponent();
            InitializeExtra();
        }

        private void InitializeExtra()
        {
            // Getting "My documents" folder for futher file creating
            string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string exideProdDir = docs + @"\ExideProduction";
            
            // Create if it doesn't exist
            if (!Directory.Exists(exideProdDir))
                Directory.CreateDirectory(exideProdDir);

            // Specifying cfg and log initialization
            string cfgName = "CStringer.xml";
            string cfgPath = $@"{exideProdDir}\{cfgName}";
            Log.Path = exideProdDir + @"\CStringer.txt";

            #region Tests

            InitCategories();

            foreach (QuickCategory category in categories)
                comboBox2.Items.Add(category.Name);

            #endregion

            // If cfg doesnt exist (or 1st program launch)
            if (!File.Exists(cfgPath))
            {
                DialogResult result;
                string folderName;

                do
                {
                    // Using folder dialog to ask user csgo path
                    folderBrowserDialog1.Description = "Choose \"Counter-Strike Global Offensive\" folder";
                    result = folderBrowserDialog1.ShowDialog();
                    folderName = "";
                    
                    // If user didnt choose folder we just close form and stop
                    if (result != DialogResult.OK)
                    {
                        this.Close();
                        return;
                    }
                    
                    folderName = folderBrowserDialog1.SelectedPath.Split('\\').Last();
                }
                // Verifying if the folder is correct, otherwise try again
                while (folderName != @"Counter-Strike Global Offensive");

                // Creating ini object which we save later
                //  and it's default value initialization
                iniSet = new iniSettings()
                {
                    CsgoRoot = folderBrowserDialog1.SelectedPath,
                    BackupsOnSave = true,
                    MaxBackupCount = 5,
                    ExideProdDir = exideProdDir,
                    ColorSet = null
                };
                iniSet.SetPath(cfgPath);
            }
            else
            {   // Otherwise read cfg and init fields
                iniSet = iniSettings.Read(cfgPath);

                // TODO: If cfg version is older than actual one
            }

            // Specifying resource path
            string resourcePath = iniSet.CsgoRoot + @"\csgo\resource";

            // Now we can get locale list from resource folder..
            localesPaths = Locale.LocalesAt(resourcePath, getFullPath: true);
            // .. and fill our combobox
            foreach (string locale in localesPaths)
                comboBox1.Items.Add(locale.Split('\\').Last());
        }
        
        private void InitCategories()
        {
            categories = new List<QuickCategory>();

            QuickCategory all = new QuickCategory("All", true);
            categories.Add(all);

            QuickCategory ranks = new QuickCategory("Ranks");
            for (int i = 1; i <= 18; i++)
                ranks.Keys.Add($"SFUI_ELO_RankName_{i}");
            categories.Add(ranks);
        }

        private void UpdateTable(Dictionary<string, string> data)
        {
            table.Rows.Clear();
            int rowCount = 0;

            foreach (KeyValuePair<string, string> pair in data)
                table.Rows.Add(++rowCount, pair.Key, pair.Value);
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void table_SelectionChanged(object sender, EventArgs e)
        {
            richTextBox1.Text = table.CurrentRow.Cells[STRING_COLUMN].Value.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string key = table.CurrentRow.Cells[NAME_COLUMN].Value.ToString();
            string value = richTextBox1.Text;

            if (value.Contains("\n"))
            {
                value = Regex.Replace(value, "\n", "");
                richTextBox1.Text = value;
            }

            currentLocale.EditEntry(key, value);
            table.CurrentRow.Cells[STRING_COLUMN].Value = value;            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            currentLocale.UpdateFile();

            if (iniSet.BackupsOnSave)
                backupMaster.CreateBackup();

            button2.Text = "File saved";
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
            string searchedValue = textBox1.Text.ToLower();

            // TODO: Nothing to do if items are not already filtered
            if (searchedValue != string.Empty)
            {
                // Commit search otherwise
                Dictionary<string, string> searchResult = currentLocale.SelectedEntries // currentLocale.Entries
                    .Where(entry => entry.Key.ToLower().Contains(searchedValue)
                                 || entry.Value.ToLower().Contains(searchedValue))
                    .ToDictionary(entry => entry.Key, entry => entry.Value);

                UpdateTable(searchResult);
            }
            else
                UpdateTable(currentLocale.SelectedEntries); //currentLocale.Entries);            

            // If the table is empty, "Apply" button is gonna throw exceptions
            button1.Enabled = table.Rows.Count > 0;
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                button3.PerformClick();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            //iniSettings iniSet = new iniSettings();            
            iniSet.Save();
        }

        private void debug_Click(object sender, EventArgs e)
        {
            BackupForm bf = new BackupForm(backupMaster, iniSet);
            bf.Visible = true;
        }

        private void button2_MouseLeave(object sender, EventArgs e)
        {
            button2.Text = "Save file";
        }

        private void richTextBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
                button1.PerformClick();
        }

        private void InsertTag(HtmlPaster.Tag tag)
        {
            string line = richTextBox1.Text;
            int start = richTextBox1.SelectionStart;
            int length = richTextBox1.SelectionLength;

            if (start == length)
                return;
            
            string newLine = HtmlPaster.InsertTag(line, start, length, tag);
            richTextBox1.Text = newLine;
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
        {

        }

        private void bindingNavigatorPositionItem_Click(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            InsertTag(HtmlPaster.Tag.Italic);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            InsertTag(HtmlPaster.Tag.Bold);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            InsertTag(HtmlPaster.Tag.Strikethrough);
        }

        private void button9_Click(object sender, EventArgs e)
        {
            InsertTag(HtmlPaster.Tag.Underline);
        }

        private void SetFont(HtmlPaster.FontAttribute attribute, object obj)
        {
            string line = richTextBox1.Text;
            int start = richTextBox1.SelectionStart;
            int length = richTextBox1.SelectionLength;

            if (start == length)
                return;

            string newLine = HtmlPaster.SetFont(line, start, length, attribute, obj);
            richTextBox1.Text = newLine;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (richTextBox1.SelectionLength == 0)
                return;

            // Getting actual color set
            colorDialog1.CustomColors = iniSet.ColorSet;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color color = colorDialog1.Color;
                SetFont(HtmlPaster.FontAttribute.Color, color);
            }
            
            // Saving custom colors in any way
            iniSet.ColorSet = colorDialog1.CustomColors;            
        }

        private void button11_Click(object sender, EventArgs e)
        {
            
            if (richTextBox1.SelectionLength == 0)
                return;

            int defaultSize = 5;

            SetFont(HtmlPaster.FontAttribute.Size, defaultSize);

            int foundIndex = richTextBox1.Text.LastIndexOf($"'{defaultSize}'"); // searching for '5'

            richTextBox1.Focus();
            richTextBox1.SelectionStart = foundIndex + 1;
            richTextBox1.SelectionLength = 1;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {     
            string localePath = localesPaths[comboBox1.SelectedIndex];

            currentLocale = new Locale(localePath);
            backupMaster = new BackupMaster(currentLocale.Path, iniSet.MaxBackupCount);
                                    
            if (comboBox2.SelectedIndex == 0)
                // Table update if combobox property won't 
                UpdateTable(currentLocale.SelectedEntries);
            else
                // or allowing combobox to update it itself
                comboBox2.SelectedIndex = 0;
            

            textBox1.Enabled = true;
            richTextBox1.Enabled = true;
            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button12.Enabled = true;
            button13.Enabled = true;
            comboBox2.Enabled = true;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {
            BackupForm bf = new BackupForm(backupMaster, iniSet);
            bf.ShowDialog();

            // If locale file is restored from backup
            if (bf.BackupRestored)
            {
                // Then we construct new locale object
                // from the same path
                string localePath = currentLocale.Path;
                QuickCategory category = currentLocale.SelectedCategory;

                currentLocale = new Locale(localePath, category);

                UpdateTable(currentLocale.SelectedEntries); // currentLocale.Entries);
            }                
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // Config required by configform constructor
            LocaleConfig localeConfig = 
                new LocaleConfig(this.currentLocale.EditedEntries, iniSet.ColorSet);

            ConfigForm cf = new ConfigForm(localeConfig);
            cf.LocalePath = this.currentLocale.Path;
            cf.ShowDialog();
            
            // Update form if config has been executed
            if (cf.IsExecutionRequested) // TODO: DEBUG
            {
                // Update locale with specified entries
                currentLocale.ExecuteConfig(cf.Config);
                // And update colorset for main form
                iniSet.ColorSet = cf.Config.CustomColors;
                
                UpdateTable(currentLocale.SelectedEntries);
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            string categoryName = comboBox2.SelectedItem.ToString();
            QuickCategory chosenCategory = categories.Find(n => n.Name == categoryName);
            
            // Setting table entries filter
            currentLocale.SelectedCategory = chosenCategory;            

            //if (doUpdateTable) // If comboBox is allowed to update table then we do
            UpdateTable(currentLocale.SelectedEntries);
        }

        private void button4_Click_1(object sender, EventArgs e)
        {

        }

        private void table_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void searchBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}

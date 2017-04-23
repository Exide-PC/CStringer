using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CStringer.Utils
{
    public class Locale
    {
        private Regex entryPattern = new Regex("^\\s*\"(?!\\[english\\])\\S+\"\\s*\".*\"[^\"]*$", RegexOptions.Compiled);
        private Regex entryKeyPattern = new Regex("^\\s*\"(?!\\[english\\])\\S+\"", RegexOptions.Compiled);
        private Regex sideQuotesPattern = new Regex("(^[^\"]*\"|\"[^\"]*$)", RegexOptions.Compiled);

        private string[] fileLines = null;        
        private Dictionary<string, string> defaultEntries;        
        private static Regex localePattern = new Regex(@"csgo_[a-z]+\.txt", RegexOptions.Compiled);

        public Dictionary<string, string> AllEntries { get; } = new Dictionary<string, string>();
        public Dictionary<string, string> EditedEntries { get; } = null;
        public QuickCategory SelectedCategory { get; set; } = null;
        public string Path { get; private set; } = null;

        public Dictionary<string, string> SelectedEntries
        {
            get
            {
                // If default category then we return all entries
                if (SelectedCategory.IsDefault)
                    return new Dictionary<string, string>(AllEntries);

                // Otherwise we fill new dictionary with all actual entries
                Dictionary<string, string> selected = new Dictionary<string, string>();

                foreach (string key in SelectedCategory)
                {
                    string value;
                    bool isFound = this.AllEntries.TryGetValue(key, out value);

                    if (isFound)
                        selected[key] = value;
                }

                return selected;
            }
        }

        public static string[] LocalesAt(string path, bool getFullPath = false)
        {
            string[] locales = Directory.GetFiles(path)
                .Where(file => localePattern.IsMatch(file.Split('\\').Last()))
                .Select(file => getFullPath? file: file.Split('\\').Last())
                .ToArray();

            return locales;
        }

        #region Constructors
        public Locale(string localizationPath)
        {
            this.Path = localizationPath;
            this.SelectedCategory = new QuickCategory(); // Default category

            fileLines = File.ReadAllLines(Path);
            string[] entriesArray = fileLines.Where(n => entryPattern.IsMatch(n)).ToArray();
            
            foreach (string entry in entriesArray)
            {
                string dirtyKey = entryKeyPattern.Match(entry).ToString().Trim();
                string key = sideQuotesPattern.Replace(dirtyKey, "");

                string dirtyValue = entryKeyPattern.Replace(entry, "").Trim();
                string value = sideQuotesPattern.Replace(dirtyValue, "");
                
                AllEntries[key] = value;
            }

            defaultEntries = new Dictionary<string, string>(AllEntries);
            EditedEntries = new Dictionary<string, string>();
        }

        public Locale(string localizationPath, QuickCategory category): this(localizationPath)
        {
            this.SelectedCategory = category;
        }
        #endregion

        public void EditEntry(string key, string value)
        {
            EditedEntries[key] = value;
            AllEntries[key] = value;
        }

        public void ExecuteConfig(LocaleConfig localeCfg)
        {
            foreach (KeyValuePair<string, string> pair in localeCfg.CustomEntries)
            {
                // If current key exists then we edit it
                if (this.AllEntries.ContainsKey(pair.Key))
                    this.EditEntry(pair.Key, pair.Value);
            }                
        }
        
        public void UpdateFile()
        {
            for (int i = 0; i < fileLines.Length; i++)
            {
                string line = fileLines[i];

                foreach (KeyValuePair<string, string> pair in EditedEntries)
                {
                    string defaultValue = defaultEntries[pair.Key]; // TODO: fix "not found" exception

                    if (line.Contains(pair.Key) && line.Contains(defaultValue))
                        fileLines[i] = line.Replace("\"" + defaultValue + "\"", "\"" + pair.Value + "\"");
                }
            }

            File.WriteAllLines(Path, fileLines, Encoding.Unicode);
        }
    }
}

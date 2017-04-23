using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace CStringer.Utils
{
    public class LocaleConfig
    {
        public Dictionary<string, string> CustomEntries { get; private set; }
        public int[] CustomColors { get; set; }
        private string path;
                
        public LocaleConfig(Dictionary<string, string> editedEntries, int[] colors)
        {
            this.CustomEntries = new Dictionary<string, string>(editedEntries);
            this.CustomColors = colors;
        }
        
        public static LocaleConfig Read(string path)
        {          
            // Getting config holder from file
            LocaleConfigHolder cfgHolder = EasySerializer.Read<LocaleConfigHolder>(path);

            // Getting entries map
            Dictionary<string, string> customEntries = new Dictionary<string, string>();

            for (int i = 0; i < cfgHolder.Keys.Length; i++)
            {
                string key = cfgHolder.Keys[i];
                string value = cfgHolder.Values[i];

                customEntries[key] = value;
            }

            // Getting color
            int[] colors = cfgHolder.CustomColors;

            // Packing data into config class
            LocaleConfig localeConfig = new LocaleConfig(customEntries, colors);
            localeConfig.path = path;

            return localeConfig;
        }

        public void MergeWith(LocaleConfig anotherCfg)
        {
            // Getting all entries
            foreach (KeyValuePair<string, string> pair in anotherCfg.CustomEntries)
            {
                string key = pair.Key;
                string value = pair.Value;

                this.CustomEntries[key] = value;
            }

            // Setting the same color
            this.CustomColors = anotherCfg.CustomColors;
        }

        public void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
                path = this.path;

            LocaleConfigHolder cfgHolder = new LocaleConfigHolder();

            // Saving map
            cfgHolder.Keys = CustomEntries.Keys.ToArray();
            cfgHolder.Values = CustomEntries.Values.ToArray();

            // Saving color
            cfgHolder.CustomColors = this.CustomColors;

            EasySerializer.Write<LocaleConfigHolder>(cfgHolder, path);
        }

        public class LocaleConfigHolder
        {
            public int[] CustomColors { get; set; }
            public string[] Keys { get; set; }
            public string[] Values { get; set; }
        }
    }
}

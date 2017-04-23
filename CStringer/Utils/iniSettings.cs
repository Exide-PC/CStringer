using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CStringer.Utils
{
    public class iniSettings
    {
        public string CsgoRoot { get; set; }
        public string ExideProdDir { get; set; }
        public bool BackupsOnSave { get; set; }
        public int MaxBackupCount { get; set; }
        public int[] ColorSet { get; set; }
        
        private string cfgPath;

        public iniSettings() { }

        public void SetPath(string path)
        {
            this.cfgPath = path;
        }

        public static iniSettings Read(string path)
        {
            iniSettings iniSet = EasySerializer.Read<iniSettings>(path);
            iniSet.cfgPath = path;

            return iniSet;
        }

        public void Save(string path = null)
        {
            if (string.IsNullOrEmpty(path))
                path = this.cfgPath;

            EasySerializer.Write<iniSettings>(this, path);
        }
    }
}

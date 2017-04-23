using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Utils
{
    static class Log
    {
        public static string Path { get; set; } = null;
        
        public static void Write(object obj)
        {
            if (Path == null)
                throw new FileNotFoundException("Некорректный путь к файлу");

            File.AppendAllText(Path, $"\n{obj.ToString()}");
        }
    }
}

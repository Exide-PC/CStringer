using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Utils
{
    public class QuickCategory: IEnumerable<string>
    {
        public string Name { get; }
        public bool IsDefault { get; }
        public List<string> Keys { get; }

        public QuickCategory(string name, bool isDefault = false)
        {
            this.Name = name;
            this.IsDefault = isDefault;
            this.Keys = new List<string>();
        }

        public QuickCategory()
        {
            this.Name = "Default";
            this.IsDefault = true;
            this.Keys = new List<string>();
        }

        public IEnumerator<string> GetEnumerator()
        {
            return this.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }
    }
}

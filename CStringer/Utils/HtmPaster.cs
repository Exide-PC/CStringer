using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CStringer.Utils
{
    static class HtmlPaster
    {
        public enum Tag
        {
            Bold, Italic, Underline, Strikethrough
        }

        public enum FontAttribute
        {
            Size, Color
        }

        static string[] SplitByIndexes(string line, int start, int length)
        {
            string[] parts = new string[3];

            parts[0] = line.Substring(0, start);
            parts[1] = line.Substring(start, length);
            parts[2] = line.Substring(start + length, line.Length - start - length);

            return parts;
        }

        public static string InsertTag(string line, int start, int length, Tag tag)
        {
            string[] parts = SplitByIndexes(line, start, length);

            string tagStr;

            if (tag == Tag.Bold) tagStr = "b";
            else if (tag == Tag.Italic) tagStr = "i";
            else if (tag == Tag.Strikethrough) tagStr = "s";
            else if (tag == Tag.Underline) tagStr = "u";
            else throw new ArgumentException("Wrong tag");

            return $"{parts[0]}<{tagStr}>{parts[1]}</{tagStr}>{parts[2]}";
        }

        public static string SetFont(string line, int start, int length, FontAttribute attribute, object obj)
        {
            string[] parts = SplitByIndexes(line, start, length);

            // abcde <font size='5'>TARGET</font>
            // abcde TARGET
            // <font size='18' color='#CD0000'>%s1</font>

            string tagStr;
            string value;

            if (attribute == FontAttribute.Size)
            {
                tagStr = "size";
                value = ((int)obj).ToString();
            }
            else if (attribute == FontAttribute.Color)
            {
                tagStr = "color";

                System.Drawing.Color color = (System.Drawing.Color)obj;
                int colorArgb = color.ToArgb();

                value = $"#{colorArgb.ToString("X")}";
            }
            else throw new ArgumentException("Wrong tag");


            if (parts[0].EndsWith("'>"))
            {
                parts[0] = parts[0].Remove(parts[0].Length - 1);
                return $"{parts[0]} {tagStr}='{value}'>{parts[1]}{parts[2]}";
            }
            else
            {
                return $"{parts[0]}<font {tagStr}='{value}'>{parts[1]}</font>{parts[2]}";
            }
        }
    }
}

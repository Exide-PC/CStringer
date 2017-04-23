using System;
using System.IO;
using System.Xml.Serialization;

namespace CStringer.Utils
{
    public static class EasySerializer
    {
        public static T Read<T>(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();

            T obj;

            using (Stream stream = new FileStream(path, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                obj = (T)serializer.Deserialize(stream);
            }

            return obj;
        }

        public static void Write<T>(T obj, string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException();

            using (Stream writer = new FileStream(path, FileMode.Create))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, obj);
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Prediction.Core.Extensions
{
    public static class XmlExtensions
    {
        public static string Serialize<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static T Deserialize<T>(this string value)
        {
            if (value == null)
            {
                return default(T);
            }
            try
            {
                XmlSerializer ser = new XmlSerializer(typeof(T));
                T obj;
                var stringReader = new StringReader(value);
                using (XmlReader reader = XmlReader.Create(stringReader))
                {
                    obj = (T)ser.Deserialize(reader);
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        public static void Serialize<T>(this T value, string path)
        {
            File.WriteAllText(path, Serialize(value), Encoding.UTF8);
        }

        public static T DeserializeFromFile<T>(string path)
        {
            return File.ReadAllText(path, Encoding.UTF8).Deserialize<T>();
        }
    }
}

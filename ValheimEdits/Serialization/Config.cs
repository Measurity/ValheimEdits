using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ValheimEdits.Serialization
{
    public class Config : IXmlSerializable
    {
        private static readonly XmlSerializer serializer = new(typeof(Config));
        private static Config instance;
        public static Config Instance => instance ??= new Config();

        [Description("Default 61440 (60KB) per second")]
        public int DataRateLimit { get; set; } = 61440;

        public bool WorkbenchRequiresRoof { get; set; } = true;

        [Description("If true, allows sleeping even if it is really inconvenient to character")]
        public bool HoboSleeping { get; set; } = false;

        public bool SkillLossOnDeath { get; set; } = true;

        public XmlSchema GetSchema() => throw new NotImplementedException();

        public void ReadXml(XmlReader reader)
        {
            var properties = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p);

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;
                if (!properties.TryGetValue(reader.Name, out var prop)) continue;

                reader.Read(); // Reads value from element.
                if (!string.IsNullOrWhiteSpace(reader.Value))
                {
                    var convertFromString = TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromString(reader.Value);
                    prop.SetValue(this, convertFromString);
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            var properties = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in properties)
            {
                if (propertyInfo.IsDefined(typeof(DescriptionAttribute), false))
                {
                    writer.WriteComment(
                        propertyInfo.GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .Cast<DescriptionAttribute>()
                            .Single()
                            .Description);
                }

                writer.WriteElementString(propertyInfo.Name, propertyInfo.GetValue(this, null).ToString());
            }
        }

        public static bool Load(string fileName = "config.xml")
        {
            fileName = GetFile(fileName);
            if (!File.Exists(fileName))
            {
                return false;
            }

            using var file = File.OpenRead(fileName);
            instance = serializer.Deserialize(file) as Config;
            return instance != null;
        }

        public void Save(string fileName = "config.xml")
        {
            using var file = File.Create(GetFile(fileName));
            serializer.Serialize(file, this);
        }

        public static void Delete(string fileName = "config.xml")
        {
            var file = GetFile(fileName);
            try
            {
                File.Delete(file);
            }
            catch
            {
                // ignored
            }
        }

        private static string GetFile(string fileName)
        {
            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), fileName);
            }
            return fileName;
        }
    }
}

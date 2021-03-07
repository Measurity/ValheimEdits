using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ValheimEdits.Serialization
{
    public abstract class ConfigBase<T> : IXmlSerializable where T : ConfigBase<T>, new()
    {
        private static T instance;

        private static readonly XmlSerializer serializer = new(typeof(T));

        private static ReadOnlyDictionary<string, PropertyInfo> serializablePropertiesCache;
        private readonly string fileName;

        protected ConfigBase(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentNullException(nameof(fileName));

            this.fileName = fileName;
        }

        private static string ConfigDir =>
            Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
            Directory.GetCurrentDirectory();

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    Load();
                    Save(); // Save again to upgrade config file.
                }
                return instance;
            }
        }

        private static ReadOnlyDictionary<string, PropertyInfo> SerializableProperties =>
            serializablePropertiesCache ??= new ReadOnlyDictionary<string, PropertyInfo>(typeof(T)
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToDictionary(p => p.Name, p => p));

        XmlSchema IXmlSerializable.GetSchema() => throw new NotImplementedException();

        void IXmlSerializable.ReadXml(XmlReader reader)
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                ;

            while (reader.Read())
            {
                if (reader.NodeType != XmlNodeType.Element) continue;
                if (!SerializableProperties.TryGetValue(reader.Name, out var prop)) continue;

                reader.Read(); // Reads value from element.
                if (!string.IsNullOrWhiteSpace(reader.Value))
                {
                    var convertFromString =
                        TypeDescriptor.GetConverter(prop.PropertyType).ConvertFromString(reader.Value);
                    prop.SetValue(this, convertFromString);
                }
            }
        }

        void IXmlSerializable.WriteXml(XmlWriter writer)
        {
            foreach (var prop in SerializableProperties.Values)
            {
                if (prop.IsDefined(typeof(DescriptionAttribute), false))
                {
                    writer.WriteComment(
                        prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .Cast<DescriptionAttribute>()
                            .Single()
                            .Description);
                }

                writer.WriteElementString(prop.Name, prop.GetValue(this, null).ToString());
            }
        }

        public static bool Load(string fileName = null)
        {
            fileName = GetFile(fileName);
            if (!File.Exists(fileName))
            {
                Save(fileName);
                return false;
            }
            
            using var file = File.OpenRead(fileName);
            instance = serializer.Deserialize(file) as T;
            var loadedSuccessfully = instance != null;
            instance ??= new T();

            return loadedSuccessfully;
        }

        public static void Save(string fileName = null)
        {
            using var file = File.Create(GetFile(fileName));
            serializer.Serialize(file, Instance);
        }

        public static void Delete(string fileName = null)
        {
            fileName = GetFile(fileName);
            try
            {
                File.Delete(fileName);
            }
            catch
            {
                // ignored
            }
        }

        private static string GetFile(string fileName)
        {
            instance ??= new T();
            fileName ??= instance.fileName;
            if (!Path.IsPathRooted(fileName))
            {
                fileName = Path.Combine(ConfigDir, fileName);
            }
            return fileName;
        }
    }
}

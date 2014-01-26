using LangStat.Contracts;
using LangStat.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace LangStat.DataAccess
{
    public class LanguagesDao : ILanguagesDao
    {
        const string baseDirectory = "Data/Languages";
        const string descriptorFileName = "language.dsc";

        public bool AddLanguage(LanguageEntity newLanguage)
        {
            var directoryName = string.Format("{0}/{1}", baseDirectory, newLanguage.Name);
            if (Directory.Exists(directoryName)) return false;

            Directory.CreateDirectory(directoryName);

            var descriptorPath = string.Format("{0}/{1}", directoryName, descriptorFileName);
            
            if (File.Exists(descriptorPath)) return false;
            using (var fileStream = File.CreateText(descriptorPath))
            {
                using (var xmlWriter = XmlWriter.Create(fileStream))
                {
                    xmlWriter.WriteStartElement("Language");
                    xmlWriter.WriteAttributeString("Name", newLanguage.Name);
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();

                    return true;
                }                
            }
        }

        public bool UpdateLanguage(LanguageEntity updatedLanguage)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLanguage(LanguageEntity language)
        {
            throw new NotImplementedException();
        }

        public LanguageEntity[] GetAllLanguages()
        {
            if (!Directory.Exists(baseDirectory)) return null;

            var directories = Directory.GetDirectories(baseDirectory);
            var languages = new List<LanguageEntity>();
            foreach (var directory in directories)
            {
                var descriptorFile = string.Format("{0}/{1}", directory, descriptorFileName);
                if (!File.Exists(descriptorFile)) continue;

                var language = new LanguageEntity();
                using (var fileStream = File.OpenText(descriptorFile))
                {
                    using (var xmlReader = XmlReader.Create(fileStream))
                    {
                        while(xmlReader.Name != "Language") xmlReader.Read();
                        var languageName = xmlReader.GetAttribute("Name", "");

                        language.Name = languageName; 
                    }
                }
                languages.Add(language);
            }

            return languages.ToArray();
        }
    }
}

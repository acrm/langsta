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

        public bool AddLanguage(LanguageDto newLanguage)
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
                }                
            }

            RaiseLanguageAdded(newLanguage);
            return true;
        }

        public bool UpdateLanguage(LanguageDto updatedLanguage)
        {
            throw new NotImplementedException();
        }

        public bool DeleteLanguage(LanguageDto language)
        {
            throw new NotImplementedException();
        }

        public LanguageDto[] GetAllLanguages()
        {
            if (!Directory.Exists(baseDirectory)) return null;

            var directories = Directory.GetDirectories(baseDirectory);
            var languages = new List<LanguageDto>();
            foreach (var directory in directories)
            {
                var descriptorFile = string.Format("{0}/{1}", directory, descriptorFileName);
                if (!File.Exists(descriptorFile)) continue;

                var language = new LanguageDto();
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
        
        public LanguageDto GetLanguage(string languageName)
        {
            if (!Directory.Exists(baseDirectory)) return null;

            var directory = string.Format("{0}/{1}", baseDirectory, languageName);
            if (!Directory.Exists(directory)) return null;

            var descriptorFile = string.Format("{0}/{1}", directory, descriptorFileName);
            if (!File.Exists(descriptorFile)) return null;

            using (var fileStream = File.OpenText(descriptorFile))
            {
                using (var xmlReader = XmlReader.Create(fileStream))
                {
                    while (xmlReader.Name != "Language") xmlReader.Read();
                    var name = xmlReader.GetAttribute("Name", "");

                    var language = new LanguageDto
                        {
                            Name = name
                        };

                    return language;
                }
            }
        }

        private void RaiseLanguageAdded(LanguageDto addedLanguage)
        {
            var handler = LanguageAdded;
            if (handler != null)
            {
                handler.Invoke(this, addedLanguage);
            }
        }

        public event EventHandler<LanguageDto> LanguageAdded;

        private void RaiseLanguageDeleted(LanguageDto deletedLanguage)
        {
            var handler = LanguageDeleted;
            if (handler != null)
            {
                handler.Invoke(this, deletedLanguage);
            }
        }

        public event EventHandler<LanguageDto> LanguageDeleted;

        private void RaiseLanguageUpdated(LanguageDto updatedLanguage)
        {
            var handler = LanguageUpdated;
            if (handler != null)
            {
                handler.Invoke(this, updatedLanguage);
            }
        }

        public event EventHandler<LanguageDto> LanguageUpdated;
    }
}

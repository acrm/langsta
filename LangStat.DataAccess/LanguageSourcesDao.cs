using LangStat.Contracts;
using LangStat.DataAccess.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace LangStat.DataAccess
{
    public class LanguageSourcesDao : ILanguageSourcesDao
    {
        const string baseDirectory = "Data/Languages";
        const string fileNameExtension = "src";
        private readonly string _languageName;

        public LanguageSourcesDao(string languageName)
        {
            _languageName = languageName;
        }
        
        private string CreateNameFromAddress(string address)
        {
            return address
                .Replace("http", "")
                .Replace(":", "")
                .Replace("//", "")
                .Replace("/", "-");
        }

        public bool AddLanguageSource(LanguageSourceEntity languageSource)
        {
            var directoryPath = string.Format("{0}/{1}",
                                            baseDirectory,
                                            _languageName);
            if (!Directory.Exists(directoryPath)) return false;

            var fileName = string.Format("{0}/{1}.{2}",
                                         directoryPath,
                                         CreateNameFromAddress(languageSource.Address),
                                         fileNameExtension);
            if (File.Exists(fileName)) return false;

            using (var fileStream = File.CreateText(fileName))
            {
                using (var xmlWriter = XmlWriter.Create(fileStream))
                {
                    xmlWriter.WriteStartElement("LanguageSource");
                    xmlWriter.WriteAttributeString("Address", languageSource.Address);
                    xmlWriter.WriteAttributeString("Id", languageSource.Id.ToString());
                    xmlWriter.WriteEndElement();
                    xmlWriter.Flush();

                    return true;
                }
            }
        }

        public LanguageSourceEntity[] GetAllLanguageSources()
        {
            var directoryPath = string.Format("{0}/{1}",
                                             baseDirectory,
                                             _languageName);
            if (!Directory.Exists(directoryPath)) return null;

            var files = Directory.EnumerateFiles(directoryPath, "*." + fileNameExtension);
            var languageSources = new List<LanguageSourceEntity>();
            foreach (var fileName in files)
            {
                using (var fileStream = File.OpenText(fileName))
                {
                    using (var xmlReader = XmlReader.Create(fileStream))
                    {
                        while (xmlReader.Name != "LanguageSource") xmlReader.Read();
                        
                        var address = xmlReader.GetAttribute("Address", "");
                        
                        var idString = xmlReader.GetAttribute("Id", "");

                        Guid id;
                        var isParsed = Guid.TryParse(idString, out id);
                        if (!isParsed)
                        {
                            id = Guid.Empty;
                        }
                        
                        var languageSource = new LanguageSourceEntity
                        {
                            Address = address,
                            Id = id
                        };
                        languageSources.Add(languageSource);
                    }
                }
            }

            return languageSources.ToArray();
        }
    }
}

using LangStat.Core.Contracts;
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
    public class LanguagesSourcesDao : ILanguageSourcesDao
    {
        const string baseDirectory = "Data/Languages";
        const string fileNameExtension = "src";
        private readonly ILanguagesDao _languagesDao;

        public LanguagesSourcesDao(ILanguagesDao languagesDao)
        {
            _languagesDao = languagesDao;
        }
        
        private string CreateNameFromAddress(string address)
        {
            return address
                .Replace("http", "")
                .Replace(":", "")
                .Replace("//", "")
                .Replace("/", "-");
        }

        public bool AddLanguageSource(string languageName, LanguageSourceDto languageSource)
        {
            var directoryPath = string.Format("{0}/{1}",
                                            baseDirectory,
                                            languageName);
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
                }
            }

            RaiseLanguageSourceAdded(languageSource);

            return true;
        }

        public LanguageSourceDto[] GetLanguageSources(string languageName)
        {
            var directoryPath = string.Format("{0}/{1}",
                                             baseDirectory,
                                             languageName);
            if (!Directory.Exists(directoryPath)) return null;

            var files = Directory.EnumerateFiles(directoryPath, "*." + fileNameExtension);
            var languageSources = new List<LanguageSourceDto>();
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
                        
                        var languageSource = new LanguageSourceDto
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

        private void RaiseLanguageSourceAdded(LanguageSourceDto addedLanguageSource)
        {
            var handler = LanguageSourceAdded;
            if (handler != null)
            {
                handler.Invoke(this, addedLanguageSource);
            }
        }

        public event EventHandler<LanguageSourceDto> LanguageSourceAdded;
    }
}

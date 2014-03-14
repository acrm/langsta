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
                    WriteLanguage(newLanguage, xmlWriter);
                    xmlWriter.Flush();
                }                
            }

            RaiseLanguageAdded(newLanguage);
            return true;
        }

        public bool UpdateLanguage(LanguageDto updatedLanguage)
        {
            if (updatedLanguage == null) return false;

            var directoryName = string.Format("{0}/{1}", baseDirectory, updatedLanguage.Name);
            if (!Directory.Exists(directoryName)) return false;

            var descriptorPath = string.Format("{0}/{1}", directoryName, descriptorFileName);
            if (!File.Exists(descriptorPath)) return false;
            
            using (var fileStream = File.OpenWrite(descriptorPath))
            {
                using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
                {
                    WriteLanguage(updatedLanguage, xmlWriter);
                    xmlWriter.Flush();
                }
            }

            RaiseLanguageUpdated(updatedLanguage);
            return true;
        }

        private static void WriteLanguage(LanguageDto updatedLanguage, XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("Language");
            xmlWriter.WriteAttributeString("Name", updatedLanguage.Name);
            xmlWriter.WriteStartElement("IgnoredWords");
            if (updatedLanguage.IgnoredWords != null)
            {
                foreach (var ignoredWord in updatedLanguage.IgnoredWords)
                {
                    xmlWriter.WriteStartElement("IgnoredWord");
                    xmlWriter.WriteString(ignoredWord);
                    xmlWriter.WriteEndElement();
                }
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
        }

        public bool DeleteLanguage(LanguageDto language)
        {
            var directoryName = string.Format("{0}/{1}", baseDirectory, language.Name);
            if (!Directory.Exists(directoryName)) return false;

            Directory.Delete(directoryName, recursive: true);
            if (Directory.Exists(directoryName)) return false;

            RaiseLanguageDeleted(language);

            return true;
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
                
                using (var fileStream = File.OpenText(descriptorFile))
                {
                    using (var xmlReader = XmlReader.Create(fileStream, new XmlReaderSettings { IgnoreWhitespace = true }))
                    {
                        var language = ReadLanguage(xmlReader);
                        languages.Add(language);
                    }
                }
            }

            return languages.ToArray();
        }

        private static LanguageDto ReadLanguage(XmlReader xmlReader)
        {
            var language = new LanguageDto();
            
            var success = FindElementOrFalse(xmlReader, "Language");
            if (!success) return null;

            var languageName = xmlReader.GetAttribute("Name", "");

            language.Name = languageName;

            success = FindElementOrFalse(xmlReader, "IgnoredWords");
            if (!success) return language;

            var ignoredWords = new List<string>();  
            while(success)
            {
                success = FindElementOrFalse(xmlReader, "IgnoredWord");
                if (!success) break;
                
                xmlReader.Read();
                if (xmlReader.NodeType == XmlNodeType.Text)
                {
                    var ignoredWord = xmlReader.ReadContentAsString();
                    if (string.IsNullOrWhiteSpace(ignoredWord)) continue;

                    ignoredWords.Add(ignoredWord); 
                }
            }
            language.IgnoredWords = ignoredWords.ToArray(); 
             
            return language;
        }

        private static bool FindElementOrFalse(XmlReader reader, string elementName)
        {
            reader.Read();
            while (reader.ReadState != ReadState.EndOfFile && reader.Name != elementName) reader.Read();
            if (reader.ReadState == ReadState.EndOfFile || reader.ReadState == ReadState.Error) return false;
            if (reader.Name == elementName) return true;
            return false;
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
                    var language = ReadLanguage(xmlReader);
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

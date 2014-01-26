using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class WordsRepository
    {
        private readonly string baseDirectory = "languages";

        public void SaveWords(string language, string[] words)
        {
            var fileName = string.Format("{0}/{1}.txt", baseDirectory, language);
            
            var builder = new StringBuilder();
            foreach (var word in words)
            {
                builder.AppendLine(word);
            }

            using (var streamWriter = new StreamWriter(fileName))
            {
                streamWriter.WriteLine(builder.ToString());
            }
        }

        public string[] LoadWords(string language)
        {
            var fileName = string.Format("{0}/{1}.txt", baseDirectory, language);
            using (var wordsStream = new StreamReader(fileName))
            {
                var content = wordsStream.ReadToEnd();
                var words = content.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                return words;
            }

        }
    }
}

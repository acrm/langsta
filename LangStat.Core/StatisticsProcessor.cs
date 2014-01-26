using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class StatisticsProcessor
    {

        public event EventHandler<string> TextLoaded;

        private void RaiseTextLoaded(string processedText)
        {
            var handler = TextLoaded;
            if (handler != null)
            {
                handler.Invoke(this, processedText);
            }
        }

        private string LoadContent(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            //var request = (HttpWebRequest)WebRequest.Create(address);
            //var response = (HttpWebResponse)request.GetResponse();
            var stream = new StreamReader(@"R:\Рома\WebParser\html\Кавычки — Википедия.htm");
            using (var reader = stream)
            {
                var content = reader.ReadToEnd();
                RaiseTextLoaded(content);

                return content;
            }
        }

        private string[] ExtractWords(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            var tokens = text
                    .Split(new char[] { ' ', '<', '>', '"', '=', '.', ',', '?', '!', ':' }, StringSplitOptions.RemoveEmptyEntries);

            return tokens;
        }

        public void SaveWords()
        {
            //Address = @"http://ru.wikipedia.org/wiki/%D0%98%D0%B7%D1%80%D0%B0%D0%B8%D0%BB%D1%8C";
            //var content = LoadContent(Address);
            //var words = ExtractWords(content);
            //_wordsRepository.SaveWords(Language, words);
        }

        public Task<string> Process()
        {
            return Task.Factory.StartNew<string>(() =>
            {
                //var content = LoadContent(Address);
                //if (content == null) return null;

                //var words = ExtractWords(content);
                //if (words == null) return null;

                //var report = CalculateStatistics(words);
                //if (report == null) return null;

                //return report;
                return "";
            });
        }

        private string CalculateStatistics(string[] words)
        {
            if (words == null) return null;

            var dictionary = new Dictionary<string, int>();
            foreach (var token in words)
            {
                var word = token.ToLower();
                if (!dictionary.ContainsKey(word))
                {
                    dictionary[word] = 1;
                }
                else
                {
                    dictionary[word]++;
                }
            }

            var statistics = new WordStatistics[dictionary.Count];
            int i = 0;
            foreach (var record in dictionary)
            {
                statistics[i] = new WordStatistics { Word = record.Key, CountOfAccurances = record.Value };
                i++;
            }

            var orderedStatistics = statistics.OrderByDescending(statistic => statistic.CountOfAccurances);

            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Всего слов: " + statistics.Length);
            foreach (var statistic in orderedStatistics)
            {
                stringBuilder.AppendLine(string.Format("{0} - {1}", statistic.Word, statistic.CountOfAccurances));
            }
            var output = stringBuilder.ToString();

            return output;
        }

        private struct WordStatistics
        {
            public string Word { get; set; }

            public int CountOfAccurances { get; set; }
        }
    }
}

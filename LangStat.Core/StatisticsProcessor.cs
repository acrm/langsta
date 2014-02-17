using LangStat.Core.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LangStat.Core
{
    public class StatisticsProcessor
    {
        private readonly ILanguagesRepository _languagesRepository;

        public StatisticsProcessor(ILanguagesRepository languagesRepository)
        {
            _languagesRepository = languagesRepository;
        }

        public string BuildLanguageStatistics(string languageName)
        {
            var language = _languagesRepository.GetLanguage(languageName);
            if (language == null) return "Нет выбранного языка.";

            var languageSources = language.LanguageSourcesRepository.GetAllLanguageSources();
            if (languageSources == null || languageSources.Length == 0) return "Отсутствуют источники языка";


            var languageStatistics = new StringBuilder();
            foreach (var languageSource in languageSources)
            {
                var statistics = BuildLanguageSourceStatistics(languageSource);

                languageStatistics.AppendFormat("Источник: {0}\n", languageSource.Address);
                languageStatistics.AppendLine(statistics);
                languageStatistics.AppendLine();
            }

            return languageStatistics.ToString();
        }

        private string BuildLanguageSourceStatistics(LanguageSource languageSource)
        {
            if (languageSource == null) return null;

            var content = LoadContent(languageSource.Address);
            if (content == null) return null;

            string[] misfits;
            var words = ExtractWords(content, out misfits);
            if (words == null || words.Length == 0) return null;

            var statistics = CalculateStatistics(words);
            var misfitsStatistics = CalculateStatistics(misfits);
            return string.Format("{0}\n\n Отбракованные слова:\n{1}", statistics, misfitsStatistics);
        }
                
        private string LoadContent(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;

            if (!address.StartsWith("http://"))
            {
                address = "http://" + address;
            }

            var request = (HttpWebRequest)WebRequest.Create(address);
            var response = (HttpWebResponse)request.GetResponse();
            using (var reader = new StreamReader(response.GetResponseStream()))
            {
                var content = reader.ReadToEnd();

                return content;
            }
        }        

        private string[] ExtractWords(string html, out string[] misfits)
        {
            misfits = null;

            if (string.IsNullOrWhiteSpace(html)) return null;

            var tagPattern = new Regex("(<[^>]+>)");
            var scriptTagPattern = new Regex("<script>.*</script>");
            var nbspPattern = new Regex("&nbsp;");
            var wordPattern = new Regex(@"\s+(\w+)\s+");
            var wordStrongPattern = new Regex(@"[^\d]+");

            var text = tagPattern.Replace(html, " ");
            text = nbspPattern.Replace(text, " ");
            var wordsMatches = wordPattern.Matches(text);
            
            var words = new List<string>(wordsMatches.Count);
            var misfitsList = new List<string>(wordsMatches.Count);
            foreach(Match wordMatch in wordsMatches)
            {
                var candidate = wordMatch.Groups[1].Value;
                if (!wordStrongPattern.IsMatch(candidate))
                {
                    misfitsList.Add(candidate); 
                    continue;
                }

                words.Add(string.Format("[{0}]", wordMatch.Groups[1].Value));
            }

            misfits = misfitsList.ToArray();

            return words
                .Where(word => !string.IsNullOrEmpty(word))
                .ToArray();
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

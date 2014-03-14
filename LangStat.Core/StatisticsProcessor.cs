using LangStat.Contracts;
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

        public Task<LanguageStatisticsBuildingResult> BuildLanguageStatisticsAsync(string languageName)
        {
            return Task.Factory.StartNew(() => BuildLanguageStatistics(languageName));
        }
        
        public LanguageStatisticsBuildingResult BuildLanguageStatistics(string languageName)
        {
            var language = _languagesRepository.GetLanguage(languageName);
            if (language == null) 
                return new LanguageStatisticsBuildingResult 
                    {
                        BuildIsSuccessful = false,
                        ErrorMessage = "Нет выбранного языка" 
                    };

            var languageSources = language.LanguageSourcesRepository.GetAllLanguageSources();
            if (languageSources == null || languageSources.Length == 0)
                return new LanguageStatisticsBuildingResult
                    {
                        BuildIsSuccessful = false,
                        ErrorMessage = "Отсутствуют источники языка"
                    };

            var buildingResults = languageSources
                .Select(BuildLanguageSourceStatistics)
                .Where(buildingResult => buildingResult != null)
                .ToArray();

            var ignoredWords = new HashSet<string>(language.IgnoredWordsRepository.GetAllWords());
            
            var errorMessages = new List<string>();
            var dic = new Dictionary<string, int>();
            var wordsTotalCount = 0;
            foreach (var buildingResult in buildingResults)
            {
                if (!buildingResult.BuildIsSuccessful)
                {
                    errorMessages.Add(buildingResult.ErrorMessage);
                    continue;
                }                 
                
                var statistics = buildingResult.Result;
                if (statistics == null) continue;

                wordsTotalCount += statistics.WordsTotalCount;

                foreach (var wordStatistics in statistics.UniqueWords)
                {
                    if (ignoredWords.Contains(wordStatistics.Spelling)) continue;

                    if (!dic.ContainsKey(wordStatistics.Spelling))
                    {
                        dic[wordStatistics.Spelling] = wordStatistics.CountOfAccurances;
                    }
                    else
                    {
                        dic[wordStatistics.Spelling] += wordStatistics.CountOfAccurances;
                    }
                }                
            }
            
            var wordsStatistics = new WordStatistics[dic.Count];
            var i = 0;
            foreach (var record in dic)
            {
                wordsStatistics[i] = new WordStatistics 
                {
                    Spelling = record.Key,
                    CountOfAccurances = record.Value 
                };
                i++;
            }

            var languageStatistics = new LanguageStatistics 
            {
                WordsTotalCount = wordsTotalCount,
                UniqueWords = wordsStatistics
            };

            return new LanguageStatisticsBuildingResult
                {
                    Result = languageStatistics,
                    ErrorMessage = errorMessages.Count > 0
                        ? string.Join("\n", errorMessages)
                        : null
                };
        }

        private SourceStatisticsBuildingResult BuildLanguageSourceStatistics(LanguageSource languageSource)
        {
            if (languageSource == null) 
                return new SourceStatisticsBuildingResult
                {
                    BuildIsSuccessful = false,
                    ErrorMessage = "Отсутствует источник."
                };

            string errorMessage;
            var content = LoadContent(languageSource.Address, out errorMessage);
            if (content == null)
                return new SourceStatisticsBuildingResult
                {
                    BuildIsSuccessful = false,
                    ErrorMessage = errorMessage
                };
                        
            string[] misfits;
            var words = ExtractWords(content, out misfits);
            if (words == null || words.Length == 0)
                return new SourceStatisticsBuildingResult
                {
                    BuildIsSuccessful = false,
                    ErrorMessage = "Не извлечено ни одного слова."
                };

            var statistics = new SourceStatistics
            {
                WordsTotalCount = words.Length,
                UniqueWords = CalculateUniqueWords(words)
            };

            return new SourceStatisticsBuildingResult { Result = statistics };
        }
                
        private string LoadContent(string address, out string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(address))
            {
                errorMessage = "Некорретный адрес.";
                return null;
            }

            if (!address.StartsWith("http://"))
            {
                address = "http://" + address;
            }

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(address);
                request.Timeout = (int)TimeSpan.FromSeconds(5).TotalMilliseconds;
                var response = (HttpWebResponse)request.GetResponse();

                using (var reader = new StreamReader(response.GetResponseStream()))
                {
                    var content = reader.ReadToEnd();
                    errorMessage = null; 
                    return content;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null; 
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

                words.Add(string.Format("{0}", wordMatch.Groups[1].Value.Trim()));
            }

            misfits = misfitsList.ToArray();

            return words
                .Where(word => !string.IsNullOrEmpty(word))
                .ToArray();
        }
        
        private WordStatistics[] CalculateUniqueWords(string[] words)
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
                statistics[i] = new WordStatistics { Spelling = record.Key, CountOfAccurances = record.Value };
                i++;
            }
                    
            return statistics;
        }

        private class SourceStatistics
        {
            public int WordsTotalCount { get; set; }

            public WordStatistics[] UniqueWords { get; set; }
        }

        private class SourceStatisticsBuildingResult
        {
            public SourceStatisticsBuildingResult()
            {
                BuildIsSuccessful = true;
            }

            public bool BuildIsSuccessful { get; set; }

            public string ErrorMessage { get; set; }

            public SourceStatistics Result { get; set; }
        }
    }
}

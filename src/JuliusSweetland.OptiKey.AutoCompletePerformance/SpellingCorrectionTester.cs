using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JuliusSweetland.OptiKey.Enums;
using JuliusSweetland.OptiKey.Extensions;
using JuliusSweetland.OptiKey.Models;
using JuliusSweetland.OptiKey.Properties;
using JuliusSweetland.OptiKey.Services.AutoComplete;

namespace JuliusSweetland.OptiKey.AutoCompletePerformance
{
    /// <summary>
    ///     Given a mispelt word, start typing it and grab the top 4 suggestions.
    ///     If any of these suggestions is the correct word, then capture:
    ///     * how many characters we entered, or -1 if the correct word never appeared
    ///     * how long this process took
    ///     * the misspelt and target words
    /// </summary>
    internal class SpellingCorrectionTester
    {
        private const string DictionaryFileType = "dic";
        private const int NumberOfSuggestionsToCheck = 4;
        private const string OriginalDictionariesSubPath = @"Dictionaries";
        private readonly IManageAutoComplete autoComplete;

        public SpellingCorrectionTester(AutoCompleteMethods autoCompleteMethod, Languages language)
        {
            switch (autoCompleteMethod)
            {
                case AutoCompleteMethods.NGram:
                    autoComplete = new NGramAutoComplete();
                    break;
                case AutoCompleteMethods.Basic:
                    autoComplete = new BasicAutoComplete();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("autoCompleteMethod", autoCompleteMethod, null);
            }

            Configure(language);
        }

        public SpellingCorrectionTestResult TestWord(MisspellingTest misspellingTest)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var charactersTyped = -1;
            for (var n = 1; n <= misspellingTest.Misspelling.Length; ++n)
            {
                var typedSoFar = misspellingTest.Misspelling.Substring(0, n);
                var suggestions = autoComplete.GetSuggestions(typedSoFar).Take(NumberOfSuggestionsToCheck);

                if (!suggestions.Any(
                        suggestion =>
                            suggestion.Equals(misspellingTest.TargetWord, StringComparison.CurrentCultureIgnoreCase)))
                {
                    continue;
                }

                charactersTyped = n;
                break;
            }
            stopwatch.Stop();

            var timeTaken = stopwatch.Elapsed;
            return new SpellingCorrectionTestResult(charactersTyped, timeTaken, misspellingTest);
        }

        private void AddEntryToDictionary(string entry)
        {
            if (string.IsNullOrWhiteSpace(entry))
            {
                return;
            }

            var hash = entry.NormaliseAndRemoveRepeatingCharactersAndHandlePhrases(false);
            if (string.IsNullOrWhiteSpace(hash))
            {
                return;
            }

            var newEntryWithUsageCount = new DictionaryEntry(entry);

            autoComplete.AddEntry(entry, newEntryWithUsageCount);
        }

        private void Configure(Languages keyboardAndDictionaryLanguage)
        {
            var originalDictionaryPath =
                Path.GetFullPath(Path.Combine(OriginalDictionariesSubPath,
                    Path.ChangeExtension(keyboardAndDictionaryLanguage.ToString(), DictionaryFileType)));

            if (File.Exists(originalDictionaryPath))
            {
                LoadOriginalDictionaryFromFile(originalDictionaryPath);
            }
            else
            {
                throw new ApplicationException(string.Format(Resources.DICTIONARY_FILE_NOT_FOUND_ERROR,
                    originalDictionaryPath));
            }
        }

        private void LoadOriginalDictionaryFromFile(string filePath)
        {
            using (var reader = File.OpenText(filePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    //Entries must be londer than 1 character
                    if (!string.IsNullOrWhiteSpace(line) && (line.Trim().Length > 1))
                    {
                        AddEntryToDictionary(line.Trim());
                    }
                }
            }
        }
    }
}
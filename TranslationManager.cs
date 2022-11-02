using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;
using System.Text;
using System.Net;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Web;
using System.IO;
using File = Godot.File;
using System.Web.UI.WebControls;
using System.Reflection;

namespace godotlocalizationeditor
{

    public class TranslationManager : ITranslationManager
    {
        Dictionary<int, LocalizedTexts> localizations { get; set; }

        int targetTextIndex { get; set; }

        int referenceTextIndex { get; set; }

        string selectedKey { get; set; }

        List<string> languagesList { get; set; }

        List<string> keysList { get; set; }

        string filePath;

        public TranslationManager()
        {
            localizations = new Dictionary<int, LocalizedTexts>();

            languagesList = new List<string>();

            keysList = new List<string>();
        }

        public string GetReferenceText()
        {
            if (selectedKey == null) return string.Empty;

            return localizations[referenceTextIndex].Texts[selectedKey];
        }

        public List<string> GetKeysBySearchTerm(string searchTerm)
        {
            var matchInRef = localizations[referenceTextIndex].Texts
                .Where(t => t.Value.ToLower().Contains(searchTerm.ToLower()) || t.Key.ToLower().Contains(searchTerm.ToLower()))
                .Select(t => t.Key);

            var matchInTarget = localizations[targetTextIndex].Texts
                .Where(t => t.Value.ToLower().Contains(searchTerm.ToLower()))
                .Select(t => t.Key);

            return matchInRef.Concat(matchInTarget).ToList();           
        }

        public string GetTargetText()
        {
            if (selectedKey == null) return string.Empty;

            return localizations[targetTextIndex].Texts[selectedKey];
        }


        public void UpdateTargetLanguage(string newText)
        {
            if (selectedKey == null) return;

            localizations[targetTextIndex].Texts[selectedKey] = newText;
        }

        public string GetReferenceLanguage()
        {
            return languagesList[referenceTextIndex];
        }

        public string GetTargetLanguage()
        {
            return languagesList[targetTextIndex];
        }

        public string GetCurrentKey()
        {
            return selectedKey;
        }

        public List<string> GetAllKeys()
        {
            return keysList;
        }

        public List<string> GetAllLanguages()
        {
            return languagesList;
        }

        public void SelectReferenceLanguage(int newIndex)
        {
            referenceTextIndex = newIndex;
        }

        public void SelectTargetLanguage(int newIndex)
        {
            targetTextIndex = newIndex;
        }

        public void CallAPI(HTTPRequest hTTPRequest, string apiKey, bool mock)
        {
            var auth = $"Authorization: DeepL-Auth-Key {apiKey}";

            var contentTpye = "Content-Type: application/json";

            var message = new ApiMessage() { text = "Hello!", source_lang = "HU", target_lang = "RU" };

            var jsonMessage = JsonConvert.SerializeObject(message);

            var url = mock ? "http://localhost:3000/v2/translate" : "https://api-free.deepl.com/v2/translate";

            hTTPRequest.Request(url, new string[] { auth, contentTpye }, true, HTTPClient.Method.Post, jsonMessage);
        }

        public string HandleAPIResponse(int result, int response_code, String[] headers, byte[] body)
        {
            string jsonStr = Encoding.UTF8.GetString(body);

            if (response_code == 200)
            {
                DebugHelper.PrettyPrintVerbose(jsonStr);

                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(jsonStr);

                DebugHelper.PrettyPrintVerbose(apiResponse.translations.First()?.text);

                return apiResponse.translations.First()?.text;
            }

            DebugHelper.PrettyPrintVerbose($"Http response code: {response_code}");

            return string.Empty;
        }


        public void MergeFiles(string path)
        {
            var lines = ReadLinesFromFile(path);

            var firstLine = lines.First().Split(";");

            var sisterLanguagesList = new List<string>();

            var sisterLocalizations = new Dictionary<int, LocalizedTexts>();

            var sisterKeys = new List<string>();

            for (int i = 1; i < firstLine.Length; i++)
            {
                sisterLanguagesList.Add(firstLine[i]);

                sisterLocalizations.Add(i - 1, new LocalizedTexts() { Locale = firstLine[i], Texts = new Dictionary<string, string>() });
            }

            ProcessFileData(lines, sisterLocalizations, sisterKeys);


            for (int j = 0; j < sisterLanguagesList.Count; j++)
            {
                var currentSisterLocalizedText = sisterLocalizations[j];

                var currentLocalizedText = localizations.Where(l => l.Value.Locale == sisterLanguagesList[j]).FirstOrDefault().Value;

                if (currentLocalizedText == default)
                {
                    AddLanguage(sisterLanguagesList[j]);

                    currentLocalizedText = localizations.Where(l => l.Value.Locale == sisterLanguagesList[j]).FirstOrDefault().Value;
                }

                for (int i = 0; i < sisterKeys.Count; i++)
                {
                    var currentSisterKey = sisterKeys[i];

                    if (!currentLocalizedText.Texts.TryGetValue(currentSisterKey, out _))
                    {
                        AddNewKey(currentSisterKey);
                    }

                    if (currentLocalizedText.Texts[currentSisterKey] == "MT" && currentSisterLocalizedText.Texts[currentSisterKey] != "MT")
                    {
                        currentLocalizedText.Texts[currentSisterKey] = currentSisterLocalizedText.Texts[currentSisterKey];
                    }
                }
            }

        }

        public void LoadData(String path)
        {
            var lines = ReadLinesFromFile(path);

            var firstLine = lines.First().Split(";");

            for (int i = 1; i < firstLine.Length; i++)
            {
                languagesList.Add(firstLine[i]);

                localizations.Add(i - 1, new LocalizedTexts() { Locale = firstLine[i], Texts = new Dictionary<string, string>() });
            }

            ProcessFileData(lines, localizations, keysList);
        }

        private List<string> ReadLinesFromFile(String path)
        {
            DebugHelper.PrettyPrintVerbose($"Selected file: {path}", ConsoleColor.Green);

            filePath = path;

            var file = new File();

            if (!file.FileExists(path)) return new List<string>();

            file.Open(path, File.ModeFlags.Read);

            var lines = file.GetLines();

            file.Close();

            return lines;
        }

        private void ProcessFileData(List<string> lines, Dictionary<int, LocalizedTexts> localizationDictionary, List<string> loaclizationKeys)
        {
            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i].Split(";");

                ProcessLine(line, localizations, keysList);
            }
        }

        private void ProcessLine(string[] line, Dictionary<int, LocalizedTexts> localizationDictionary, List<string> loaclizationKeys)
        {
            var key = line.First();

            loaclizationKeys.Add(key);

            for (int c = 1; c < line.Length; c++)
            {
                if (localizationDictionary[c - 1] == null) continue;

                if (!localizationDictionary[c - 1].Texts.Any(t => t.Key == key))
                {
                    localizationDictionary[c - 1].Texts.Add(key, line[c]);
                }
            }
        }

        public void SelectKeyByIndex(int index)
        {
            selectedKey = keysList[index];
        }

        public List<string> GetAllLines(Dictionary<int, LocalizedTexts> localizationDictionary, List<string> languagesToSave)
        {
            var result = new List<string>();

            var firstLine = "ID";

            localizationDictionary.ToList().ForEach(l => { firstLine = firstLine + $";{l.Value.Locale}"; });

            result.Add(firstLine);

            for (int i = 0; i < keysList.Count(); i++)
            {
                var newLine = keysList[i];

                if (newLine == String.Empty) continue;

                for (int j = 0; j < languagesToSave.Count; j++)
                {
                    newLine = $"{newLine};{localizationDictionary[j].Texts[keysList[i]]}";
                }

                result.Add(newLine);
            
            }

            return result;
        }

        public void SaveData()
        {
            StoreLinesToFile(filePath);
        }

        public void SaveData(string path)
        {
            StoreLinesToFile(path);
        }

        private void StoreLinesToFile(string path)
        {
            StoreLinesToFile(path, localizations, languagesList);
        }

        private void StoreLinesToFile(string path, Dictionary<int, LocalizedTexts> localizationDictionary, List<string> languagesToSave)
        {
            var lines = GetAllLines(localizationDictionary, languagesToSave);

            var file = new File();

            file.Open(path, File.ModeFlags.Write);

            for (int i = 0; i < lines.Count(); i++)
            {
                file.StoreLine(lines[i]);
            }

            file.Close();

            var languagesString = "";

            languagesToSave.ForEach(l => { languagesString = $"{languagesString} {l},"; });

            DebugHelper.PrettyPrintVerbose($"Saved csv to: {path} - Languages: {languagesString}");
        }

        public void AddLanguage(string locale)
        {
            if (locale == null || languagesList.Any(l => l == locale)) return;

            var index = languagesList.Count();

            languagesList.Add(locale);

            localizations.Add(index, new LocalizedTexts() {Locale = locale, Texts = new Dictionary<string, string>() });

            for (int i = 0; i < keysList.Count; i++)
            {
                localizations[index].Texts.Add(keysList[i], "MT");
            }
        }

        public void AddNewKey(string key)
        {
            if (key == null || keysList.Any(l => l == key)) return;

            keysList.Add(key);

            for (int i = 0; i < languagesList.Count; i++)
            {
                localizations[i].Texts.Add(key, "MT");
            }
        }

        public void ExportPartial(string path)
        {
            var partialLocalizations = new Dictionary<int, LocalizedTexts>()
            {
                { 0, localizations[referenceTextIndex] },
                { 1, localizations[targetTextIndex]}
            };

            var languagesToSave = new List<string>() { localizations[referenceTextIndex].Locale, localizations[targetTextIndex].Locale };

            StoreLinesToFile(path, partialLocalizations, languagesToSave);
        }
    }
}

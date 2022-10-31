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
            return localizations[referenceTextIndex].Texts
                .Where(t => t.Value.ToLower().Contains(searchTerm.ToLower()))
                .Select(t => t.Key)
                .ToList();
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

        public void LoadData(String path)
        {
            DebugHelper.PrettyPrintVerbose($"Selected file: {path}", ConsoleColor.Green);

            filePath = path;

            var file = new File();

            if (!file.FileExists(path)) return;

            file.Open(path, File.ModeFlags.Read);

            var lines = file.GetLines();

            file.Close();

            var firstLine = lines.First().Split(";");

            for (int i = 1; i < firstLine.Length; i++)
            {
                languagesList.Add(firstLine[i]);

                localizations.Add(i - 1, new LocalizedTexts() { Index = i - 1, Locale = firstLine[i], Texts = new Dictionary<string, string>() });
            }

            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i].Split(";");

                ProcessLine(line);
            }
        }

        private void ProcessLine(string[] line)
        {
            var key = line.First();

            keysList.Add(key);

            for (int c = 1; c < line.Length; c++)
            {
                if (localizations[c - 1] == null) continue;

                if (!localizations[c - 1].Texts.Any(t => t.Key == key))
                {
                    localizations[c - 1].Texts.Add(key, line[c]);
                }
            }
        }

        public void SelectKeyByIndex(int index)
        {
            selectedKey = keysList[index];
        }

    }
}

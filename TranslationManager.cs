using Godot;
using System.Collections.Generic;
using godotlocalizationeditor;
using System;
using System.Linq;
using System.Text;
using System.Net;

namespace godotlocalizationeditor
{
    public class TranslationManager: ITranslationManager
    {
        public Dictionary<int, LocalizedTexts> localizations { get; set; }

        public int targetTextIndex { get; set; }

        public int referenceTextIndex { get; set; }

        public string selectedKey { get; set; }

        public List<string> Languages { get; set; }

        public List<string> Keys { get; set; }

        public TranslationManager()
        {
            localizations = new Dictionary<int, LocalizedTexts>();

            Languages = new List<string>();

            Keys = new List<string>();
        }

        public string GetReferenceText()
        {
            if (selectedKey == null) return string.Empty;

            return localizations[referenceTextIndex].Texts[selectedKey];
        }

        public string GetTargetText()
        {
            if (selectedKey == null) return string.Empty;

            return localizations[targetTextIndex].Texts[selectedKey];
        }

        public string GetReferenceLanguage()
        {
            return Languages[referenceTextIndex];
        }

        public string GetTargetLanguage()
        {
            return Languages[targetTextIndex];
        }

        public string GetCurrentKey()
        {
            return selectedKey;
        }

        public List<string> GetAllKeys()
        {
            return Keys;
        }

        public List<string> GetAllLanguages()
        {
            return Languages;
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

            var contentTpye = "Content-Type: application/x-www-form-urlencoded";

            var url = mock ? "http://localhost:3000/v2/translate" : "https://api-free.deepl.com/v2/translate";

            hTTPRequest.Request(url, new string[] { auth, contentTpye }, true, HTTPClient.Method.Post, "text=Hello%2C%20world!&target_lang=DE");
        }

        public void HandleAPIResponse(int result, int response_code, String[] headers, byte[] body)
        {
            JSONParseResult json = JSON.Parse(Encoding.UTF8.GetString(body));

            if (response_code == 200)
            {
                DebugHelper.PrettyPrintVerbose(json.Result);

                return;
            }

            DebugHelper.PrettyPrintVerbose($"Http response code: {response_code}");
        }

        public void LoadData(String path)
        {
            DebugHelper.PrettyPrintVerbose($"Selected file: {path}", ConsoleColor.Green);

            var file = new File();

            if (!file.FileExists(path)) return;

            file.Open(path, File.ModeFlags.Read);

            var lines = file.GetLines();

            file.Close();

            var languages = lines.First().Split(";");

            for (int i = 1; i < languages.Length; i++)
            {
                Languages.Add(languages[i]);

                localizations.Add(i-1, new LocalizedTexts() { Index = i-1, Locale = languages[i], Texts = new Dictionary<string, string>() });
            }

            for (int i = 1; i < lines.Count; i++)
            {
                var line = lines[i].Split(";");

                ProcessLine(line);
            }
        }

        private void ProcessLine(string[] line)
        {
            Keys.Add(line.First());

            for (int c = 1; c < line.Length; c++)
            {
                if (localizations[c-1] == null) continue;

                if (!localizations[c-1].Texts.Any(t => t.Key == line.First()))
                {
                    localizations[c-1].Texts.Add(line.First(), line[c]);
                }
            }
        }

        public void SelectKeyByIndex(int index)
        {
            selectedKey = Keys[index];
        }


    }
}

using Godot;
using System;
using System.Collections.Generic;

namespace godotlocalizationeditor
{
    internal interface ITranslationManager
    {
        string GetReferenceText();

        string GetReferenceLanguage();

        string GetTargetText();

        string GetTargetLanguage();

        void UpdateTargetLanguage(string newText);

        void SelectKeyByIndex(int index);

        string GetCurrentKey();

        List<string> GetAllKeys();

        List<string> GetAllLanguages();

        void SelectReferenceLanguage(int newIndex);

        void SelectTargetLanguage(int newIndex);

        void CallAPI(HTTPRequest hTTPRequest, TranslationRequestParams trParams);

        string HandleAPIResponse(int result, int response_code, String[] headers, byte[] body);

        void LoadData(String path);

        void SaveData();

        void SaveData(String path);

        void MergeFiles(String path);

        void ExportPartial(String path);

        void Copy();

        List<string> GetKeysBySearchTerm(string searchTerm);

        List<string> GetAllLines(Dictionary<int, LocalizedTexts> localizationDictionary, List<string> languagesToSave);

        void AddLanguage(string locale);

        void AddNewKey(string key);
    }
}

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace godotlocalizationeditor
{
    internal interface ITranslationManager
    {
        string GetReferenceText();

        string GetReferenceLanguage();

        string GetTargetText();

        string GetTargetLanguage();

        void SelectKeyByIndex(int index);

        string GetCurrentKey();

        List<string> GetAllKeys();

        List<string> GetAllLanguages();

        void SelectReferenceLanguage(int newIndex);

        void SelectTargetLanguage(int newIndex);

        void CallAPI(HTTPRequest hTTPRequest, string apiKey, bool mock);

        void HandleAPIResponse(int result, int response_code, String[] headers, byte[] body);

        void LoadData(String path);
    }
}

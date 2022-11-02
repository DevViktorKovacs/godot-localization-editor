namespace godotlocalizationeditor
{
    public class TranslationRequestParams
    {
        public string Text { get; set; }

        public string SourceLanguage { get; set; }

        public string TargetLanguage { get; set; }

        public string APIKey { get; set; }

        public bool Mock { get; set; }
    }
}

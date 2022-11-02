
namespace godotlocalizationeditor
{
    internal class ApiResponse
    {
        public Translation[] translations { get; set; }
    }

    public class Translation
    {
        public string detected_source_language { get; set; }
        public string text { get; set; }
    }
}

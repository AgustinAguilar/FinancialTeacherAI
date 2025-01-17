using Newtonsoft.Json;

namespace FinancialTeacherAI.Services.DTO
{
    public class Input
    {
        [JsonProperty("source_sentence")]
        public string SourceSentence { get; set; }
        [JsonProperty("sentences")]
        public List<string> Sentences { get; set; }
    }
}

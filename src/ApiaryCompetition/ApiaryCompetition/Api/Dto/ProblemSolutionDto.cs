using Newtonsoft.Json;

namespace ApiaryCompetition.Api.Dto
{
    public class ProblemSolutionDto
    {
        [JsonProperty("path")]
        public string Path { get; set; }
    }
}

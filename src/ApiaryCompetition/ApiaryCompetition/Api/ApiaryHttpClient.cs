using ApiaryCompetition.Api.Dto;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ApiaryCompetition.Api
{
    public class ApiaryHttpClient : IDisposable
    {
        public const int RequiredDelay = 1500;

        readonly HttpClient httpClient;
        readonly CamelCaseJsonConvert jsonConvert;
        readonly bool saveRequests;
        const string SaveDirectory = "ApiLog";

        public ApiaryHttpClient(bool saveRequests = false)
        {
            httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(10)
            };
            jsonConvert = new CamelCaseJsonConvert();

            this.saveRequests = saveRequests;
            if (saveRequests)
                Directory.CreateDirectory(SaveDirectory);
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<ProblemDefinitionDto> GetProblemDefinitionAsync()
        {
            var response = await httpClient.GetAsync(Endpoints.TaskDefinitionUrl);
            var content = await response.Content.ReadAsStringAsync();

            var result = jsonConvert.Deserialize<ProblemDefinitionDto>(content);
            if (saveRequests)
                await File.WriteAllTextAsync($"{SaveDirectory}/problem-{result.Id}.json", content);
            return result;
        }

        HttpContent PrepareJsonContent(object o) =>
            new StringContent(jsonConvert.Serialize(o), Encoding.UTF8, "application/json");

        public async Task<ProblemSolutionResponseDto> PutSolution(ProblemDefinitionDto definition, ProblemSolutionDto solution)
        {
            var response = await httpClient.PutAsync(Endpoints.GetTaskSubmitUrl(definition.Id), PrepareJsonContent(solution));
            var content = await response.Content.ReadAsStringAsync();

            var result = jsonConvert.Deserialize<ProblemSolutionResponseDto>(content);
            if (saveRequests)
                await File.WriteAllTextAsync($"{SaveDirectory}/solution-{definition.Id}.json", solution.Path);
            return result;
        }

        internal class Endpoints
        {
            public const string TaskDefinitionUrl = "http://tasks-rad.quadient.com:8080/task";
            const string TaskSubmitUrl = "http://tasks-rad.quadient.com:8080/task";

            public static string GetTaskSubmitUrl(int id) => $"{TaskSubmitUrl}/{id}";
        }
    }
}

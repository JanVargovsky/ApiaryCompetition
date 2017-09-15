using ApiaryCompetition.Api.Dto;
using Newtonsoft.Json;
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
        readonly bool saveRequests;

        public ApiaryHttpClient(bool saveRequests = false)
        {
            httpClient = new HttpClient();
            this.saveRequests = saveRequests;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<ProblemDefinitionDto> GetProblemDefinitionAsync()
        {
            var response = await httpClient.GetAsync(Endpoints.TaskDefinitionUrl);
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ProblemDefinitionDto>(content);
            if (saveRequests)
                await File.WriteAllTextAsync($"ApiCache/problem-{result.Id}.json", content);
            return result;
        }

        HttpContent PrepareJsonContent(object o) =>
            new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");

        public async Task<ProblemSolutionResponseDto> PutSolution(ProblemDefinitionDto definition, ProblemSolutionDto solution)
        {
            var response = await httpClient.PutAsync(Endpoints.GetTaskSubmitUrl(definition.Id), PrepareJsonContent(solution));
            var content = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<ProblemSolutionResponseDto>(content);
            if (saveRequests)
                await File.WriteAllTextAsync($"ApiCache/solution-{definition.Id}.json", solution.Path);
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

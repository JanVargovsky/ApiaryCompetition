using ApiaryCompetition.Api.Dto;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ApiaryCompetition.Api
{
    public class ApiaryHttpClient : IDisposable
    {
        readonly HttpClient httpClient;

        public ApiaryHttpClient()
        {
            httpClient = new HttpClient();
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

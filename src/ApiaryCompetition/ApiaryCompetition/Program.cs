using ApiaryCompetition.Api;
using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiaryCompetition
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProblemSolver solver = new ProblemSolver();
            using (var apiaryClient = new ApiaryHttpClient())
            {
                Console.WriteLine("Getting problem ...");
                var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
                Console.WriteLine($"Solving {problemDefinition.Id}");

                Stopwatch sw = Stopwatch.StartNew();
                string solution = solver.Solve(problemDefinition);
                sw.Stop();
                Console.WriteLine($"Solved [{sw.ElapsedMilliseconds}ms]");
                Console.WriteLine("Posting solution");
                var response = await apiaryClient.PutSolution(problemDefinition, new ProblemSolutionDto
                {
                    Path = solution
                });

                Console.WriteLine($"Valid: {(response.Valid ? "yes" : "no")}");
                Console.WriteLine($"In time: {(response.InTime ? "yes" : "no")}");
                if (!string.IsNullOrEmpty(response.Message))
                    Console.WriteLine($"Message: {response.Message}");
            }
        }
    }
}

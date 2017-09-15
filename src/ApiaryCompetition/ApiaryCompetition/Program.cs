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
            var program = new Program();

            //await program.RunAsync(); // classic one time run
            await program.SequentialMineProblemsAsync(); // mining
        }

        async Task RunAsync()
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

        async Task<DateTime> ThrottleAsync(DateTime dt, int requiredPause)
        {
            var elapsedTime = (DateTime.Now - dt);
            var remainingPause = requiredPause - (int)elapsedTime.TotalMilliseconds;
            if (remainingPause > 0)
                await Task.Delay(remainingPause);

            return DateTime.Now;
        }

        async Task SequentialMineProblemsAsync()
        {
            var solver = new ProblemSolver();
            var apiaryClient = new ApiaryHttpClient();

            using (var solutionService = new ProblemSolutionService())
            {
                int fails = 0;
                const int MaxFailInRowCount = 3;
                DateTime time = DateTime.MinValue;
                while (true)
                {
                    try
                    {
                        time = await ThrottleAsync(time, ApiaryHttpClient.RequiredDelay);
                        var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
                        string path = solver.Solve(problemDefinition);
                        time = await ThrottleAsync(time, ApiaryHttpClient.RequiredDelay);
                        var problemSolution = new ProblemSolutionDto
                        {
                            Path = path
                        };
                        var response = await apiaryClient.PutSolution(problemDefinition, problemSolution);

                        if (!response.Valid || !response.InTime)
                        {
                            var ex = new Exception($"Invalid solution for problem={problemDefinition.Id}");
                            ex.Data[nameof(ProblemSolutionDto)] = problemSolution;
                            ex.Data[nameof(ProblemDefinitionDto)] = problemDefinition;
                            throw ex;
                        }

                        var added = await solutionService.RegisterSolutionAsync(problemDefinition.Id, path);
                        if (!added)
                            fails++;
                        else
                            fails = 0;

                        if (fails == MaxFailInRowCount)
                            break;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex);
                        Environment.Exit(1);
                    }
                }
            }
        }
    }
}

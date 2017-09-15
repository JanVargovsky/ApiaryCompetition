using ApiaryCompetition.Api;
using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using CommandLine;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ApiaryCompetition
{
    class Program
    {
        static async Task Main(string[] args)
        {
            CLIOptions options = null;
            var result = Parser.Default.ParseArguments<CLIOptions>(args)
                .WithParsed(opt => options = opt)
                .WithNotParsed(_ => Environment.Exit(1));

            await RunAsync(options);
        }

        static Task RunAsync(CLIOptions options)
        {
            switch (options.RunType)
            {
                case RunType.OneTime:
                    return RunOneTimeAsync(options);
                case RunType.Mine:
                    return RunMineAsync(options);
                default:
                    throw new NotImplementedException();
            }
        }

        static async Task RunOneTimeAsync(CLIOptions options)
        {
            ProblemSolver solver = new ProblemSolver(options.LogProgress);
            using (var apiaryClient = new ApiaryHttpClient())
            {
                Console.WriteLine("Getting problem ...");
                var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
                Console.WriteLine($"Solving {problemDefinition.Id}");

                Stopwatch sw = Stopwatch.StartNew();
                string path = solver.Solve(problemDefinition);
                sw.Stop();
                Console.WriteLine($"Solved [{sw.ElapsedMilliseconds}ms]");
                Console.WriteLine("Posting solution");
                var problemSolution = new ProblemSolutionDto
                {
                    Path = path
                };
                var response = await apiaryClient.PutSolution(problemDefinition, problemSolution);

                Console.WriteLine($"Valid: {(response.Valid ? "yes" : "no")}");
                Console.WriteLine($"In time: {(response.InTime ? "yes" : "no")}");
                if (!string.IsNullOrEmpty(response.Message))
                    Console.WriteLine($"Message: {response.Message}");
            }
        }

        static async Task RunMineAsync(CLIOptions options)
        {
            var solver = new ProblemSolver(options.LogProgress);
            using (var apiaryClient = new ApiaryHttpClient(options.SaveApiCalls))
            using (var solutionService = options.SaveSolutions ? new ProblemSolutionService() : null)
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
                        Console.WriteLine($"Solving {problemDefinition.Id}");
                        Stopwatch sw = Stopwatch.StartNew();
                        string path = solver.Solve(problemDefinition);
                        sw.Stop();
                        Console.WriteLine($"Solved [{sw.ElapsedMilliseconds}ms]");
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

                        if (options.SaveSolutions)
                        {
                            var added = await solutionService?.RegisterSolutionAsync(problemDefinition.Id, path);
                            if (!added)
                                fails++;
                            else
                                fails = 0;

                            if (fails == MaxFailInRowCount)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Console.WriteLine(ex);
                        Environment.Exit(1);
                    }
                }
            }
        }

        static async Task<DateTime> ThrottleAsync(DateTime dt, int requiredPause)
        {
            var elapsedTime = (DateTime.Now - dt);
            var remainingPause = requiredPause - (int)elapsedTime.TotalMilliseconds;
            if (remainingPause > 0)
                await Task.Delay(remainingPause);

            return DateTime.Now;
        }
    }
}

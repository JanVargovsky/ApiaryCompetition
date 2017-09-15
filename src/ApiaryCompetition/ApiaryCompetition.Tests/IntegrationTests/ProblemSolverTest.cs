using ApiaryCompetition.Api;
using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using FluentAssertions;
using MoreLinq;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ApiaryCompetition.Tests.IntegrationTests
{
    [TestFixture]
    [Explicit("Integration tests")]
    [Parallelizable(ParallelScope.Fixtures)]
    public class ProblemSolverTest
    {
        ProblemSolver solver;
        ApiaryHttpClient apiaryClient;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            solver = new ProblemSolver();
            apiaryClient = new ApiaryHttpClient();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            apiaryClient.Dispose();
        }

        [Test]
        public async Task BasicResultPutTest()
        {
            var response = await apiaryClient.PutSolution(new ProblemDefinitionDto { Id = 2727 }, new ProblemSolutionDto { Path = "DLDRRU" });
            response.Message.Should().NotBe("Data (in HTTP request body) are in wrong format or does not contain resolution");
        }

        [Test]
        public async Task CompleteSolveTest()
        {
            var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
            string solution = solver.Solve(problemDefinition);
            var response = await apiaryClient.PutSolution(problemDefinition, new ProblemSolutionDto
            {
                Path = solution
            });

            response.Valid.ShouldBeEquivalentTo(true);
            response.InTime.ShouldBeEquivalentTo(true);
        }

        async Task<DateTime> ThrottleAsync(DateTime dt, int requiredPause)
        {
            var elapsedTime = (DateTime.Now - dt);
            var remainingPause = requiredPause - (int)elapsedTime.TotalMilliseconds;
            if (remainingPause > 0)
                await Task.Delay(remainingPause);

            return DateTime.Now;
        }

        [Test]
        public async Task SequentialSolveAllTest()
        {
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
                        string solution = solver.Solve(problemDefinition);
                        time = await ThrottleAsync(time, ApiaryHttpClient.RequiredDelay);
                        var response = await apiaryClient.PutSolution(problemDefinition, new ProblemSolutionDto
                        {
                            Path = solution
                        });

                        response.Valid.ShouldBeEquivalentTo(true);
                        response.InTime.ShouldBeEquivalentTo(true);

                        var added = await solutionService.RegisterSolutionAsync(problemDefinition.Id, solution);
                        if (!added)
                            fails++;
                        else
                            fails = 0;

                        if (fails == MaxFailInRowCount)
                            break;
                    }
                    catch
                    {
                    }
                }
            }
        }

        [Test, Ignore("Get and Put and requires throttle")]
        public async Task ParallelSolveAllTest()
        {
            const int threads = 4;

            var problems = new ConcurrentDictionary<int, string>();

            async Task<(int id, string solution)> RunTestAsync()
            {
                var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
                string solution = solver.Solve(problemDefinition);
                var response = await apiaryClient.PutSolution(problemDefinition, new ProblemSolutionDto
                {
                    Path = solution
                });

                response.Valid.ShouldBeEquivalentTo(true);
                response.InTime.ShouldBeEquivalentTo(true);
                return (problemDefinition.Id, solution);
            }

            var tasks = new Task<(int id, string solution)>[threads];
            while (true)
            {
                for (int i = 0; i < tasks.Length; i++)
                {
                    await Task.Delay(1500);
                    tasks[i] = Task.Run(RunTestAsync);
                }

                await Task.WhenAll(tasks);

                var atLeastOneSolutionWasAdded = tasks.Select(t => t.Result).All(t => !problems.TryAdd(t.id, t.solution));
                if (!atLeastOneSolutionWasAdded)
                    break;
            }

            using (var sw = new StreamWriter("solutions.txt"))
            {
                problems.ForEach(t => sw.WriteLine($"{t.Key}-{t.Value}"));
            }
        }
    }
}

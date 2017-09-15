using ApiaryCompetition.Api;
using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using FluentAssertions;
using NUnit.Framework;
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
    }
}

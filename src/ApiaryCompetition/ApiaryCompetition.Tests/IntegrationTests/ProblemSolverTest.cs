using ApiaryCompetition.Api;
using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using FluentAssertions;
using NUnit.Framework;
using System.Threading.Tasks;

namespace ApiaryCompetition.Tests.IntegrationTests
{
    [TestFixture]
    [Category("Integration")]
    public class ProblemSolverTest
    {
        [Test]
        public async Task CompleteSolveTest()
        {
            ProblemSolver solver = new ProblemSolver();
            using (var apiaryClient = new ApiaryHttpClient())
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
}

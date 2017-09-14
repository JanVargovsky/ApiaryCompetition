using ApiaryCompetition.Api.Dto;
using ApiaryCompetition.Solver;
using FluentAssertions;
using NUnit.Framework;

namespace ApiaryCompetition.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    class ProblemSolverTest
    {
        ProblemSolver problemSolver;
        ProblemDefinitionDto problemDefinition;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            problemSolver = new ProblemSolver();
            problemDefinition = new ProblemDefinitionDto
            {
                Map = new MapDto
                {
                    Areas = new[] { "5-R", "1-RDL", "10-DL", "2-RD", "1-UL", "1-UD", "2-RU", "1-RL", "2-UL" }
                },
                Astroants = new Point2dDto
                {
                    X = 1,
                    Y = 0,
                },
                Sugar = new Point2dDto
                {
                    X = 2,
                    Y = 1,
                }
            };
        }

        [Test]
        public void SolveTest()
        {
            var actual = problemSolver.Solve(problemDefinition);
            const string expected = "DLDRRU";

            actual.ShouldBeEquivalentTo(expected);
        }
    }
}

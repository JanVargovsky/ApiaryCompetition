using ApiaryCompetition.Solver;
using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace ApiaryCompetition.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class MapTests
    {
        Map map;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            map = new Map(new Api.Dto.MapDto
            {
                Areas = new[] {
                    "5-R", "1-RDL", "10-DL",
                    "2-RD", "1-UL", "1-UD",
                    "2-RU", "1-RL", "2-UL" }
            });
        }

        [Test]
        public void IndexTest()
        {
            var expectedAstronauts = new Cell(1, 0, 1, "RDL");
            var expectedSugar = new Cell(2, 1, 1, "UD");

            var astronauts = map[1, 0];
            var sugar = map[2, 1];

            astronauts.ShouldBeEquivalentTo(expectedAstronauts);
            sugar.ShouldBeEquivalentTo(expectedSugar);
        }

        [Test, TestCaseSource(typeof(GetNeighborsTestData))]
        public void GetNeighborsTest(int x, int y, Cell[] neighbors)
        {
            var actual = map.GetNeighbors(x, y);

            actual.ShouldBeEquivalentTo(neighbors);
        }

        public class GetNeighborsTestData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { 0, 0, new[] { new Cell(1, 0, 1, "RDL") } };
                yield return new object[] { 1, 0, new[] { new Cell(2, 0, 10, "DL"), new Cell(1, 1, 1, "UL"), new Cell(0, 0, 5, "R") } };
                yield return new object[] { 2, 0, new[] { new Cell(1, 0, 1, "RDL"), new Cell(2, 1, 1, "UD") } };
                yield return new object[] { 1, 1, new[] { new Cell(1, 0, 1, "RDL"), new Cell(0, 1, 2, "RD") } };
            }
        }

        [Test]
        [TestCase(1, 1, 0, 1, 'L')]
        [TestCase(1, 1, 2, 1, 'R')]
        [TestCase(1, 1, 1, 0, 'U')]
        [TestCase(1, 1, 1, 2, 'D')]
        public void GetDirectionTest(int x1, int y1, int x2, int y2, char dir)
        {
            var from = map[x1, y1];
            var to = map[x2, y2];

            var actual = map.GetDirection(from, to);
            actual.ShouldBeEquivalentTo(dir);
        }
    }
}

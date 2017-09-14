using ApiaryCompetition.Solver;
using FluentAssertions;
using NUnit.Framework;
using System.Collections;

namespace ApiaryCompetition.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.Self)]
    public class MapProxyTests
    {
        MapProxy map;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            map = new MapProxy(new Api.Dto.MapDto
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
            var expectedAstronauts = new Cell(1, "RDL");
            var expectedSugar = new Cell(1, "UD");

            var astronauts = map[1, 0];
            var sugar = map[2, 1];

            astronauts.ShouldBeEquivalentTo(expectedAstronauts);
            sugar.ShouldBeEquivalentTo(expectedSugar);
        }

        [Test]
        [TestCaseSource(typeof(GetNeighborsTestData))]
        public void GetNeighborsTest(int x, int y, Cell[] neighbors)
        {
            var actual = map.GetNeighbors(x, y);

            actual.ShouldBeEquivalentTo(neighbors);
        }

        public class GetNeighborsTestData : IEnumerable
        {
            public IEnumerator GetEnumerator()
            {
                yield return new object[] { 0, 0, new[] { new Cell(1, "RDL") } };
                yield return new object[] { 1, 0, new[] { new Cell(10, "DL"), new Cell(1, "UL"), new Cell(5, "R") } };
                yield return new object[] { 2, 0, new[] { new Cell(1, "RDL"), new Cell(1, "UD") } };
                yield return new object[] { 1, 1, new[] { new Cell(1, "RDL"), new Cell(2, "RD") } };
            }
        }
    }
}

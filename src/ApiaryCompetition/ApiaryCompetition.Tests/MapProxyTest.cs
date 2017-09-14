using ApiaryCompetition.Solver;
using NUnit.Framework;
using FluentAssertions;

namespace ApiaryCompetition.Tests
{
    [TestFixture]
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
    }
}

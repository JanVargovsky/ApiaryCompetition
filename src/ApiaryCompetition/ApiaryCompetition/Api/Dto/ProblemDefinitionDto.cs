namespace ApiaryCompetition.Api.Dto
{
    public class ProblemDefinitionDto
    {
        public string Id { get; set; }
        public long StartedTimestamp { get; set; }
        public MapDto Map { get; set; }
        public Point2dDto Astroants { get; set; }
        public Point2dDto Sugar { get; set; }
    }

    public class MapDto
    {
        public string[] Areas { get; set; }
    }

    public class Point2dDto
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
}

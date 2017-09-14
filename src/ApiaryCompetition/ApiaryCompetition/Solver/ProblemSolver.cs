using ApiaryCompetition.Api.Dto;
using System.Collections.Generic;

namespace ApiaryCompetition.Solver
{
    public class ProblemSolver
    {
        public string Solve(ProblemDefinitionDto problemDefinition)
        {
            MapProxy map = new MapProxy(problemDefinition.Map);
            string solution = Solve(map, problemDefinition.Astroants, problemDefinition.Sugar);

            return solution;
        }

        string Solve(MapProxy map, Point2dDto startPoint, Point2dDto endPoint)
        {
            var visited = new Queue<Cell>();
            var start = map[startPoint.X, startPoint.Y];
            var end = map[endPoint.X, endPoint.Y];

            return string.Empty;
        }
    }
}

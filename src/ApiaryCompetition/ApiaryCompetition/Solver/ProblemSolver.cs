using ApiaryCompetition.Api.Dto;
using MoreLinq;
using System.Collections.Generic;
using System.Linq;
using static System.Diagnostics.Debug;

namespace ApiaryCompetition.Solver
{
    public class ProblemSolver
    {
        public string Solve(ProblemDefinitionDto problemDefinition)
        {
            MapProxy map = new MapProxy(problemDefinition.Map);

            Cell C(Point2dDto p) => map[p.X, p.Y];

            var start = C(problemDefinition.Astroants);
            var end = C(problemDefinition.Sugar);
            string solution = Solve(map, start, end);

            return solution;
        }

        string Solve(MapProxy map, Cell start, Cell end)
        {
            var notVisited = new HashSet<Cell>();
            var distances = new Dictionary<Cell, uint>();
            var previous = new Dictionary<Cell, Cell>();
            foreach (var cell in map.AllCells())
            {
                notVisited.Add(cell);
                distances[cell] = uint.MaxValue;
                previous[cell] = null;
            }
            distances[start] = 0;

            uint Length(uint distance, Cell to) => (distance == uint.MaxValue ? 0 : distance) + to.Difficulty;

            while (notVisited.Any())
            {
                var u = notVisited.MinBy(c => distances[c]);
                notVisited.Remove(u);

                //WriteLine($"Current [{u.X}, {u.Y}]");

                foreach (var v in map.GetNeighbors(u))
                {
                    var currentDistance = distances[v];
                    var newDistance = Length(distances[u], v);
                    if (newDistance < currentDistance)
                    {
                        //WriteLine($"Update distance to [{v.X}, {v.Y}] from {currentDistance} to {newDistance}");
                        distances[v] = newDistance;
                        previous[v] = u;

                        if (v == end)
                        {
                            notVisited.Clear();
                            break;
                        }
                    }
                }
            }

            var path = new Queue<Cell>();
            var current = previous[end];
            path.Enqueue(end);
            while (current != null)
            {
                path.Enqueue(current);
                current = previous[current];
            }

            string result = new string(path.Reverse().Pairwise((from, to) => map.GetDirection(from, to)).ToArray());
            return result;
        }
    }
}

using ApiaryCompetition.Api.Dto;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;

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
            var distances = new Dictionary<Cell, double>();
            var previous = new Dictionary<Cell, Cell>();
            Init(map, start, notVisited, distances, previous);

            while (notVisited.Any())
            {
                Cell u = GetCell(notVisited, distances);

                if (notVisited.Count % 10 == 0)
                    Console.WriteLine($"Remaining nodes {notVisited.Count}/{distances.Count}");

                foreach (var v in map.GetNeighbors(u))
                {
                    var currentDistance = distances[v];
                    var newDistance = distances[u] + v.Difficulty;
                    if (newDistance < currentDistance)
                    {
                        distances[v] = newDistance;
                        previous[v] = u;

                        //if (v.Equals(end))
                        //{
                        //    notVisited.Clear();
                        //    break;
                        //}
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

        Cell GetCell(HashSet<Cell> notVisited, Dictionary<Cell, double> distances)
        {
            // https://www.nuget.org/packages?q=priority+queue
            // https://www.nuget.org/packages/OptimizedPriorityQueue/
            // priority queue!!!
            var u = notVisited.MinBy(c => distances[c]);
            notVisited.Remove(u);
            return u;
        }

        private static void Init(MapProxy map, Cell start, HashSet<Cell> notVisited, Dictionary<Cell, double> distances, Dictionary<Cell, Cell> previous)
        {
            foreach (var cell in map.AllCells())
            {
                notVisited.Add(cell);
                distances[cell] = double.PositiveInfinity;
                previous[cell] = null;
            }
            distances[start] = 0;
        }
    }
}

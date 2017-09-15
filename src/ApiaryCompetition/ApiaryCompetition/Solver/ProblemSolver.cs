using ApiaryCompetition.Api.Dto;
using MoreLinq;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var queue = new FastPriorityQueue<Cell>(map.TotalMapSize);
            var distances = new Dictionary<Cell, float>();
            var previous = new Dictionary<Cell, Cell>();
            foreach (var cell in map.AllCells())
            {
                if (!cell.Equals(start))
                {
                    distances[cell] = float.PositiveInfinity;
                    previous[cell] = null;
                    queue.Enqueue(cell, float.PositiveInfinity);
                }
            }
            queue.Enqueue(start, 0);
            distances[start] = 0;

            int oneTenth = map.TotalMapSize > 10 ? map.TotalMapSize / 10 : 1;

            while (queue.Any())
            {
                var u = queue.Dequeue();

                //Debug.WriteLine($"Current node [{u.X}, {u.Y}]");

                if (queue.Count % oneTenth == 0)
                    Console.WriteLine($"Remaining nodes {queue.Count}/{map.TotalMapSize}");

                foreach (var v in map.GetNeighbors(u))
                {
                    if (!queue.Contains(v))
                        continue;

                    var currentDistance = distances[v];
                    var newDistance = distances[u] + v.Difficulty;
                    if (newDistance < currentDistance)
                    {
                        //Debug.WriteLine($"Update distance to [{v.X}, {v.Y}] from {currentDistance} to {newDistance}");

                        queue.UpdatePriority(v, newDistance);
                        distances[v] = newDistance;
                        previous[v] = u;

                        if (v.Equals(end))
                        {
                            queue.Clear();
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
                previous.TryGetValue(current, out current);
            }

            string result = new string(path.Reverse().Pairwise((from, to) => map.GetDirection(from, to)).ToArray());
            return result;
        }
    }
}

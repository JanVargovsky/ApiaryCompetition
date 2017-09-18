using ApiaryCompetition.Api.Dto;
using MoreLinq;
using Priority_Queue;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiaryCompetition.Solver
{
    public class ProblemSolver
    {
        readonly bool reportProgress;

        public ProblemSolver(bool reportProgress = false)
        {
            this.reportProgress = reportProgress;
        }

        public string Solve(ProblemDefinitionDto problemDefinition)
        {
            Map map = new Map(problemDefinition.Map);

            Cell C(Point2dDto p) => map[p.X, p.Y];

            var start = C(problemDefinition.Astroants);
            var end = C(problemDefinition.Sugar);
            string solution = Solve(map, start, end);

            return solution;
        }

        /// <summary>
        /// Finds a shortest path from the <paramref name="start"/> to the <paramref name="end"/>.
        /// It uses a Dijkstra's algorithm with a priority queue.
        /// </summary>
        string Solve(Map map, Cell start, Cell end)
        {
            var vertices = new FastPriorityQueue<Cell>(map.TotalMapSize);
            var distances = new Dictionary<Cell, float>();
            var previous = new Dictionary<Cell, Cell>();
            foreach (var cell in map.AllCells())
            {
                if (!cell.Equals(start))
                {
                    distances[cell] = float.PositiveInfinity;
                    previous[cell] = null;
                    vertices.Enqueue(cell, float.PositiveInfinity);
                }
            }
            vertices.Enqueue(start, 0);
            distances[start] = 0;

            int oneTenth = map.TotalMapSize > 10 ? map.TotalMapSize / 10 : 1;

            while (vertices.Any())
            {
                var u = vertices.Dequeue();

                if (reportProgress && vertices.Count % oneTenth == 0)
                    Console.WriteLine($"Remaining nodes {vertices.Count}/{map.TotalMapSize}");

                foreach (var v in map.GetNeighbors(u))
                {
                    if (!vertices.Contains(v))
                        continue;

                    var currentDistance = distances[v];
                    var newDistance = distances[u] + v.Difficulty;
                    if (newDistance < currentDistance)
                    {
                        vertices.UpdatePriority(v, newDistance);
                        distances[v] = newDistance;
                        previous[v] = u;

                        if (v.Equals(end))
                        {
                            vertices.Clear();
                            break;
                        }
                    }
                }
            }

            return GeneratePath(end, previous, map);
        }

        string GeneratePath(Cell end, Dictionary<Cell, Cell> path, Map map)
        {
            var result = new Queue<Cell>();
            var current = path[end];
            result.Enqueue(end);
            while (current != null)
            {
                result.Enqueue(current);
                path.TryGetValue(current, out current);
            }

            return new string(result.Reverse().Pairwise((from, to) => map.GetDirection(from, to)).ToArray());
        }
    }
}

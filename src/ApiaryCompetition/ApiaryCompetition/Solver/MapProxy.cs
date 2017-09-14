using ApiaryCompetition.Api.Dto;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ApiaryCompetition.Solver
{
    public class MapProxy
    {
        readonly MapDto map;
        readonly int mapSize;

        public Cell this[int x, int y] => ParseCell(map.Areas[y * mapSize + x]);

        public MapProxy(MapDto map)
        {
            this.map = map;
            mapSize = (int)Math.Sqrt(map.Areas.Length);
        }

        Cell ParseCell(string cell)
        {
            int separatorIndex = cell.IndexOf('-');
            string difficulty = cell.Substring(0, separatorIndex);
            string paths = cell.Substring(separatorIndex + 1);

            return new Cell(int.Parse(difficulty), paths);
        }

        (int x, int y) GetDirectionOffset(char direction)
        {
            if (direction == 'L') return (-1, 0);
            else if (direction == 'R') return (1, 0);
            else if (direction == 'U') return (0, -1);
            else if (direction == 'D') return (0, 1);
            else throw new ArgumentException(nameof(direction));
        }

        public IEnumerable<Cell> GetNeighbors(int x, int y)
        {
            var current = this[x, y];
            foreach (var direction in current.Paths)
            {
                (int offsetX, int offsetY) = GetDirectionOffset(direction);
                yield return this[x + offsetX, y + offsetY];
            }
        }
    }

    public class Cell
    {
        public int Difficulty { get; }
        public string Paths { get; }

        public Cell(int difficulty, string paths)
        {
            Difficulty = difficulty;
            Paths = paths;
        }
    }
}

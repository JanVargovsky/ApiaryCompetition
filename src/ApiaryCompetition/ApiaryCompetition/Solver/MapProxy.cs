using ApiaryCompetition.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ApiaryCompetition.Solver
{
    public class MapProxy
    {
        static readonly Dictionary<char, (int x, int y)> directionToPointMapping;
        static readonly Dictionary<(int x, int y), char> pointToDirectionMapping;

        static MapProxy()
        {
            directionToPointMapping = new Dictionary<char, (int x, int y)>();
            pointToDirectionMapping = new Dictionary<(int x, int y), char>();
            DictionaryExtensions.Add(directionToPointMapping, pointToDirectionMapping, 'L', (-1, 0));
            DictionaryExtensions.Add(directionToPointMapping, pointToDirectionMapping, 'R', (1, 0));
            DictionaryExtensions.Add(directionToPointMapping, pointToDirectionMapping, 'U', (0, -1));
            DictionaryExtensions.Add(directionToPointMapping, pointToDirectionMapping, 'D', (0, 1));
        }

        readonly MapDto map;
        public int MapSize { get; }

        public Cell this[int x, int y] => ParseCell(x, y);

        public MapProxy(MapDto map)
        {
            this.map = map;
            MapSize = (int)Math.Sqrt(map.Areas.Length);
        }

        Cell ParseCell(int x, int y)
        {
            string cell = map.Areas[y * MapSize + x];
            int separatorIndex = cell.IndexOf('-');
            string difficulty = cell.Substring(0, separatorIndex);
            string paths = cell.Substring(separatorIndex + 1);

            return new Cell(x, y, uint.Parse(difficulty), paths);
        }

        (int x, int y) GetDirectionOffset(char direction) => directionToPointMapping[direction];

        public char GetDirection(Cell from, Cell to)
        {
            var dirX = to.X - from.X;
            var dirY = to.Y - from.Y;
            return pointToDirectionMapping[(dirX, dirY)];
        }

        public IEnumerable<Cell> GetNeighbors(Cell cell)
        {
            foreach (var direction in cell.Paths)
            {
                (int offsetX, int offsetY) = GetDirectionOffset(direction);
                yield return this[cell.X + offsetX, cell.Y + offsetY];
            }
        }

        public IEnumerable<Cell> GetNeighbors(int x, int y) => GetNeighbors(this[x, y]);

        public IEnumerable<Cell> AllCells() => Enumerable.Range(0, MapSize)
            .SelectMany(x => Enumerable.Range(0, MapSize)
                .Select(y => this[x, y]));
    }
}

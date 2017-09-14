using ApiaryCompetition.Api.Dto;
using System;

namespace ApiaryCompetition.Solver
{
    class MapProxy
    {
        readonly MapDto map;
        readonly int mapSize;

        public Cell this[int x, int y] => ParseCell(map.Areas[x * mapSize + y]);

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

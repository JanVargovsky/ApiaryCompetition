using Priority_Queue;
using System.Collections.Generic;
using System.Diagnostics;

namespace ApiaryCompetition.Solver
{
    [DebuggerDisplay("X={X}, Y={Y}, {Difficulty}-{Paths,nq}")]
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public double Difficulty { get; }
        public string Paths { get; }

        public Cell(int x, int y, double difficulty, string paths)
        {
            X = x;
            Y = y;
            Difficulty = difficulty;
            Paths = paths;
        }

        public override bool Equals(object obj) =>
            obj is Cell cell && X == cell.X && Y == cell.Y;

        public override int GetHashCode() => X << 16 + Y;

        public override string ToString()
        {
            return $"X={X}, Y={Y}, {Difficulty}-{Paths}";
        }
    }
}

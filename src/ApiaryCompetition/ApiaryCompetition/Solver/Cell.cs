using System.Collections.Generic;
using System.Diagnostics;

namespace ApiaryCompetition.Solver
{
    [DebuggerDisplay("X={X}, Y={Y}, {Difficulty}-{Paths,nq}")]
    public class Cell
    {
        public int X { get; }
        public int Y { get; }
        public uint Difficulty { get; }
        public string Paths { get; }

        public Cell(int x, int y, uint difficulty, string paths)
        {
            X = x;
            Y = y;
            Difficulty = difficulty;
            Paths = paths;
        }

        public override bool Equals(object obj)
        {
            var cell = obj as Cell;
            return cell != null &&
                   X == cell.X &&
                   Y == cell.Y &&
                   Difficulty == cell.Difficulty &&
                   Paths == cell.Paths;
        }

        public override int GetHashCode()
        {
            var hashCode = -404474542;
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Difficulty.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Paths);
            return hashCode;
        }

        public override string ToString()
        {
            return $"X={X}, Y={Y}, {Difficulty}-{Paths}";
        }
    }
}

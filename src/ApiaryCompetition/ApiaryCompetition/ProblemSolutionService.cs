using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ApiaryCompetition
{
    public class ProblemSolutionService : IDisposable
    {
        readonly Dictionary<int, string> problems;
        readonly StreamWriter stream;

        public ProblemSolutionService()
        {
            problems = new Dictionary<int, string>();
            var fileStream = File.Open("solutions.csv", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            LoadExistingProblems(fileStream);
            stream = new StreamWriter(fileStream);
        }

        void LoadExistingProblems(Stream stream)
        {
            string line;
            var sr = new StreamReader(stream);
            while ((line = sr.ReadLine()) != null)
            {
                var tokens = line.Split(',');
                int problemId = int.Parse(tokens[0]);
                string solution = tokens[1];

                problems[problemId] = solution;
            }
        }

        public void Dispose()
        {
            stream.Dispose();
        }

        public async Task<bool> RegisterSolutionAsync(int problemId, string solution)
        {
            if (!problems.TryAdd(problemId, solution))
                return false;

            await stream.WriteLineAsync($"{problemId},{solution}");
            await stream.FlushAsync();
            return true;
        }
    }
}

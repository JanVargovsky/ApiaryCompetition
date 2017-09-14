using ApiaryCompetition.Api;
using ApiaryCompetition.Solver;
using System.Threading.Tasks;

namespace ApiaryCompetition
{
    class Program
    {
        static async Task Main(string[] args)
        {
            ProblemSolver solver = new ProblemSolver();
            using (var apiaryClient = new ApiaryHttpClient())
            {
                var problemDefinition = await apiaryClient.GetProblemDefinitionAsync();
                string solution = solver.Solve(problemDefinition);

                // TODO: put solution
            }
        }
    }
}

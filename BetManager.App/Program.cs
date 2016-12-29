using BetManager.App.Solving;
using System;

namespace BetManager.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var solver = new DXSolver();
            var ksi = 0.0065 / 3.5;
            DateTime dateActual = new DateTime(2016, 12, 28);
            int[] tournaments = new int[] { 2, 112, 125, 142, 285, 673, 930, 1649, 2520, 2768, 3625, 3660, 3708, 3799, 3801, 3962, 5126, 5502, 7489, 9515, 9516, 9517, 9958, 11959, 11960, 13376, 13768, 37884, 37885, 37886, 54634, 55756, 56202, 56203, 56206, 56207, 56208, 56209, 56211, 56215, 57047, 66 };
            // int[] tournaments = new int[] { 2 };
            try
            {
                bool doParalel = true;
                if (doParalel)
                {
                    var options = new System.Threading.Tasks.ParallelOptions { MaxDegreeOfParallelism = 2 };
                    System.Threading.Tasks.Parallel.ForEach(tournaments, options, t => solver.Solve(ksi, t, dateActual));
                }
                else
                {
                    foreach (var t in tournaments)
                    {
                        solver.Solve(ksi, t, dateActual);
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText("errz.txt", string.Format("Message: {0}\nTrace: {1}\nInnerMessage: {2}", ex.Message, ex.StackTrace, ex.InnerException?.Message));
                throw;
            }
        }
    }
}

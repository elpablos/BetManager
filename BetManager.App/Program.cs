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
            var idtournament = 54;
            DateTime dateActual = new DateTime(2016, 12, 26);


            int[] tournaments = new int[] { 1, 2, 3, 4, 16, 19, 33, 34, 36, 38, 40, 41, 42, 52, 54, 55, 56, 57, 62, 64, 66, 68, 72, 84, 86, 101, 127, 144, 150, 161, 219, 280, 681, 877, 904, 950, 1130, 1377, 3608, 3625, 3640, 3657, 3708, 3799, 3800, 3801, 3802, 3830, 3962, 4051, 5071, 5126, 7002, 16755, 23303, 23304, 36800, 37412, 37884, 37885, 37886, 37929, 37930, 38612, 39256, 39257, 39258, 39259, 47943 };
            System.Threading.Tasks.Parallel.ForEach(tournaments, t => solver.Solve(ksi, idtournament, dateActual));

            // solver.Solve(ksi, idtournament, dateActual);
        }
    }
}

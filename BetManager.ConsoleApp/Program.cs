using BetManager.Tester.Solving;
using System;
using System.Diagnostics;

namespace BetManager.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string sourcePath = "data.csv";
            DateTime dateActual = DateTime.Parse("2015-03-01"); // 01.03.2015

            if (args.Length > 0)
            {
                sourcePath = args[1];
            }
            if (args.Length > 1)
            {
                dateActual = DateTime.Parse(args[2]);
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            var solver = new RDXSolver();
            solver.HasSaveToDb = false;
            solver.SourcePath = sourcePath;
            // double ksi = 2.1;// 0.0065 / 3.5;
            // solver.Solve(ksi, -1, dateActual);

            double[] ksis = new double[] { 0.0,  1.0, 1.5, 1.9, 2.0, 2.1, 2.2, 2.3, 2.4, 2.5, 3.0 };

            var options = new System.Threading.Tasks.ParallelOptions(); // { MaxDegreeOfParallelism = 2 };
            System.Threading.Tasks.Parallel.ForEach(ksis, options, ksi => solver.Solve(ksi, -1, dateActual));

            watch.Stop();

            Console.WriteLine("LastElapsed;{0}\n", watch.Elapsed);
        }
    }
}

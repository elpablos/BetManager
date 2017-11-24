using BetManager.Tester.Solving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.ConsoleApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string sourcePath = "premier-league.csv";
            DateTime dateActual = DateTime.Parse("2017-11-20");

            if (args.Length > 0)
            {
                sourcePath = args[1];
            }
            if (args.Length > 1)
            {
                dateActual = DateTime.Parse(args[2]);
            }

            var solver = new RDXSolver();
            solver.HasSaveToDb = false;
            solver.SourcePath = sourcePath;
            double ksi = 0.0065 / 3.5;
          
            solver.Solve(ksi, -1, dateActual);
        }
    }
}

using BetManager.App.Solving;
using System;

namespace BetManager.App
{
    class Program
    {
        static void Main(string[] args)
        {
            //var solver = new DXSolver();
            //var ksi = 0.0065 / 3.5;
            //DateTime dateActual = DateTime.Now; // new DateTime(2017, 03, 13);
            //// int[] tournaments = new int[] { 9958, 11960, 56206, 37884, 3801, 7489, 37886, 2520, 285, 56209 };
            //// 49 - CR 1.LIGA (fotbal)
            //int[] tournaments = new int[] { 49 };// 84, 86, 127, 144, 150, 1130 }; // 150,
            //try
            //{
            //    bool doParalel = false;
            //    if (doParalel)
            //    {
            //        var options = new System.Threading.Tasks.ParallelOptions(); // { MaxDegreeOfParallelism = 2 };
            //        System.Threading.Tasks.Parallel.ForEach(tournaments, options, t => solver.Solve(ksi, t, dateActual));
            //    }
            //    else
            //    {
            //        foreach (var t in tournaments)
            //        {
            //            solver.Solve(ksi, t, dateActual);
            //        }
            //    }
            //}

            DateTime date = DateTime.Parse("2017-04-14");
            var solver = new DXSolver();
            solver.HasSaveToDb = true;
            try
            {
                while (date > DateTime.Parse("2016-08-12"))
                {
                    solver.Solve(0.0065 / 3.5, 1, 11733, 10356, date);
                    date = date.AddDays(-7);
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

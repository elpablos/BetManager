using BetManager.Tester.Solving;
using System;
using System.IO;

namespace BetManager.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            // ID_Tournament: 1, ID_Season: 11733, ID_LastSeason: 10356, prvni zapas: 2016-08-13 11:30:00.000

            DateTime date = DateTime.Parse("2017-04-14");
            var solver = new RDXSolver();
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
                File.AppendAllText(@"C:\Develop\BitBucket\betmanager\BetManager.Tester\bin\logs\errz.txt", 
                    string.Format("Message: {0}\nTrace: {1}\nInnerMessage: {2}", 
                    ex.Message, ex.StackTrace, ex.InnerException?.Message));
                throw;
            }
        }

    }
}

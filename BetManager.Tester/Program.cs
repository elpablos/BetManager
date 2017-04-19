using BetManager.Tester.Solving;
using Microsoft.Win32;
using System;
using System.IO;

namespace BetManager.Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            DateTime date = DateTime.Parse("2017-03-24");
            var solver = new RDXSolver();
            solver.HasSaveToDb = false;
            try
            {
                while (date > DateTime.Parse("2015-12-31"))
                {
                    solver.Solve(0.0065 / 3.5, 49, date);
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

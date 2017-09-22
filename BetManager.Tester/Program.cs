using BetManager.Tester.Solving;
using System;
using System.IO;

namespace BetManager.Tester
{
    public class TestItem
    {
        public int ID_Tournament { get; set; }
        public int ID_Season { get; set; }
        public int ID_LastSeason { get; set; }

        public TestItem(int t, int s, int l)
        {
            ID_Tournament = t;
            ID_Season = s;
            ID_LastSeason = l;
        }
    }

    class Program
    {
        static TestItem[] Items = new TestItem[]
        {
            new TestItem( 1                    ,    11733                ,    10356                ),
            new TestItem( 2                    ,    11784                ,    10358                ),
            new TestItem( 4                    ,    11648                ,    10373                ),
            new TestItem( 36                   ,    11906                ,    10495                ),
            new TestItem( 42                   ,    11818                ,    10419                ),
            new TestItem( 49                   ,    11779                ,    10406                ),
        };

        static void Main(string[] args)
        {
            // ID_Tournament: 1, ID_Season: 11733, ID_LastSeason: 10356, prvni zapas: 2016-08-13 11:30:00.000

            DateTime date = DateTime.Parse("2017-04-21");
            var solver = new RDXSolver();
            solver.HasSaveToDb = true;
            try
            {
                while (date > DateTime.Parse("2016-08-12"))
                {
                    foreach (var item in Items)
                    {
                        solver.Solve(0.0065 / 3.5, item.ID_Tournament, item.ID_Season, item.ID_LastSeason, date);
                    }

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

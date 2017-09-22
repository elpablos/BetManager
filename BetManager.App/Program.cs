using BetManager.App.Solving;
using System;

namespace BetManager.App
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
           /*
            // England - Premier League, 16/17, 15/16 ('2016-08-13' - '2017-05-21')
            new TestItem( 1                    ,    11733                ,    10356                ),
            // Czech Republic - 1. Liga, 16/17, 15/16 ('2016-07-29' - '2017-05-27')
            new TestItem( 49                   ,    11779                ,    10406                ),
               Germany - Bundesliga, 16/17, 15/16 ('2015-08-14' - '2017-05-29')
            new TestItem( 42                   ,    11818                ,    10419                ),
            // France - Ligue 1, 16/17, 15/16 ('2015-08-07' - '2017-05-28')
            new TestItem( 4                    ,    11648                ,    10373                ),
            // Spain - Primera División, 16/17, 15/16  ('2015-08-21' - '2017-05-21')
            new TestItem( 36                   ,    11906                ,    10495                ),
            // Italy - Serie A, 16/17, 15/16  ('2015-08-22' - '2017-05-28')
            new TestItem( 33                   ,    11966                ,    10596                ),
            // Slovakia   Superliga, 16/17, 15/16 ('2015-07-18' - '2017-05-27')
            new TestItem( 92                   ,    11738                ,    10403                ),
            // Poland - Ekstraklasa, 16/17, 15/16 ('2015-07-17' - '2017-06-04')
            new TestItem( 64                   ,    11734                ,    10361                ),
            //  Romania   Liga I, 16/17, 15/16 ('2015-07-10' - '2017-06-15')
            new TestItem( 219                   ,    11884                ,    10408                ),
            */
            // England - Premier League, 17/18, 16/17
            new TestItem( 1                    ,    13380                ,    11733                ),
            // Czech Republic - 1. Liga, 17/18, 16/17
            new TestItem( 49                   ,    13444                ,    11779                ),
            // Germany - Bundesliga, 17/18, 16/17
            new TestItem( 42                   ,    13477                ,    11818                ),
            // France - Ligue 1, 17/18, 16/17 (dlouho trva)
            new TestItem( 4                    ,    13384                ,    11648                ),
            // Spain - Primera División, 17/18, 16/17
            new TestItem( 36                   ,    13662                ,    11906                ),
            // Italy - Serie A, 17/18, 16/17
            new TestItem( 33                   ,    13768                ,    11966                ),
            // Slovakia   Superliga, 17/18, 16/17
            new TestItem( 92                   ,    13439                ,    11738                ),
            // Poland - Ekstraklasa, 17/18, 16/17
            new TestItem( 64                   ,    13350                ,    11734                ),
            //  Romania   Liga I, 17/18, 16/17 219
            new TestItem( 219                   ,    13535                ,    11884                ),

            // deprecated 
            //new TestItem( 2                    ,    11784                ,    10358                ),
            
        };

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

            DateTime date = DateTime.Parse("2017-09-22");
            var solver = new DXSolver();
            solver.HasSaveToDb = true;
            try
            {
                while (date >= DateTime.Parse("2017-09-22"))
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
                System.IO.File.AppendAllText("errz.txt", string.Format("Message: {0}\nTrace: {1}\nInnerMessage: {2}",
                    ex.Message, ex.StackTrace, ex.InnerException?.Message));
                throw;
            }
        }
    }
}

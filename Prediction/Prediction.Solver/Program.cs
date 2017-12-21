using Prediction.Core.Extensions;
using Prediction.Core.Services;
using Prediction.Core.Workers;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Prediction.Solver
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = "input.xml";
            if (args.Length > 0)
            {
                inputFile = args[0];
            }

            var input = XmlExtensions.DeserializeFromFile<Input>(inputFile);

            IGameMatchService matchService = new GameMatchService();
            IGamePredictionService predictionService = new GamePredictionService();
            IDataInputService dataInputService = new DataInputService();

            try
            {
                matchService.Prepare();
                predictionService.Prepare();

                var dataz = dataInputService.ReadFile(input.Path);
                matchService.Bulk(dataz);
            }
            catch (Exception ex)
            {
                if (!(ex.Message.Contains("SQL logic error") && ex.Message.Contains("already exists")))
                {
                    throw ex;
                }
            }

            var data = matchService.GetAll();

            var worker = new DixonColesWorker(predictionService);
            worker.CanSaveToDb = input.CanSaveToDb;
            worker.KsiStart = input.KsiStart;
            worker.PropLength = input.PropLength;
            worker.EqualityTolerance = input.EqualityTolerance;
            worker.TimeLimit = input.TimeLimit;

            Stopwatch watch = new Stopwatch();

            watch.Start();

            worker.Solve(input.Inputs[0].Ksi, data, input.Inputs[0].DateStart, input.Inputs[0].Type);

            //var options = new ParallelOptions();
            //Parallel.ForEach(input.Inputs, options, x => worker.Solve(x.Ksi, data, x.DateStart, x.Type));

            watch.Stop();

            Console.WriteLine("Complete - Elapsed;{0}\n", watch.Elapsed);
        }
    }
}

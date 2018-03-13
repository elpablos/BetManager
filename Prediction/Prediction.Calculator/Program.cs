using Prediction.Core.Extensions;
using Prediction.Core.Services;
using Prediction.Core.Workers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prediction.Calculator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            string inputFile = "calc-input.xml";
            if (args.Length > 0)
            {
                inputFile = args[0];
            }

            var input = XmlExtensions.DeserializeFromFile<Input>(inputFile);

            IGameMatchService matchService = new GameMatchService();
            IGamePredictionService predictionService = new GamePredictionService();
            IDataInputService dataInputService = new DataInputService();

            var data = matchService.GetAll();

            Stopwatch watch = new Stopwatch();

            Console.WriteLine("Start calculating...");

            watch.Start();

            DixonColesCalculator calc = new DixonColesCalculator(predictionService);
            calc.KsiStart = input.KsiStart;
            calc.Type = input.Type;
            calc.PropLength = input.PropLength;
            calc.Ksi = input.Ksi;
            var result = calc.Calculate(data);
            watch.Stop();

            string outputFile = inputFile.Replace(".xml", ".csv").Replace("input", "output");
            string resultFile = inputFile.Replace(".xml", "-result.txt");
            string resultText = string.Empty;

            System.IO.File.WriteAllText(outputFile, calc.Csv);

            resultText = string.Format("Result: {0}", result) + Environment.NewLine;
            resultText += string.Format("Complete - Elapsed;{0}\n", watch.Elapsed) + Environment.NewLine;

            Console.WriteLine(resultText);
            System.IO.File.WriteAllText(resultFile, resultText);
        }
    }
}

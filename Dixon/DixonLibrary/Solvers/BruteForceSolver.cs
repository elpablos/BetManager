using Dixon.Library.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Library.Solvers
{
    public class BruteForceSolver : IDixonColesSolver
    {
        private readonly IDixonManager _DixonManager;

        private readonly Random _Random;
        private readonly RandomNumberGenerator _RandomNumberGenerator;

        /// <summary>
        /// Počáteční velikost populace
        /// </summary>
        public int PopulationSize { get; set; } = 1000000;

        public List<DxGeneticGen> CurrentPopulation { get; private set; }

        public List<DxGeneticGen> NextPopulation { get; private set; }

        public BruteForceSolver(IDixonManager dixonManager)
        {
            _DixonManager = dixonManager;
            CurrentPopulation = new List<DxGeneticGen>();
            NextPopulation = new List<DxGeneticGen>();
            _Random = new Random();
            _RandomNumberGenerator = new RandomNumberGenerator(_Random);
        }

        public double Solve(DateTime actualDate)
        {
            // 1) deklarace počáteční populace a další generace
            for (int i = 0; i < PopulationSize; i++)
            {
                CurrentPopulation.Add(new DxGeneticGen(_RandomNumberGenerator, _Random, _DixonManager.Teams.Count));
            }

            // výchozí výpočet vah
            foreach (var gen in CurrentPopulation)
            {
                gen.CalculateFitness(_DixonManager, actualDate);
            }

            var nans = CurrentPopulation.Where(x => double.IsNaN(x.Fitness)).Count();

            //foreach (var nn in nans)
            //{
            //    nn.CalculateFitness(_DixonManager, actualDate);
            //}

            var best = CurrentPopulation.Where(x => !double.IsNaN(x.Fitness)).OrderByDescending(x => x.Fitness).FirstOrDefault();
            best.CalculateFitness(_DixonManager, actualDate);

            return best.Fitness;
        }
    }

    public class RandomNumberGenerator
    {
        private readonly Random _random;
        public RandomNumberGenerator(Random random)
        {
            _random = random;
        }

        public int[] GenNumbers(int n, int sum)
        {
            int[] nums = new int[n];
            int upperbound = (int)(Math.Round(sum * 1.0 / n));
            int offset = (int)(Math.Round(0.5 * upperbound));

            int cursum = 0;

            for (int i = 0; i < n; i++)
            {
                int rand = _random.Next(upperbound) + offset;
                if (cursum + rand > sum || i == n - 1)
                {
                    rand = sum - cursum;
                }
                cursum += rand;
                nums[i] = rand;
                if (cursum == sum)
                {
                    break;
                }
            }
            return nums;
        }

        public double[] GenDoubles(int n, int sum)
        {
            var ints = new int[] { 0 };

            while (ints.Contains(0))
            {
                ints = GenNumbers(n, sum * 100);
            }

            var doubles = new double[n];

            for (int i = 0; i < ints.Length; i++)
            {
                doubles[i] = ints[i] / 100.0;
            }

            return doubles;
        }
    }
}

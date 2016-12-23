using Dixon.Library.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Library.Solvers
{
    public class DxGeneticSolver : IDixonColesSolver
    {
        private readonly IDixonManager _DixonManager;
        
        private readonly Random _Random;

        /// <summary>
        /// Počáteční velikost populace
        /// </summary>
        public int PopulationSize { get; set; } = 20;

        /// <summary>
        /// Počet iterací/křížení
        /// </summary>
        public int GenerationLength { get; set; } = 1000;

        /// <summary>
        /// Ppdst křížení
        /// </summary>
        public double Crossover { get; set; } = 0.8;

        /// <summary>
        /// Ppdt mutace
        /// </summary>
        public double Mutation { get; set; } = 0.2;

        public List<DxGeneticGen> CurrentPopulation { get; private set; }

        public List<DxGeneticGen> NextPopulation { get; private set; }

        public DxGeneticSolver(IDixonManager dixonManager)
        {
            _DixonManager = dixonManager;
            CurrentPopulation = new List<DxGeneticGen>();
            NextPopulation = new List<DxGeneticGen>();
            _Random = new Random();
        }

        public double Solve(DateTime actualDate)
        {
            // 1) deklarace počáteční populace a další generace
            for (int i = 0; i < PopulationSize; i++)
            {
                CurrentPopulation.Add(new DxGeneticGen(_Random,  _DixonManager.Teams.Count));
            }

            // 2) iterujeme nad generacemi
            for (int i = 0; i < GenerationLength; i++)
            {
                // výchozí výpočet vah
                foreach (var gen in CurrentPopulation)
                {
                    gen.CalculateFitness(_DixonManager, actualDate);
                }

                for (int j = 0; j < PopulationSize; j++)
                {
                    DxGeneticGen gen = null;
                    DxGeneticGen left = CurrentPopulation.ElementAt(_Random.Next(0, PopulationSize - 1));
                    DxGeneticGen right = CurrentPopulation.ElementAt(_Random.Next(0, PopulationSize - 1));

                    DxGeneticGen nleft = CurrentPopulation.ElementAt(_Random.Next(0, PopulationSize - 1));
                    DxGeneticGen nright = CurrentPopulation.ElementAt(_Random.Next(0, PopulationSize - 1));

                    // 3) křížení nebo výběr
                    if (_Random.NextDouble() < Crossover)
                    {
                        // krizeni
                        var first = left.Fitness > right.Fitness ? left : right;
                        var second = nleft.Fitness > nright.Fitness ? left : right;
                        gen = DxGeneticGen.Cross(first, second, _Random);
                    }
                    else
                    {
                        // vyber
                        gen = left.Fitness > right.Fitness ? left : right;
                    }

                    // 4) mutace
                    if (_Random.NextDouble() < Mutation)
                    {
                        // attack & def
                        for (int x = 0; x < gen.Attack.Length; x++)
                        {
                            if (_Random.NextDouble() < 0.5)
                            {
                                gen.RandomAttack(_Random, x);
                            }

                            if (_Random.NextDouble() < 0.5)
                            {
                                gen.RandomDefence(_Random, x);
                            }
                        }

                        // rho
                        if (_Random.NextDouble() < 0.5)
                        {
                            gen.RandomRho(_Random);
                        }

                        // game
                        if (_Random.NextDouble() < 0.5)
                        {
                            gen.RandomGama(_Random);
                        }
                    }

                    NextPopulation.Add(gen);
                }

                // priprava na dalsi iteraci
                CurrentPopulation = NextPopulation;
                NextPopulation = new List<DxGeneticGen>();
            }

            var best = CurrentPopulation.OrderBy(x => x.Fitness).FirstOrDefault();

            return best.Fitness;
        }
    }
}

using Dixon.Library.Managers;
using System;

namespace Dixon.Library.Solvers
{
    public class DxGeneticGen
    {
        private const double MIN = 0.01;
        private const double MAX = 3.00;

        private const double MINRHO = -0.4;
        private const double MAXRHO = 0.5;

        private const double MINGAMA = 1.00;
        private const double MAXGAMA = 2.00;


        public int Id { get; set; }
        public double[] Attack { get; set; }
        public double[] Defence { get; set; }
        public double Rho { get; set; }
        public double Gama { get; set; }

        public double Fitness { get; set; } = double.MinValue;

        private DxGeneticGen(int length)
        {
            Attack = new double[length];
            Defence = new double[length];
        }

        public DxGeneticGen(Random random, int length)
        {
            Attack = new double[length];
            Defence = new double[length];
            for (int i = 0; i < length; i++)
            {
                RandomAttack(random, i);
                RandomDefence(random, i);
            }

            RandomRho(random);
            RandomGama(random);
        }

        public DxGeneticGen(RandomNumberGenerator generator, Random random, int length)
        {
            Attack = generator.GenDoubles(length, length);
            Defence = generator.GenDoubles(length, length);

            RandomRho(random);
            RandomGama(random);
        }

        public void RandomAttack(Random random, int i)
        {
            Attack[i] = random.NextDouble() * random.NextDouble() * (MAX - MIN) + MIN;
        }

        public void RandomDefence(Random random, int i)
        {
            Defence[i] = random.NextDouble() * random.NextDouble() * (MAX - MIN) + MIN;
        }

        public void RandomRho(Random random)
        {
            Rho = random.NextDouble() * random.NextDouble() * (MAXRHO - MINRHO) + MINRHO;
        }

        public void RandomGama(Random random)
        {
            Gama = random.NextDouble() * random.NextDouble() * (MAXGAMA - MINGAMA) + MINGAMA;
        }

        public static DxGeneticGen Cross(DxGeneticGen first, DxGeneticGen second, Random random)
        {
            var crossProduct = new DxGeneticGen(first.Attack.Length);

            // attack & def
            for (int i = 0; i < first.Attack.Length; i++)
            {
                crossProduct.Attack[i] = (random.NextDouble() < 0.5) ? first.Attack[i] : second.Attack[i];
                crossProduct.Defence[i] = (random.NextDouble() < 0.5) ? first.Defence[i] : second.Defence[i];
            }

            // rho & gama
            crossProduct.Rho = (random.NextDouble() < 0.5) ? first.Rho : second.Rho;
            crossProduct.Gama = (random.NextDouble() < 0.5) ? first.Gama : second.Gama;

            return crossProduct;
        }

        public void CalculateFitness(IDixonManager manager, DateTime dateActual)
        {
            manager.Rho = Rho;
            manager.Gama = Gama;

            for (int i = 0; i < manager.Teams.Count; i++)
            {
                var team = manager.Teams[i];
                team.HomeAttack = Attack[i];
                team.AwayAttack = Defence[i];
            }

            Fitness = manager.Sum(dateActual);
        }
    }
}

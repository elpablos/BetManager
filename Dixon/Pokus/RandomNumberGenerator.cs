using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokus
{
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
            var ints = GenNumbers(n, sum * 100);
            var doubles = new double[n];

            for (int i = 0; i < ints.Length; i++)
            {
                doubles[i] = ints[i] / 100.0;
            }

            return doubles;
        }
    }
}

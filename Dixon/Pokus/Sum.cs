using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokus
{
    class Sum
    {
        public static void Main()
        {
            //int sum = 2000;
            //int size = 20; // assumes that (sum % size == 0)
            //int[] result = new int[size];
            //Random rand = new Random();
            //int x = sum / size;

            //for (int i = 0; i < size; i++)
            //{
            //    result[i] = x;
            //}

            //for (int i = 0; i < x; i++)
            //{
            //    var a = rand.Next(size - 1); // not sure if parameter is inclusive?
            //    var b = rand.Next(size - 1); // should return number between 0 and size-1 inclusively

            //    result[a]++;
            //    result[b]--;
            //}

            //int testSum = result.Sum(); // will equal "sum" (3000)

            RandomNumberGenerator gen = new RandomNumberGenerator(new Random());
            for (int i = 0; i < 50; i++)
            {
                foreach (var x in gen.GenDoubles(20,20))
                {
                    Console.Write("{0} ", x);

                }
                Console.WriteLine();

            }

        }
    }
}

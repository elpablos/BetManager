using Accord.Math.Optimization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccordApp
{
    public class AuglagSampleOne
    {
        public void Solve()
        {
            // Suppose we would like to minimize the following function:
            // 
            //    f(x,y) = min 100(y-x²)²+(1-x)²
            // 
            // Subject to the constraints
            // 
            //    x >= 0  (x must be positive)
            //    y >= 0  (y must be positive)
            // 

            // Now, we can create an objective function using vectors
            var f = new NonlinearObjectiveFunction(numberOfVariables: 2,

                // This is the objective function:  f(x,y) = min 100(y-x²)²+(1-x)²
                function: (x) => 100 * Math.Pow(x[1] - x[0] * x[0], 2) + Math.Pow(1 - x[0], 2),

                // And this is the vector gradient for the same function:
                gradient: (x) => new[]
                {
                    2 * (200 * Math.Pow(x[0], 3) - 200 * x[0] * x[1] + x[0] - 1), // df/dx = 2(200x³-200xy+x-1)
                    200 * (x[1] - x[0]*x[0])                                   // df/dy = 200(y-x²)
                }
            );

            // Now we can start stating the constraints
            var constraints = new List<NonlinearConstraint>()
            {
                // Add the non-negativity constraint for x
                new NonlinearConstraint(f,
                    // 1st constraint: x should be greater than or equal to 0
                    function: (x) => x[0], // x
                    shouldBe: ConstraintType.GreaterThanOrEqualTo,
                    value: 0,
                    gradient: (x) => new[] { 1.0, 0.0 }
                ),

                // Add the non-negativity constraint for y
                new NonlinearConstraint(f,
                    // 2nd constraint: y should be greater than or equal to 0
                    function: (x) => x[1], // y 
                    shouldBe: ConstraintType.GreaterThanOrEqualTo,
                    value: 0,
                    gradient: (x) => new[] { 0.0, 1.0 }
                )
            };

            // Finally, we create the non-linear programming solver
            var solver = new AugmentedLagrangian(f, constraints);

            // And attempt to find a minimum
            bool success = solver.Minimize();

            // The solution found was { 1, 1 }
            double[] solution = solver.Solution;

            // with the minimum value zero.
            double minValue = solver.Value;
        }
    }
}

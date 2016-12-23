//using System;
//using System.Linq;
//using Microsoft.SolverFoundation.Services;

//namespace DecisionTest
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            SolverContext context = SolverContext.GetContext();
//            Model model = context.CreateModel();
//            // Let's create an indexed parameter.
//            Set s = new Set(Domain.Real, "s");
//            Parameter p = new Parameter(Domain.Real, "p", s);
//            // Here's some dummy data.
//            var input = Enumerable.Range(1, 10).Select(item => new { Id = item, Value = item });
//            p.SetBinding(input, "Value", "Id");
//            model.AddParameter(p);

//            // Let's create an indexed decision.
//            Decision x = new Decision(Domain.Real, "x", s);
//            model.AddDecision(x);

//            // Here's a scalar decision.
//            Decision y = new Decision(Domain.Real, "y");
//            model.AddDecision(y);

//            // Here are some dummy constraints to tie everything together. Just for fun.
//            model.AddConstraint("c1", Model.ForEach(s, i => p[i] == x[i]));
//            model.AddConstraint("c2", Model.Sum(Model.ForEach(s, i => x[i])) == y);

//            // Extract data from the indexed decision.
//            context.Solve();
//            foreach (object[] value in x.GetValues())
//            {
//                Console.WriteLine("x[{0}] = {1}", value[1], value[0]);
//            }
//            // Extract data from the scalar decision.
//            Console.WriteLine("y = {0}", y.GetDouble());
//        }
//    }
//}
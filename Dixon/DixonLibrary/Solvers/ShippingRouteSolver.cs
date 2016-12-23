using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;

namespace Dixon.Library.Solvers
{
    class Segment
    {
        // The name of the starting port.
        public string StartingPort { get; set; }

        // A unique identifier for the segment.
        public int Id { get; set; }

        // Segment distance in nautical miles.
        public double Distance { get; set; }

        // The earliest time (in hours) when the ship can depart from port.
        public double MinDepartDay { get; set; }

        // The earliest time (in days) when the ship can depart from port.
        public double MinDepartTime { get { return MinDepartDay * 24.0; } }

        // The latest time (in hours) when the ship can depart from port.
        public double MaxDepartDay { get; set; }

        // The latest time (in days) when the ship can depart from port.
        public double MaxDepartTime { get { return MaxDepartDay * 24.0; } }

        // The departure time.
        public double DepartTime { get; set; }

        // The departure day.
        public double DepartDay { get { return DepartTime / 24.0; } }

        // The average sailing speed (in knots).
        public double Knots { get; set; }

        // Time in port.
        public double WaitTime { get; set; }

        // Number of days in port.
        public double WaitDays { get { return WaitTime / 24.0; } }

        // Returns a string representation of the Segment.
        public override string ToString()
        {
            return String.Format("{0}   [{1}, {2}]   wait {5}   depart {3}   knots {4:f2}",
              StartingPort.PadRight(15),
              MinDepartDay.ToString().PadLeft(2),
              MaxDepartDay.ToString().PadLeft(2),
              DepartDay.ToString("f1").PadLeft(4),
              Knots,
              WaitDays.ToString("f1").PadLeft(4));
        }
    }

    public class ShippingRouteSolver
    {
        public void Solve()
        {
            // data
            Segment[] segmentData = new Segment[] {
              new Segment { Id = 0, Distance = 0, MinDepartDay = 0, MaxDepartDay = 0,
                  StartingPort = "Vancouver" },
              new Segment { Id = 1, Distance = 510, MinDepartDay = 1, MaxDepartDay = 4,
                  StartingPort = "Seattle" },
              new Segment { Id = 2, Distance = 2699, MinDepartDay = 50, MaxDepartDay = 65,
                  StartingPort = "Busan" },
              new Segment { Id = 3, Distance = 838, MinDepartDay = 70, MaxDepartDay = 75,
                  StartingPort = "Kaohsiung" },
              new Segment { Id = 4, Distance = 3625, MinDepartDay = 74, MaxDepartDay = 80,
                  StartingPort = "Hong Kong" }
            };

            // solver init
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            // parameters
            Set segments = new Set(Domain.Integer, "segments");

            Parameter distance = new Parameter(Domain.RealNonnegative, "distance", segments);
            distance.SetBinding(segmentData, "Distance", "Id");

            Parameter early = new Parameter(Domain.RealNonnegative, "early", segments);
            early.SetBinding(segmentData, "MinDepartTime", "Id");

            Parameter late = new Parameter(Domain.RealNonnegative, "late", segments);
            late.SetBinding(segmentData, "MaxDepartTime", "Id");
            model.AddParameters(distance, early, late);

            // decisions
            Decision speed = new Decision(Domain.RealRange(14, 20), "speed", segments);
            speed.SetBinding(segmentData, "Knots", "Id");

            Decision time = new Decision(Domain.RealRange(0, 100 * 24), "time", segments);
            time.SetBinding(segmentData, "DepartTime", "Id");

            Decision wait = new Decision(Domain.RealRange(0, 100 * 24), "wait", segments);
            wait.SetBinding(segmentData, "WaitTime", "Id");
            model.AddDecisions(speed, time, wait);

            // constraints
            model.AddConstraint("bounds", Model.ForEach(segments, s => early[s] <= time[s] <= late[s]));

            // The departure time for segment s is the sum of departure time for the previous segment,
            // the sailing time, and time in port.
            model.AddConstraint("times", Model.ForEachWhere(segments,
              s => time[s - 1] + distance[s - 1] / speed[s - 1] + wait[s] == time[s],
              s => (s > 0)));

            model.AddConstraint("wait_0", wait[0] == 0);

            // goal
            Goal fuel = model.AddGoal("fuel", GoalKind.Minimize, Model.Sum(Model.ForEach(segments,
              s => distance[s] * (0.0036 * Model.Power(speed[s], 2) - 0.1015 * speed[s] + 0.8848)
                  + 0.01 * wait[s])));

            // solve
            context.Solve();
            context.PropagateDecisions();

            // see results
            Console.WriteLine(String.Format("Fuel consumption: {0:f2}", fuel.ToDouble()));
            Console.WriteLine();
            Console.WriteLine("Schedule:");
            Console.WriteLine(new string('-', segmentData[0].ToString().Length));
            foreach (var seg in segmentData)
            {
                Console.WriteLine(seg);
            }
        }
    }
}

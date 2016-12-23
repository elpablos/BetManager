using Microsoft.SolverFoundation.Common;
using Microsoft.SolverFoundation.Services;
using System;

namespace Dixon.Library.Solvers
{
    class CountryDef
    {
        public string Country { get; set; }
        public double MaxProduction { get; set; }
        public double Price { get; set; }
        public double Yield { get; set; }
        public double Production { get; set; }

        public CountryDef(string country, double maxProduction, double price, double yield)
        {
            Country = country;
            MaxProduction = maxProduction;
            Price = price;
            Yield = yield;
            Production = -42;
        }
    }

    class ProductionDef
    {
        public string Product { get; set; }
        public double MinBuy { get; set; }

        public ProductionDef(string product, double minBuy)
        {
            Product = product;
            MinBuy = minBuy;
        }
    }

    class YieldDef
    {
        public string Country { get; set; }
        public string Product { get; set; }
        public double Yield { get; set; }

        public YieldDef(string country, string product, double yield)
        {
            Country = country;
            Product = product;
            Yield = yield;
        }
    }

    public class PetroChemSolver
    {
        public void Solve()
        {
            CountryDef[] ProductionCapacity = new CountryDef[] {
              new CountryDef("Venezuela", 9000, 15, 0.4),
              new CountryDef("Saudi Arabia", 6000, 20, 0.3)
            };

            YieldDef[] ProductionYield = new YieldDef[] {
              new YieldDef("Venezuela", "Gasoline", 0.4),
              new YieldDef("Venezuela", "JetFuel", 0.2),
              new YieldDef("Venezuela", "Lubricant", 0.3),
              new YieldDef("Saudi Arabia", "Gasoline", 0.3),
              new YieldDef("Saudi Arabia", "JetFuel", 0.4),
              new YieldDef("Saudi Arabia", "Lubricant", 0.2)
            };

            ProductionDef[] ProductionRequirments = new ProductionDef[] {
              new ProductionDef("Gasoline", 2000),
              new ProductionDef("JetFuel", 1500),
              new ProductionDef("Lubricant", 500)
            };

            SolverContext context = SolverContext.GetContext();
            context.ClearModel();
            Model model = context.CreateModel();

            Set countries = new Set(Domain.Any, "countries");
            Set products = new Set(Domain.Any, "products");

            Decision buy = new Decision(Domain.RealNonnegative, "barrels", countries);
            buy.SetBinding(ProductionCapacity, "Production", "Country");
            model.AddDecisions(buy);

            Parameter max = new Parameter(Domain.RealNonnegative, "max", countries);
            Parameter price = new Parameter(Domain.RealNonnegative, "price", countries);
            Parameter yield = new Parameter(Domain.RealNonnegative, "yield", countries, products);
            Parameter min = new Parameter(Domain.RealNonnegative, "min", products);
            max.SetBinding(ProductionCapacity, "MaxProduction", "Country");
            price.SetBinding(ProductionCapacity, "Price", "Country");
            yield.SetBinding(ProductionYield, "Yield", "Country", "Product");
            min.SetBinding(ProductionRequirments, "MinBuy", "Product");
            model.AddParameters(max, price, yield, min);

            model.AddConstraint("maxproduction",
                Model.ForEach(countries, c => 0 <= buy[c] <= max[c]));

            model.AddConstraint("productionyield",
                Model.ForEach(
                products,
                p => Model.Sum(Model.ForEach(countries, c => yield[c, p] * buy[c])) >= min[p]));

            model.AddGoal("cost", GoalKind.Minimize,
                Model.Sum(Model.ForEach(countries, c => price[c] * buy[c])));

            Solution solution = context.Solve(new SimplexDirective());

            context.PropagateDecisions();

            Report report = solution.GetReport();
            Console.Write("{0}", report);
        }
    }
}

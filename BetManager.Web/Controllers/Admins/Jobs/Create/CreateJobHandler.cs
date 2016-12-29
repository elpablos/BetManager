using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using Hangfire;
using BetManager.Web.HangfireJobs;

namespace BetManager.Web.Controllers.Admins.Jobs.Create
{
    public class CreateJobHandler : IModelHandler<CreateJobModel>
    {
        public ModelHandlerResult Handle(CreateJobModel model)
        {
            var result = new ModelHandlerResult();

            var job = new SolverJob();

            var tournaments = new int[] { 1 };

            System.Threading.Tasks.Parallel.ForEach(tournaments, t => job.Solve(t, model.DateActual.Value, model.Ksi.Value));

            //job.Solve(model.TournamentId.Value, model.DateActual.Value, model.Ksi.Value);

            //Hangfire.RecurringJob.

            // BackgroundJob.Enqueue<SolverJob>(x => x.Solve(model.TournamentId.Value, model.DateActual.Value, model.Ksi.Value));
            return result;
        }
    }
}
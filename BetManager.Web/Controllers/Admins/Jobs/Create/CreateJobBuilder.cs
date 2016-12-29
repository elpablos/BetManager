using BetManager.Web.Mvc.Common;
using System;
using BetManager.Web.Mvc;
using Hangfire;
using BetManager.Web.HangfireJobs;

namespace BetManager.Web.Controllers.Admins.Jobs.Create
{
    public class CreateJobBuilder : IModelBuilder<CreateJobModel>
    {
        public ModelBuilderResult<CreateJobModel> Build()
        {
            var result = new ModelBuilderResult<CreateJobModel>();
            var tomorrow = DateTime.Now.AddDays(-1);

            result.Model = new CreateJobModel
            {
                DateActual = new DateTime(tomorrow.Ticks - tomorrow.Ticks % TimeSpan.TicksPerDay),
                Ksi = 0.0065/3.5 
            };

            return result;
        }
    }
}
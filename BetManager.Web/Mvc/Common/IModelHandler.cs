using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.Mvc.Common
{
    public interface IModelHandler<in TInput> : IModelHandlerResponse
    {
        ModelHandlerResult Handle(TInput model);
    }

    public interface IModelHandler<in TInput, in THandler> : IModelHandlerResponse
    {
        ModelHandlerResult Handle(TInput model);
    }

    public interface IModelHandler : IModelHandlerResponse
    {
        ModelHandlerResult Handle();
    }

    public interface IModelHandlerResponse
    {
    }
}
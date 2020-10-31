using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoyaltyProgram
{
    public class Bootstrapper : DefaultNancyBootstrapper
    {
        protected override Func<ITypeCatalog, NancyInternalConfiguration> InternalConfiguration => NancyInternalConfiguration.WithOverrides(
            t=>t.StatusCodeHandlers.Clear());

        protected override void ApplicationStartup(
            TinyIoCContainer container,
            IPipelines pipelines)
        {
            pipelines.OnError += (ctx, ex) =>
            {
                Log("Unhandled", ex);
                return null;
            };
        }

        private void Log(string message, Exception ex)
        {
            // send message and ex to central log store
            // in chapter 9 we will see how to do this
        }
    }
}

using System;

using Microsoft.AspNet.SignalR.Hubs;
using CAF.IM.Core.Infrastructure;

namespace CAF.IM.Services.Hubs
{
    public class LoggingHubPipelineModule : HubPipelineModule
    {
        private readonly ILogger _logger;

        public LoggingHubPipelineModule(ILogger logger)
        {
            _logger = logger;
        }

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext context)
        {
            _logger.LogError("{0}: Failure while invoking '{1}'.", context.Hub.Context.Request.User.GetUserId(), context.MethodDescriptor.Name);
            _logger.Log(exceptionContext.Error);
        }
    }
}
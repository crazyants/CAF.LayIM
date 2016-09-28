
using CAF.IM.Core.Infrastructure;
using Microsoft.AspNet.SignalR;

namespace CAF.IM.Services.Hubs
{
    [AuthorizeClaim(ChatClaimTypes.Admin)]
    public class Monitor : Hub
    {
    }
}
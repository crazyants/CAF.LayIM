﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace CAF.IM.ServicesMiddleware
{
    using Core.Infrastructure;
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class RequireHttpsHandler
    {
        private readonly AppFunc _next;

        public RequireHttpsHandler(AppFunc next)
        {
            _next = next;
        }

        public Task Invoke(IDictionary<string, object> env)
        {
            var request = new OwinRequest(env);
            var response = new OwinResponse(env);

            if (!request.Uri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
            {
                var builder = new UriBuilder(request.Uri);
                builder.Scheme = "https";

                if (request.Uri.IsDefaultPort)
                {
                    builder.Port = -1;
                }

                response.Headers.Set("Location", builder.ToString());
                response.StatusCode = 302;

                return TaskAsyncHelper.Empty;
            }
            else
            {
                return _next(env);
            }
        }
    }
}
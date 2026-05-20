using Microsoft.Owin;
using PropMT5Service.Constants;
using System;
using System.Threading.Tasks;

namespace PropMT5Service.Middleware
{
    public class CustomMiddleware : OwinMiddleware
    {
        public CustomMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public async override Task Invoke(IOwinContext context)
        {
            context.Response.Headers[MT5Constants.ServiceInfo.Name] = Environment.MachineName;

            await Next.Invoke(context);
        }
    }
}

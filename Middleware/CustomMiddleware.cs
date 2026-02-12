using Microsoft.Owin;
using System;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Middleware
{
    public class CustomMiddleware : OwinMiddleware
    {
        public CustomMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public async override Task Invoke(IOwinContext context)
        {
            context.Response.Headers["MT5Service"] = Environment.MachineName;

            await Next.Invoke(context);
        }
    }
}

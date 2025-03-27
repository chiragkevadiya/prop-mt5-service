using Microsoft.Owin;
using System;
using System.Threading.Tasks;


namespace NaptunePropTrading_Service.Middleware
{
    public class CustomMiddleware : OwinMiddleware
    {
        public CustomMiddleware(OwinMiddleware next) : base(next)
        {

        }

        public async override Task Invoke(IOwinContext context)
        {
            context.Response.Headers["PropTradingService"] = Environment.MachineName;

            await Next.Invoke(context);
        }
    }
}

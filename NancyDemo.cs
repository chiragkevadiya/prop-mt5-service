using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService
{
    public class NancyDemo : NancyModule
    {
        public NancyDemo()
        {
            //Get["/nancy/demo"] = parameters => new string[] { "Hello", "World" };
        }
    }
}

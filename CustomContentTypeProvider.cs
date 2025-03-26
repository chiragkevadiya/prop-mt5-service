using Microsoft.Owin.StaticFiles.ContentTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaptunePropTrading_Service
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
        }
    }
}

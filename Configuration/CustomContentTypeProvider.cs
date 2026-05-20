using Microsoft.Owin.StaticFiles.ContentTypes;

namespace PropMT5Service
{
    public class CustomContentTypeProvider : FileExtensionContentTypeProvider
    {
        public CustomContentTypeProvider()
        {
            Mappings.Add(".json", "application/json");
        }
    }
}

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using PropMT5ConnectionService.Helper;
using System.Collections.Generic;
using System.Linq;

namespace PropTradingMT5.Helpers
{
    public static class InvalidResponseFactory
    {
        public static IActionResult ProduceErrorResponse(ActionContext context)
        {
            var errors = context.ModelState.GetErrorMessages();
            var response = new BaseResponse { Message = errors.FirstOrDefault(), Success = false };

            return new BadRequestObjectResult(response);
        }
    }

    public static class ModelStateExtensions
    {
        public static List<string> GetErrorMessages(this ModelStateDictionary dictionary)
        {
            return (List<string>)dictionary.SelectMany(m => m.Value.Errors).Select(m => m.ErrorMessage);
        }
    }
}

using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    [RoutePrefix("api/demo/account")]
    public class DemoAccountStatusController : ApiController
    {
        [HttpPost]
        [Route("inactive")]
        public Helpers.BaseResponseModel<int> DemoMT5AccountInActive([FromBody] Mt5AccountStatusVM entity)
        {
            var managerDemo = Mt5DemoManagerFactory.GetManagerDemo();
            if (managerDemo == null)
                return new Helpers.BaseResponseModel<int> { Success = false, Message = "Demo MT5 server is not configured." };

            try
            {
                return MT5AccountOperations.SetAccountActiveStatus(managerDemo, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

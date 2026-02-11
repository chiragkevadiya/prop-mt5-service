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
        CIMTManagerAPI _managerDemo = Mt5DemoManagerFactory.GetManagerDemo();

        [HttpPost]
        [Route("inactive")]
        public BaseResponseModel<int> DemoMT5AccountInActive([FromBody] Mt5AccountStatusVM entity)
        {
            try
            {
                return MT5AccountOperations.SetAccountActiveStatus(_managerDemo, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

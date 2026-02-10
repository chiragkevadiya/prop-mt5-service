using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    [RoutePrefix("api/demo/account")]
    public class DemoMT5AccountInActiveController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();

        [HttpPost]
        [Route("inactive")]
        public BaseResponseModel<int> DemoMT5AccountInActive([FromBody] MT5AccountInActiveVM entity)
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

using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.ViewModels;
using System;
using System.Web.Http;


namespace PropMT5ConnectionService.Controllers
{
    public class LiveAccountStatusController : ApiController
    {
        CIMTManagerAPI _manager = Mt5ManagerFactory.GetManager();

        [HttpPost]
        public BaseResponseModel<int> MT5AccountInActive([FromBody] Mt5AccountStatusVM entity)
        {
            try
            {
                return MT5AccountOperations.SetAccountActiveStatus(_manager, entity);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

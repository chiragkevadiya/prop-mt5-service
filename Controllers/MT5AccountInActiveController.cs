using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using System;
using System.Web.Http;


namespace MT5ConnectionService.Controllers
{
    public class MT5AccountInActiveController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpPost]
        public BaseResponseModel<int> MT5AccountInActive([FromBody] MT5AccountInActiveVM entity)
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

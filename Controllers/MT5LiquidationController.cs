using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels;
using PropMT5ConnectionService.Helper;
using PropMT5ConnectionService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class MT5LiquidationController : ApiController
    {
        private readonly ILiqudationService _liqudationService;

        public MT5LiquidationController()
        {
           
        }

        //}
        /// <summary>
        /// Check and liquidate accounts (all or specific account by ID)
        /// </summary>
        /// <param name="accountId">Optional: Pass accountId to liquidate a single account. Pass 0 (or skip) for all accounts.</param>
        [HttpGet]
        public async Task<IHttpActionResult> MT5Liquidation(long accountId = 0)
        {
            try
            {
                var result = await _liqudationService.CheckAndLiquidateAccounts();

                return Ok(new BaseResponseObject<object>
                {
                    Success = result.Success,
                    Message = result.Message,
                    Data = result.Data
                });
            }
            catch (Exception ex)
            {
                return Content(HttpStatusCode.InternalServerError, new BaseResponseObject<object>
                {
                    Success = false,
                    Message = $"Exception: {ex.Message}"
                });
            }
        }
    }
}

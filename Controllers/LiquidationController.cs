using PropMT5ConnectionService.Helpers;
using PropMT5ConnectionService.Services;
using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    public class LiquidationController : ApiController
    {
        private readonly ILiquidationService _liquidationService;

        public LiquidationController(ILiquidationService liquidationService)
        {
            _liquidationService = liquidationService ?? throw new ArgumentNullException(nameof(liquidationService));
        }

        /// <summary>
        /// Check and liquidate accounts (all or specific account by ID)
        /// </summary>
        /// <param name="accountId">Optional: Pass accountId to liquidate a single account. Pass 0 (or skip) for all accounts.</param>
        [HttpGet]
        public async Task<IHttpActionResult> MT5Liquidation(long accountId = 0)
        {
            try
            {
                var result = await _liquidationService.CheckAndLiquidateAccounts();

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

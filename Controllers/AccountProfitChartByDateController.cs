using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class AccountProfitChartByDateController : ApiController
    {
        private readonly CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        [HttpGet]
        public async Task<BaseResponseModel<ChartProfitVM>> GetProfitChartByDateRange(ulong loginId, string fromDate, string toDate)
        {
            var response = new BaseResponseModel<ChartProfitVM>();

            try
            {
                // If dates are null or empty, use the last 7 days
                DateTime from, to;
                if (string.IsNullOrWhiteSpace(fromDate) || string.IsNullOrWhiteSpace(toDate))
                {
                    to = DateTime.UtcNow.Date;
                    from = to.AddDays(-6); // Last 7 days including today
                }
                else
                {
                    from = DateTime.ParseExact(fromDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                    to = DateTime.ParseExact(toDate, "dd-MM-yyyy", CultureInfo.InvariantCulture).Date;
                }

                if (from > to)
                    throw new ArgumentException("From date cannot be after To date.");

                DateTimeOffset startOffset = new DateTimeOffset(from, TimeSpan.Zero);
                DateTimeOffset endOffset = new DateTimeOffset(to.AddDays(1), TimeSpan.Zero); // inclusive of last day

                long fromTimestamp = startOffset.ToUnixTimeSeconds();
                long toTimestamp = endOffset.ToUnixTimeSeconds();

                // Single API call to fetch deals in the entire range
                CIMTDealArray dealArray = _manager.DealCreateArray();
                if (dealArray == null)
                {
                    response.Success = false;
                    response.Message = "Failed to create deal array.";
                    response.MTRetErrorCode = MTRetCode.MT_RET_ERR_CANCEL;
                    return response;
                }

                MTRetCode result = _manager.DealRequestByLogins(new ulong[] { loginId }, fromTimestamp, toTimestamp, dealArray);
                List<CIMTDeal> closedDeals = new List<CIMTDeal>();
                if (result == MTRetCode.MT_RET_OK)
                {
                    closedDeals = dealArray.ToArray()
                        .Where(deal => deal.Entry() == 1) // DEAL_ENTRY_OUT
                        .ToList();
                }

                var categories = new List<string>();
                var profitData = new List<double>();

                // Group deals by date
                var groupedProfits = closedDeals
                    .GroupBy(deal => DateTimeOffset.FromUnixTimeSeconds(deal.Time()).UtcDateTime.Date)
                    .ToDictionary(
                        g => g.Key,
                        g => Math.Round(g.Sum(d => d.Profit()), 2)
                    );

                for (DateTime date = from.Date; date <= to.Date; date = date.AddDays(1))
                {
                    categories.Add(date.ToString("dd-MM-yyyy"));
                    profitData.Add(groupedProfits.ContainsKey(date) ? groupedProfits[date] : 0.0);
                }

                response.Data = new ChartProfitVM
                {
                    Date = categories,
                    Profit = profitData
                };
                response.Success = true;
                response.Message = "Profit chart data fetched successfully.";
                response.MTRetErrorCode = MTRetCode.MT_RET_OK;
                dealArray.Release();

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error in GetProfitChartByDateRange: " + ex.Message;
                response.MTRetErrorCode = MTRetCode.MT_RET_ERROR;
                return response;
            }
        }
    }

    public class ChartProfitVM
    {
        public List<string> Date { get; set; }
        public List<double> Profit { get; set; }
    }

}

using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.StaticMethod;
using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoMT5TradingHistoryController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();
        public DemoMT5TradingHistoryController()
        {

        }

        [HttpGet]
        public IEnumerable<MT5TradingHistoryVM> TradingHistoryFromDateToDate(ulong LoginId, string fromDatet, string toDatet)
        {
            try
            {
                List<MT5TradingHistoryVM> ListTradingHistoryVM = new List<MT5TradingHistoryVM>();

                ulong[] LoginIds = { LoginId };

                // Assuming DateFormatCovert has a static method FormatDate that returns DateTimeOffset
                DateTimeOffset dateFromString = DateFormatCovert.FormatDate(fromDatet);
                DateTimeOffset dateToString = DateFormatCovert.FormatDate(toDatet);

                DateTimeOffset startDate = dateFromString.AddDays(-1);
                DateTimeOffset endDate = dateToString.AddDays(1);

                long fromDateAss = (long)(startDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;
                long toDateAss = (long)(endDate - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds;


                CIMTDealArray ciMTDealArray = _managerDemo.DealCreateArray();
                MTRetCode mTRetCode12 = _managerDemo.DealRequestByLogins(LoginIds, fromDateAss, toDateAss, ciMTDealArray);


                if (MTRetCode.MT_RET_OK == mTRetCode12)
                {

                    foreach (var Item in ciMTDealArray.ToArray())
                    {

                        MT5TradingHistoryVM liveAccountVM1 = new MT5TradingHistoryVM()
                        {
                            //Time1 = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                            Order = Item.Order(),
                            Symbol = Item.Symbol(),
                            //Type = Item.GetType(),
                            Volume = Item.Volume(),
                            Price = Item.Price(),
                            PriceSL = Item.PriceSL(),
                            PriceTP = Item.PriceTP(),
                            //Time2 = DateTimeOffset.FromUnixTimeSeconds(Item.Time()).DateTime,
                            Profit = Item.Profit(),

                            //Change = Item.Change()

                            // Additional properties

                            //Commission = Item.Commission().,
                            //DealVolume = Item.Volume(),

                            Comment = Item.Comment(),
                            Deal = Item.Deal(),
                            Login = Item.Login(),
                            MarketAsk = Item.MarketAsk(),
                            MarketBid = Item.MarketBid(),
                            PositionID = Item.PositionID(),
                            //Print = Item.Print(),
                            RateProfit = Item.RateProfit(),
                            RateMargin = Item.RateMargin()

                        };

                        ListTradingHistoryVM.Add(liveAccountVM1);
                    }

                    ciMTDealArray.Clear();
                    ciMTDealArray.Release();

                    return ListTradingHistoryVM;
                }
                else
                {
                    MT5TradingHistoryVM liveAccountVM1 = new MT5TradingHistoryVM();
                    liveAccountVM1.mTRetCodeError = mTRetCode12;

                    // Create a list containing the single object
                    List<MT5TradingHistoryVM> resultList = new List<MT5TradingHistoryVM> { liveAccountVM1 };

                    return resultList;

                }


            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}

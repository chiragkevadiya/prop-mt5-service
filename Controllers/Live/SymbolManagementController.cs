using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 symbols and symbol configurations
    /// </summary>
    [RoutePrefix("api/symbols")]
    public class SymbolManagementController : BaseApiController
    {
        public SymbolManagementController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all available symbols
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllSymbols()
        {
            return ExecuteSafe(() =>
            {
                uint total = _manager.SymbolTotal();
                var symbols = new List<object>();

                for (uint i = 0; i < total; i++)
                {
                    var symbol = _manager.SymbolCreate();
                    if (_manager.SymbolNext(i, symbol) == MTRetCode.MT_RET_OK)
                    {
                        symbols.Add(new
                        {
                            Symbol = symbol.Symbol(),
                            Path = symbol.Path(),
                            ISIN = symbol.ISIN(),
                            Description = symbol.Description(),
                            Page = symbol.Page(),
                            CurrencyBase = symbol.CurrencyBase(),
                            CurrencyProfit = symbol.CurrencyProfit(),
                            CurrencyMargin = symbol.CurrencyMargin(),
                            Digits = symbol.Digits(),
                            Point = symbol.Point(),
                            ContractSize = symbol.ContractSize(),
                            TickSize = symbol.TickSize(),
                            TickValue = symbol.TickValue(),
                            TradeMode = symbol.TradeMode()
                        });
                    }
                    symbol.Release();
                }

                return new BaseResponse<object>().WithSuccess(symbols, $"Retrieved {symbols.Count} symbols");
            });
        }

        /// <summary>
        /// Get specific symbol details
        /// </summary>
        [HttpGet]
        [Route("{symbolName}")]
        public IHttpActionResult GetSymbol(string symbolName)
        {
            return ExecuteSafe(() =>
            {
                var symbol = _manager.SymbolCreate();
                var result = _manager.SymbolGet(symbolName, symbol);

                if (result != MTRetCode.MT_RET_OK)
                {
                    symbol.Release();
                    return new BaseResponse<object>().WithError($"Symbol '{symbolName}' not found", 404);
                }

                var symbolData = new
                {
                    Symbol = symbol.Symbol(),
                    Path = symbol.Path(),
                    ISIN = symbol.ISIN(),
                    Description = symbol.Description(),
                    International = symbol.International(),
                    Basis = symbol.Basis(),
                    Source = symbol.Source(),
                    Page = symbol.Page(),
                    CurrencyBase = symbol.CurrencyBase(),
                    CurrencyBaseDigits = symbol.CurrencyBaseDigits(),
                    CurrencyProfit = symbol.CurrencyProfit(),
                    CurrencyProfitDigits = symbol.CurrencyProfitDigits(),
                    CurrencyMargin = symbol.CurrencyMargin(),
                    CurrencyMarginDigits = symbol.CurrencyMarginDigits(),
                    Color = symbol.Color(),
                    ColorBackground = symbol.ColorBackground(),
                    Digits = symbol.Digits(),
                    Point = symbol.Point(),
                    Multiply = symbol.Multiply(),
                    TickFlags = symbol.TickFlags(),
                    TickBookDepth = symbol.TickBookDepth(),
                    ChartMode = symbol.ChartMode(),
                    FilterSoft = symbol.FilterSoft(),
                    FilterSoftTicks = symbol.FilterSoftTicks(),
                    FilterHard = symbol.FilterHard(),
                    FilterHardTicks = symbol.FilterHardTicks(),
                    FilterDiscard = symbol.FilterDiscard(),
                    FilterSpreadMax = symbol.FilterSpreadMax(),
                    FilterSpreadMin = symbol.FilterSpreadMin(),
                    FilterGap = symbol.FilterGap(),
                    FilterGapTicks = symbol.FilterGapTicks(),
                    TradeMode = symbol.TradeMode(),
                    TradeFlags = symbol.TradeFlags(),
                    CalcMode = symbol.CalcMode(),
                    ExecMode = symbol.ExecMode(),
                    GTCMode = symbol.GTCMode(),
                    FillFlags = symbol.FillFlags(),
                    ExpirFlags = symbol.ExpirFlags(),
                    OrderFlags = symbol.OrderFlags(),
                    Spread = symbol.Spread(),
                    SpreadBalance = symbol.SpreadBalance(),
                    SpreadDiff = symbol.SpreadDiff(),
                    SpreadDiffBalance = symbol.SpreadDiffBalance(),
                    TickValue = symbol.TickValue(),
                    TickSize = symbol.TickSize(),
                    ContractSize = symbol.ContractSize(),
                    StopsLevel = symbol.StopsLevel(),
                    FreezeLevel = symbol.FreezeLevel(),
                    QuotesTimeout = symbol.QuotesTimeout(),
                    VolumeMin = symbol.VolumeMin(),
                    VolumeMinExt = symbol.VolumeMinExt(),
                    VolumeMax = symbol.VolumeMax(),
                    VolumeMaxExt = symbol.VolumeMaxExt(),
                    VolumeStep = symbol.VolumeStep(),
                    VolumeStepExt = symbol.VolumeStepExt(),
                    VolumeLimit = symbol.VolumeLimit(),
                    VolumeLimitExt = symbol.VolumeLimitExt(),
                    MarginFlags = symbol.MarginFlags(),
                    MarginInitial = symbol.MarginInitial(),
                    MarginMaintenance = symbol.MarginMaintenance(),
                    MarginLong = symbol.MarginLong(),
                    MarginShort = symbol.MarginShort(),
                    MarginLimit = symbol.MarginLimit(),
                    MarginStop = symbol.MarginStop(),
                    MarginStopLimit = symbol.MarginStopLimit(),
                    PriceLimitMax = symbol.PriceLimitMax(),
                    PriceLimitMin = symbol.PriceLimitMin(),
                    PriceSettle = symbol.PriceSettle()
                };

                symbol.Release();
                return new BaseResponse<object>().WithSuccess(symbolData, "Symbol details retrieved successfully");
            });
        }

        /// <summary>
        /// Get symbols by path/category
        /// </summary>
        [HttpGet]
        [Route("path/{path}")]
        public IHttpActionResult GetSymbolsByPath(string path)
        {
            return ExecuteSafe(() =>
            {
                uint total = _manager.SymbolTotal();
                var symbols = new List<object>();

                for (uint i = 0; i < total; i++)
                {
                    var symbol = _manager.SymbolCreate();
                    if (_manager.SymbolNext(i, symbol) == MTRetCode.MT_RET_OK)
                    {
                        if (symbol.Path().Contains(path))
                        {
                            symbols.Add(new
                            {
                                Symbol = symbol.Symbol(),
                                Path = symbol.Path(),
                                Description = symbol.Description(),
                                Digits = symbol.Digits(),
                                ContractSize = symbol.ContractSize(),
                                TradeMode = symbol.TradeMode()
                            });
                        }
                    }
                    symbol.Release();
                }

                return new BaseResponse<object>().WithSuccess(symbols, $"Retrieved {symbols.Count} symbols from path '{path}'");
            });
        }

        /// <summary>
        /// Get symbol sessions/trading hours (Note: session info may not be available)
        /// </summary>
        [HttpGet]
        [Route("{symbolName}/sessions")]
        public IHttpActionResult GetSymbolSessions(string symbolName)
        {
            return ExecuteSafe(() =>
            {
                var symbol = _manager.SymbolCreate();
                var result = _manager.SymbolGet(symbolName, symbol);

                if (result != MTRetCode.MT_RET_OK)
                {
                    symbol.Release();
                    return new BaseResponse<object>().WithError($"Symbol '{symbolName}' not found", 404);
                }

                // Note: Session information may not be available in all MT5 API versions
                // Return basic symbol info instead
                var symbolInfo = new
                {
                    Symbol = symbol.Symbol(),
                    Path = symbol.Path(),
                    Description = symbol.Description(),
                    Message = "Session details API not available in this MT5 version"
                };

                symbol.Release();

                return new BaseResponse<object>().WithSuccess(symbolInfo, $"Symbol info retrieved for '{symbolName}'");
            });
        }

        private string GetDayName(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0: return "Sunday";
                case 1: return "Monday";
                case 2: return "Tuesday";
                case 3: return "Wednesday";
                case 4: return "Thursday";
                case 5: return "Friday";
                case 6: return "Saturday";
                default: return "Unknown";
            }
        }
    }
}

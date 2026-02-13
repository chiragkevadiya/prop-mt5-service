using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 positions
    /// </summary>
    [RoutePrefix("api/positions")]
    public class PositionController : BaseApiController
    {
        public PositionController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all open positions for an account
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}")]
        public IHttpActionResult GetPositions(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var positionsArray = _manager.PositionCreateArray();
                var result = _manager.PositionGet((ulong)loginId, positionsArray);

                if (result != MTRetCode.MT_RET_OK)
                {
                    positionsArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var positions = new List<object>();
                uint total = positionsArray.Total();

                for (uint i = 0; i < total; i++)
                {
                    var position = positionsArray.Next(i);
                    if (position != null)
                    {
                        positions.Add(new
                        {
                            Position = position.Position(),
                            Login = position.Login(),
                            Symbol = position.Symbol(),
                            Action = position.Action() == 0 ? "BUY" : "SELL",
                            Digits = position.Digits(),
                            Volume = (decimal)position.Volume() / 10000,
                            PriceOpen = Math.Round(position.PriceOpen(), 5),
                            PriceCurrent = Math.Round(position.PriceCurrent(), 5),
                            PriceSL = Math.Round(position.PriceSL(), 5),
                            PriceTP = Math.Round(position.PriceTP(), 5),
                            TimeCreate = DateTimeOffset.FromUnixTimeSeconds(position.TimeCreate()).LocalDateTime,
                            TimeUpdate = DateTimeOffset.FromUnixTimeSeconds(position.TimeUpdate()).LocalDateTime,
                            Storage = position.Storage(),
                            Profit = Math.Round(position.Profit(), 2),
                            RateProfit = position.RateProfit(),
                            Comment = position.Comment(),
                            ExpertID = position.ExpertID(),
                            ExpertPositionID = position.ExpertPositionID()
                        });
                    }
                }

                positionsArray.Release();
                return new BaseResponse<object>().WithSuccess(positions, "Positions retrieved successfully");
            });
        }

        /// <summary>
        /// Get specific position by position ID
        /// </summary>
        [HttpGet]
        [Route("position/{positionId:long}")]
        public IHttpActionResult GetPosition(long positionId)
        {
            return ExecuteSafe(() =>
            {
                var position = _manager.PositionCreate();
                var result = _manager.PositionGetByTicket((ulong)positionId, position);

                if (result != MTRetCode.MT_RET_OK)
                {
                    position.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(result), 400);
                }

                var positionData = new
                {
                    Position = position.Position(),
                    Login = position.Login(),
                    Symbol = position.Symbol(),
                    Action = position.Action() == 0 ? "BUY" : "SELL",
                    Digits = position.Digits(),
                    Volume = (decimal)position.Volume() / 10000,
                    PriceOpen = Math.Round(position.PriceOpen(), 5),
                    PriceCurrent = Math.Round(position.PriceCurrent(), 5),
                    PriceSL = Math.Round(position.PriceSL(), 5),
                    PriceTP = Math.Round(position.PriceTP(), 5),
                    TimeCreate = DateTimeOffset.FromUnixTimeSeconds(position.TimeCreate()).LocalDateTime,
                    TimeUpdate = DateTimeOffset.FromUnixTimeSeconds(position.TimeUpdate()).LocalDateTime,
                    Storage = position.Storage(),
                    Profit = Math.Round(position.Profit(), 2),
                    RateProfit = position.RateProfit(),
                    Comment = position.Comment()
                };

                position.Release();
                return new BaseResponse<object>().WithSuccess(positionData, "Position retrieved successfully");
            });
        }

        /// <summary>
        /// Get positions by symbol
        /// </summary>
        [HttpGet]
        [Route("symbol/{symbol}")]
        public IHttpActionResult GetPositionsBySymbol(string symbol)
        {
            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var userResult = _manager.UserAccountRequestArray("*", accountArray);

                if (userResult != MTRetCode.MT_RET_OK)
                {
                    accountArray.Release();
                    return new BaseResponse<object>().WithError(GetMT5ErrorMessage(userResult), 400);
                }

                var positions = new List<object>();
                uint totalUsers = accountArray.Total();

                for (uint i = 0; i < totalUsers; i++)
                {
                    var account = accountArray.Next(i);
                    if (account != null)
                    {
                        var userPositionsArray = _manager.PositionCreateArray();
                        if (_manager.PositionGet(account.Login(), userPositionsArray) == MTRetCode.MT_RET_OK)
                        {
                            uint total = userPositionsArray.Total();
                            for (uint j = 0; j < total; j++)
                            {
                                var position = userPositionsArray.Next(j);
                                if (position != null && position.Symbol() == symbol)
                                {
                                    positions.Add(new
                                    {
                                        Position = position.Position(),
                                        Login = position.Login(),
                                        Symbol = position.Symbol(),
                                        Action = position.Action() == 0 ? "BUY" : "SELL",
                                        Volume = (decimal)position.Volume() / 10000,
                                        PriceOpen = Math.Round(position.PriceOpen(), 5),
                                        PriceCurrent = Math.Round(position.PriceCurrent(), 5),
                                        Profit = Math.Round(position.Profit(), 2),
                                        TimeCreate = DateTimeOffset.FromUnixTimeSeconds(position.TimeCreate()).LocalDateTime
                                    });
                                }
                            }
                        }
                        userPositionsArray.Release();
                    }
                }

                accountArray.Release();
                return new BaseResponse<object>().WithSuccess(positions, $"Retrieved {positions.Count} positions for symbol '{symbol}'");
            });
        }

        /// <summary>
        /// Get total open positions count
        /// </summary>
        [HttpGet]
        [Route("{loginId:long}/count")]
        public IHttpActionResult GetPositionCount(long loginId)
        {
            return ExecuteSafe(() =>
            {
                var positionsArray = _manager.PositionCreateArray();
                var result = _manager.PositionGet((ulong)loginId, positionsArray);

                uint count = 0;
                if (result == MTRetCode.MT_RET_OK)
                {
                    count = positionsArray.Total();
                }

                positionsArray.Release();
                return new BaseResponse<object>().WithSuccess(new { Count = count }, "Position count retrieved successfully");
            });
        }
    }
}

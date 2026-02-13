using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.Helpers;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace PropMT5ConnectionService.Controllers
{
    /// <summary>
    /// Controller for managing MT5 groups
    /// </summary>
    [RoutePrefix("api/groups")]
    public class GroupManagementController : BaseApiController
    {
        public GroupManagementController(CIMTManagerAPI manager) : base(manager) { }

        /// <summary>
        /// Get all groups
        /// </summary>
        [HttpGet]
        [Route("")]
        public IHttpActionResult GetAllGroups()
        {
            return ExecuteSafe(() =>
            {
                uint total = _manager.GroupTotal();
                var groups = new List<object>();

                for (uint i = 0; i < total; i++)
                {
                    var group = _manager.GroupCreate();
                    if (_manager.GroupNext(i, group) == MTRetCode.MT_RET_OK)
                    {
                        groups.Add(new
                        {
                            Group = group.Group(),
                            Server = group.Server(),
                            Company = group.Company(),
                            Currency = group.Currency(),
                            CurrencyDigits = group.CurrencyDigits(),
                            ReportsFlags = group.ReportsFlags(),
                            ReportsEmail = group.ReportsEmail(),
                            ReportsSMTP = group.ReportsSMTP(),
                            NewsMode = group.NewsMode(),
                            NewsCategory = group.NewsCategory(),
                            MailMode = group.MailMode(),
                            TradeFlags = group.TradeFlags(),
                            TradeTransferMode = group.TradeTransferMode(),
                            TradeInterestrate = group.TradeInterestrate(),
                            TradeVirtualCredit = group.TradeVirtualCredit(),
                            MarginMode = group.MarginMode(),
                            MarginSOMode = group.MarginSOMode(),
                            MarginFreeMode = group.MarginFreeMode(),
                            MarginCall = group.MarginCall(),
                            MarginStopOut = group.MarginStopOut(),
                            MarginFreeProfitMode = group.MarginFreeProfitMode(),
                            DemoLeverage = group.DemoLeverage(),
                            DemoDeposit = group.DemoDeposit(),
                            LimitHistory = group.LimitHistory(),
                            LimitOrders = group.LimitOrders(),
                            LimitSymbols = group.LimitSymbols(),
                            LimitPositions = group.LimitPositions()
                        });
                    }
                    group.Release();
                }

                return new BaseResponse<object>().WithSuccess(groups, $"Retrieved {groups.Count} groups");
            });
        }

        /// <summary>
        /// Get specific group details
        /// </summary>
        [HttpGet]
        [Route("{groupName}")]
        public IHttpActionResult GetGroup(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var group = _manager.GroupCreate();
                var result = _manager.GroupGet(groupName, group);

                if (result != MTRetCode.MT_RET_OK)
                {
                    group.Release();
                    return new BaseResponse<object>().WithError($"Group '{groupName}' not found", 404);
                }

                var groupData = new
                {
                    Group = group.Group(),
                    Server = group.Server(),
                    PermissionsFlags = group.PermissionsFlags(),
                    AuthMode = group.AuthMode(),
                    AuthPasswordMin = group.AuthPasswordMin(),
                    AuthOTPMode = group.AuthOTPMode(),
                    Company = group.Company(),
                    CompanyPage = group.CompanyPage(),
                    CompanyEmail = group.CompanyEmail(),
                    CompanySupportPage = group.CompanySupportPage(),
                    CompanySupportEmail = group.CompanySupportEmail(),
                    CompanyCatalog = group.CompanyCatalog(),
                    Currency = group.Currency(),
                    CurrencyDigits = group.CurrencyDigits(),
                    ReportsMode = group.ReportsMode(),
                    ReportsFlags = group.ReportsFlags(),
                    ReportsEmail = group.ReportsEmail(),
                    ReportsSMTP = group.ReportsSMTP(),
                    NewsMode = group.NewsMode(),
                    NewsCategory = group.NewsCategory(),
                    MailMode = group.MailMode(),
                    TradeFlags = group.TradeFlags(),
                    TradeTransferMode = group.TradeTransferMode(),
                    TradeInterestrate = group.TradeInterestrate(),
                    TradeVirtualCredit = group.TradeVirtualCredit(),
                    MarginMode = group.MarginMode(),
                    MarginSOMode = group.MarginSOMode(),
                    MarginFreeMode = group.MarginFreeMode(),
                    MarginCall = group.MarginCall(),
                    MarginStopOut = group.MarginStopOut(),
                    MarginFreeProfitMode = group.MarginFreeProfitMode(),
                    DemoLeverage = group.DemoLeverage(),
                    DemoDeposit = group.DemoDeposit(),
                    LimitHistory = group.LimitHistory(),
                    LimitOrders = group.LimitOrders(),
                    LimitSymbols = group.LimitSymbols(),
                    LimitPositions = group.LimitPositions()
                };

                group.Release();
                return new BaseResponse<object>().WithSuccess(groupData, "Group details retrieved successfully");
            });
        }

        /// <summary>
        /// Get symbols configured for a group
        /// </summary>
        [HttpGet]
        [Route("{groupName}/symbols")]
        public IHttpActionResult GetGroupSymbols(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var group = _manager.GroupCreate();
                var result = _manager.GroupGet(groupName, group);

                if (result != MTRetCode.MT_RET_OK)
                {
                    group.Release();
                    return new BaseResponse<object>().WithError($"Group '{groupName}' not found", 404);
                }

                var symbols = new List<object>();
                uint total = group.SymbolTotal();

                for (uint i = 0; i < total; i++)
                {
                    var groupSymbol = _manager.GroupSymbolCreate();
                    if (group.SymbolNext(i, groupSymbol) == MTRetCode.MT_RET_OK)
                    {
                        symbols.Add(new
                        {
                            Path = groupSymbol.Path(),
                            TradeMode = groupSymbol.TradeMode(),
                            ExecMode = groupSymbol.ExecMode(),
                            FillFlags = groupSymbol.FillFlags(),
                            ExpirFlags = groupSymbol.ExpirFlags(),
                            SpreadDiff = groupSymbol.SpreadDiff(),
                            SpreadDiffBalance = groupSymbol.SpreadDiffBalance(),
                            StopsLevel = groupSymbol.StopsLevel(),
                            FreezeLevel = groupSymbol.FreezeLevel(),
                            VolumeMin = groupSymbol.VolumeMin(),
                            VolumeMinExt = groupSymbol.VolumeMinExt(),
                            VolumeMax = groupSymbol.VolumeMax(),
                            VolumeMaxExt = groupSymbol.VolumeMaxExt(),
                            VolumeStep = groupSymbol.VolumeStep(),
                            VolumeStepExt = groupSymbol.VolumeStepExt(),
                            VolumeLimit = groupSymbol.VolumeLimit(),
                            VolumeLimitExt = groupSymbol.VolumeLimitExt(),
                            MarginFlags = groupSymbol.MarginFlags(),
                            MarginInitial = groupSymbol.MarginInitial(),
                            MarginMaintenance = groupSymbol.MarginMaintenance()
                        });
                    }
                    groupSymbol.Release();
                }

                group.Release();
                return new BaseResponse<object>().WithSuccess(symbols, $"Retrieved {symbols.Count} symbols for group '{groupName}'");
            });
        }

        /// <summary>
        /// Get commission settings for a group
        /// </summary>
        [HttpGet]
        [Route("{groupName}/commissions")]
        public IHttpActionResult GetGroupCommissions(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var group = _manager.GroupCreate();
                var result = _manager.GroupGet(groupName, group);

                if (result != MTRetCode.MT_RET_OK)
                {
                    group.Release();
                    return new BaseResponse<object>().WithError($"Group '{groupName}' not found", 404);
                }

                var commissions = new List<object>();
                uint total = group.CommissionTotal();

                for (uint i = 0; i < total; i++)
                {
                    var commission = _manager.GroupCommissionCreate();
                    if (group.CommissionNext(i, commission) == MTRetCode.MT_RET_OK)
                    {
                        var tiers = new List<object>();
                        uint tierTotal = commission.TierTotal();

                        for (uint j = 0; j < tierTotal; j++)
                        {
                            var tier = _manager.GroupTierCreate();
                            if (commission.TierNext(j, tier) == MTRetCode.MT_RET_OK)
                            {
                                tiers.Add(new
                                {
                                    Mode = tier.Mode(),
                                    Type = tier.Type(),
                                    Value = tier.Value(),
                                    RangeFrom = tier.RangeFrom(),
                                    RangeTo = tier.RangeTo(),
                                    Minimal = tier.Minimal(),
                                    Maximal = tier.Maximal(),
                                    Currency = tier.Currency()
                                });
                            }
                            tier.Release();
                        }

                        commissions.Add(new
                        {
                            Name = commission.Name(),
                            Description = commission.Description(),
                            Path = commission.Path(),
                            Mode = commission.Mode(),
                            ChargeMode = commission.ChargeMode(),
                            Tiers = tiers
                        });
                    }
                    commission.Release();
                }

                group.Release();
                return new BaseResponse<object>().WithSuccess(commissions, $"Retrieved {commissions.Count} commissions for group '{groupName}'");
            });
        }

        /// <summary>
        /// Get accounts count in a group
        /// </summary>
        [HttpGet]
        [Route("{groupName}/accounts/count")]
        public IHttpActionResult GetGroupAccountsCount(string groupName)
        {
            return ExecuteSafe(() =>
            {
                var accountArray = _manager.UserCreateAccountArray();
                var result = _manager.UserAccountRequestArray(groupName, accountArray);

                uint count = 0;
                if (result == MTRetCode.MT_RET_OK)
                {
                    count = accountArray.Total();
                }

                accountArray.Release();
                return new BaseResponse<object>().WithSuccess(new { Count = count }, $"Group '{groupName}' has {count} accounts");
            });
        }
    }
}

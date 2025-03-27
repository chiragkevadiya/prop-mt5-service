using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using NaptunePropTrading_Service.Helper;
using System;
using System.Collections.Generic;
using System.Web.Http;
using MT5ConnectionService.ViewModels;

namespace NaptunePropTrading_Service.Controllers
{
    public class GroupController : ApiController
    {
        CIMTManagerAPI _manager = CreateManagerHelper.GetManager();

        public GroupController()
        {

        }

        [HttpGet]
        public GroupListVM GetGroupName()
        {
            try
            {
                var groupNameVMs = new List<GroupNameVM>();
                var groupCommissionLists = new List<GroupCommissionVM>();

                uint totalGroups = _manager.GroupTotal();

                CIMTConGroup cIMTConGroup = _manager.GroupCreate();
                CIMTConCommTier cIMTConCommTier = _manager.GroupTierCreate();
                CIMTConCommission cIMTConCommission = _manager.GroupCommissionCreate();
                CIMTConSymbol cIMTConSymbols = _manager.SymbolCreate();


                for (uint i = 0; i < totalGroups; i++)
                {
                    if (_manager.GroupNext(i, cIMTConGroup) != MTRetCode.MT_RET_OK)
                        continue;

                    uint symbolTotal = cIMTConGroup.SymbolTotal();
                    uint commissionTotal = cIMTConGroup.CommissionTotal();

                    // Process symbols (if needed)
                    for (uint j = 0; j < symbolTotal; j++)
                    {
                        if (_manager.SymbolNext(j, cIMTConSymbols) != MTRetCode.MT_RET_OK)
                            continue;
                    }

                    // Process commissions and tiers
                    for (uint k = 0; k < commissionTotal; k++)
                    {
                        if (cIMTConGroup.CommissionNext(k, cIMTConCommission) != MTRetCode.MT_RET_OK)
                            continue;

                        for (uint l = 0; l <= cIMTConCommission.TierTotal(); l++)
                        {
                            if (cIMTConCommission.TierNext(l, cIMTConCommTier) != MTRetCode.MT_RET_OK)
                                continue;

                            var groupCommissionTemp = new GroupCommissionVM
                            {
                                GroupCommissions = cIMTConCommTier.Value(),
                                GroupNames = cIMTConGroup.Group(),
                                Name = cIMTConCommission.Name(),
                                Description = cIMTConCommission.Description(),
                                Path = cIMTConCommission.Path(),
                                ChargeMode = cIMTConCommission.ChargeMode().ToString(),
                            };
                            groupCommissionLists.Add(groupCommissionTemp);
                        }
                    }


                    // Process Group Symbol
                    CIMTConGroupSymbol GroupSymbol = _manager.GroupSymbolCreate();
                    string trimPath = cIMTConCommission.Path();
                    MTRetCode MTRetCode1 = cIMTConGroup.SymbolGet(trimPath == "" ? "*" : trimPath, GroupSymbol);

                    var groupNameVM = new GroupNameVM
                    {
                        GroupName = cIMTConGroup.Group(),
                        MarginCall = cIMTConGroup.MarginCall(),
                        StopOutLevel = cIMTConGroup.MarginStopOut(),
                        Currency = cIMTConGroup.Currency(),
                        CurrencyDigits = cIMTConGroup.CurrencyDigits(),
                        Instruments = symbolTotal,
                        Spread = GroupSymbol.SpreadDiff() == 2147483647 ? 0 : GroupSymbol.SpreadDiff(), // Assuming spread is obtained here
                    };
                    groupNameVMs.Add(groupNameVM);

                    GroupSymbol.Release();
                }

                // Release resources
                cIMTConGroup.Release();
                cIMTConSymbols.Release();
                cIMTConCommission.Release();
                cIMTConCommTier.Release();


                return new GroupListVM
                {
                    GroupTotal = totalGroups,
                    GroupList = groupNameVMs,
                    GroupCommissionList = groupCommissionLists
                };
            }
            catch (Exception)
            {
                // Handle the exception as needed, or log it
                throw;
            }
        }

        [HttpPost]
        public List<GroupwithSymbolVM> GetGroupwithsymbol()
        {
            try
            {
                uint symbolTotal = 0;
                uint totalGroups = _manager.GroupTotal();

                List<GroupwithSymbolVM> groupwithSymbolList = new List<GroupwithSymbolVM>();

                CIMTConGroup cIMTConGroup = _manager.GroupCreate();
                CIMTConGroupSymbol cIMTConGroupSymbol = _manager.GroupSymbolCreate();

                for (uint i = 0; i < totalGroups; i++)
                {
                    GroupwithSymbolVM groupwithSymbol = new GroupwithSymbolVM();
                    groupwithSymbol.SymbolPath = new List<string>();
                    if (_manager.GroupNext(i, cIMTConGroup) != MTRetCode.MT_RET_OK)
                        continue;
                    groupwithSymbol.GroupName = cIMTConGroup.Group();

                    symbolTotal = cIMTConGroup.SymbolTotal();

                    for (uint j = 0; j < symbolTotal; j++)
                    {
                        if (cIMTConGroup.SymbolNext(j, cIMTConGroupSymbol) != MTRetCode.MT_RET_OK)
                            continue;

                        groupwithSymbol.SymbolPath.Add(cIMTConGroupSymbol.Path());
                    }
                    groupwithSymbolList.Add(groupwithSymbol);
                }
                return groupwithSymbolList;
            }
            catch (Exception) { throw; }
        }
    }
}

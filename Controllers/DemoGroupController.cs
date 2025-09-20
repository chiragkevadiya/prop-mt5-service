using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using MT5ConnectionService.Helper;
using MT5ConnectionService.ViewModels.GroupName;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace MT5ConnectionService.Controllers
{
    public class DemoGroupController : ApiController
    {
        CIMTManagerAPI _managerDemo = CreateDemoManagerHelper.GetManagerDemo();

        public DemoGroupController()
        {

        }

        [HttpGet]
        public GroupListVM GetDemoGroupName()
        {
            try
            {

                var groupNameVMs = new List<GroupNameVM>();
                var groupCommissionLists = new List<GroupCommissionVM>();

                uint totalGroups = _managerDemo.GroupTotal();

                CIMTConGroup cIMTConGroup = _managerDemo.GroupCreate();
                CIMTConCommTier cIMTConCommTier = _managerDemo.GroupTierCreate();
                CIMTConCommission cIMTConCommission = _managerDemo.GroupCommissionCreate();
                CIMTConSymbol cIMTConSymbols = _managerDemo.SymbolCreate();


                for (uint i = 0; i < totalGroups; i++)
                {
                    if (_managerDemo.GroupNext(i, cIMTConGroup) != MTRetCode.MT_RET_OK)
                        continue;

                    uint symbolTotal = cIMTConGroup.SymbolTotal();
                    uint commissionTotal = cIMTConGroup.CommissionTotal();

                    // Process symbols (if needed)
                    for (uint j = 0; j < symbolTotal; j++)
                    {
                        if (_managerDemo.SymbolNext(j, cIMTConSymbols) != MTRetCode.MT_RET_OK)
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
                    CIMTConGroupSymbol GroupSymbol = _managerDemo.GroupSymbolCreate();
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

                //List<GroupNameVM> groupNameVMs = new List<GroupNameVM>();

                //uint totalGroups = _managerDemo.GroupTotal();

                //for (uint i = 0; i < totalGroups; i++)
                //{
                //    CIMTConGroup cIMTConGroup = _managerDemo.GroupCreate();
                //    MTRetCode mTRetCode = _managerDemo.GroupNext(i, cIMTConGroup);

                //    if (mTRetCode == MTRetCode.MT_RET_OK)
                //    {
                //        GroupNameVM groupNameVM = new GroupNameVM()
                //        {
                //            GroupName = cIMTConGroup.Group(),
                //            MarginCall = cIMTConGroup.MarginCall(),
                //            StopOutLevel = cIMTConGroup.MarginStopOut(),
                //            Currency = cIMTConGroup.Currency(),
                //            CurrencyDigits = cIMTConGroup.CurrencyDigits(),
                //        };
                //        groupNameVMs.Add(groupNameVM);
                //    }
                //}

                //GroupListVM groupListVM = new GroupListVM
                //{
                //    GroupTotal = totalGroups,
                //    GroupList = groupNameVMs
                //};

                //return groupListVM;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

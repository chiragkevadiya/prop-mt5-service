//using MetaQuotes.MT5CommonAPI;
//using MetaQuotes.MT5ManagerAPI;
//using Microsoft.Extensions.DependencyInjection;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PropMT5ConnectionService.Services
//{
//    public class TickSink : CIMTTickSink
//    {
//        private readonly CIMTManagerAPI _manager;
//        private readonly IServiceScopeFactory _scopeFactory;
//        private readonly HashSet<long> _pendingAccounts;
//        private readonly object _lock = new object();

//        public TickSink(CIMTManagerAPI manager, IServiceScopeFactory scopeFactory)
//        {
//            _manager = manager;
//            _scopeFactory = scopeFactory;
//            _pendingAccounts = new HashSet<long>();
//        }

//        public override void OnTick(CIMTQuote quote)
//        {
//            // Offload heavy DB logic to background thread
//            Task.Run(() => ProcessAccountsFromTick());
//        }

//        private async Task ProcessAccountsFromTick()
//        {
//            try
//            {
//                // ✅ Step 1: Create array for positions
//                CIMTPositionArray positionArray = _manager.PositionCreateArray();
//                if (positionArray == null)
//                {
//                    Console.WriteLine("Failed to create position array.");
//                    return;
//                }

//                // ✅ Step 2: Get ALL open positions on the server
//                MTRetCode res = _manager.PositionGet(positionArray);
//                if (res != MTRetCode.MT_RET_OK)
//                {
//                    Console.WriteLine($"PositionGet failed: {res}");
//                    return;
//                }

//                // ✅ Step 3: Collect all active account IDs from open positions
//                var activeAccountIds = new List<long>();
//                for (uint i = 0; i < positionArray.Total(); i++)
//                {
//                    CIMTPosition pos = positionArray.Next(i);
//                    if (pos == null)
//                        continue;

//                    activeAccountIds.Add((long)pos.Login());
//                    pos.Release(); // cleanup each position
//                }

//                positionArray.Release(); // cleanup array

//                if (!activeAccountIds.Any())
//                    return;

//                lock (_lock)
//                {
//                    foreach (var acc in activeAccountIds.Distinct())
//                        _pendingAccounts.Add(acc);
//                }

//                // ✅ Step 4: Use DI scope to get LiquidationService
//                using var scope = _scopeFactory.CreateScope();
//                var liquidationService = scope.ServiceProvider.GetRequiredService<LiquidationService>();

//                // ✅ Step 5: Only process pending accounts
//                List<long> accountsToProcess;
//                lock (_lock)
//                {
//                    accountsToProcess = _pendingAccounts.ToList();
//                    _pendingAccounts.Clear();
//                }

//                if (accountsToProcess.Any())
//                {
//                    await liquidationService.CheckAndLiquidateAccounts(accountsToProcess);
//                }
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"TickSink error: {ex.Message}");
//            }
//        }
//    }


//}

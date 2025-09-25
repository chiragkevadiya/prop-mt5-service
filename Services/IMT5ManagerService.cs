using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropMT5ConnectionService.Services
{
    public interface IMT5ManagerService
    {
        CIMTManagerAPI GetManagerInstance(); 
        Dictionary<long, CIMTAccount> GetAccountsDetails(List<long> loginIds);
    }
}

using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using System.Collections.Generic;

namespace PropMT5Service.Services
{
    public interface IMT5ManagerService
    {
        CIMTManagerAPI GetManagerInstance();
        Dictionary<long, CIMTAccount> GetAccountsDetails(List<long> loginIds);
    }
}

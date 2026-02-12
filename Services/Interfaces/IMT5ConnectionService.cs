using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;

namespace PropMT5ConnectionService.Services.Interfaces
{
    public interface IMT5ConnectionService
    {
        CIMTManagerAPI GetLiveManager();
        CIMTManagerAPI GetDemoManager();
        bool IsConnected(bool isDemo = false);
        MTRetCode Reconnect(bool isDemo = false);
    }
}

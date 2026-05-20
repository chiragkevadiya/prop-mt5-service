using MetaQuotes.MT5CommonAPI;
using PropMT5Service.ViewModels;

namespace PropMT5Service.Helpers
{
    public static class MT5UserMapper
    {
        public static Mt5LiveAccountVM MapToLiveAccountVM(CIMTUser user, CIMTAccount account = null)
        {
            var vm = new Mt5LiveAccountVM
            {
                Login = user.Login(),
                FirstName = user.FirstName(),
                LastName = user.LastName(),
                Group = user.Group(),
                Country = user.Country(),
                Leverage = user.Leverage(),
                Status = user.Status(),
                BalancePrevDay = user.BalancePrevDay(),
                EquityPrevDay = user.EquityPrevDay(),
            };

            if (account != null)
            {
                vm.Credit = account.Credit();
                vm.Balance = account.Balance();
                vm.Margin = account.Margin();
                vm.MarginFree = account.MarginFree();
                vm.Profit = account.Profit();
                vm.Commission = 0;
                vm.Equity = account.Equity();
            }
            else
            {
                vm.Credit = user.Credit();
                vm.Balance = user.Balance();
            }

            return vm;
        }

    }
}

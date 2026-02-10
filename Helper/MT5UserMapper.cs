using MetaQuotes.MT5CommonAPI;
using MT5ConnectionService.ViewModels;

namespace MT5ConnectionService.Helper
{
    public static class MT5UserMapper
    {
        public static MtGetAllLiveAccountVM MapToLiveAccountVM(CIMTUser user, CIMTAccount account = null)
        {
            var vm = new MtGetAllLiveAccountVM
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

        public static MtGetAllLiveAccountVM MapToBasicLiveAccountVM(CIMTUser user)
        {
            return new MtGetAllLiveAccountVM
            {
                Login = user.Login(),
                FirstName = user.FirstName(),
                LastName = user.LastName(),
                Group = user.Group(),
                Country = user.Country(),
                Credit = user.Credit(),
                Balance = user.Balance(),
                Leverage = user.Leverage(),
                Status = user.Status()
            };
        }

        public static MtGetAllLiveAccountVM MapToAccountWithMargin(CIMTUser user, CIMTAccount account)
        {
            return new MtGetAllLiveAccountVM
            {
                Login = user.Login(),
                FirstName = user.FirstName(),
                LastName = user.LastName(),
                Group = user.Group(),
                Country = user.Country(),
                Credit = user.Credit(),
                Balance = user.Balance(),
                Leverage = user.Leverage(),
                Status = user.Status(),
                Margin = account.Margin(),
                MarginFree = account.MarginFree(),
                Profit = account.Profit(),
                Equity = account.Equity()
            };
        }
    }
}

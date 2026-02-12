using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5ConnectionService.ViewModels;
using System.Collections.Generic;

namespace PropMT5ConnectionService.Helpers
{
    public static class MT5AccountOperations
    {
        public static Mt5LiveAccountVM GetSingleAccount(CIMTManagerAPI manager, ulong loginId)
        {
            Mt5LiveAccountVM liveAccountVM = new Mt5LiveAccountVM();

            CIMTUser cIMTUser = manager.UserCreate();
            MTRetCode mTRetCode = manager.UserGet(loginId, cIMTUser);

            CIMTAccount cIMTAccountInfo = manager.UserCreateAccount();
            manager.UserAccountGet(loginId, cIMTAccountInfo);

            if (MTRetCode.MT_RET_OK == mTRetCode)
            {
                liveAccountVM = MT5UserMapper.MapToLiveAccountVM(cIMTUser, cIMTAccountInfo);
            }
            else
            {
                liveAccountVM.Status = "Data Not Found.";
                liveAccountVM.MTRetCodeError = mTRetCode;
            }

            cIMTUser.Clear();
            cIMTUser.Release();

            return liveAccountVM;
        }

        public static IEnumerable<Mt5LiveAccountVM> GetAllAccounts(CIMTManagerAPI manager)
        {
            List<Mt5LiveAccountVM> accounts = new List<Mt5LiveAccountVM>();

            CIMTUserArray cIMTUserArray = manager.UserCreateArray();
            MTRetCode mTRetCode = manager.UserGetByGroup("*", cIMTUserArray);

            if (MTRetCode.MT_RET_OK == mTRetCode)
            {
                for (uint i = 0; i < cIMTUserArray.Total(); i++)
                {
                    CIMTUser cIMTUser = cIMTUserArray.Next(i);
                    accounts.Add(MT5UserMapper.MapToBasicLiveAccountVM(cIMTUser));
                }

                cIMTUserArray.Clear();
                cIMTUserArray.Release();
                return accounts;
            }

            return null;
        }

        public static List<Mt5LiveAccountVM> GetAccountsByLoginIds(CIMTManagerAPI manager, List<ulong> loginIds)
        {
            List<Mt5LiveAccountVM> liveAccounts = new List<Mt5LiveAccountVM>();

            foreach (ulong loginId in loginIds)
            {
                CIMTUser cIMTUser = manager.UserCreate();
                MTRetCode mTRetCode = manager.UserGet(loginId, cIMTUser);

                CIMTAccount cIMTAccountInfo = manager.UserCreateAccount();
                manager.UserAccountGet(loginId, cIMTAccountInfo);

                if (MTRetCode.MT_RET_OK == mTRetCode)
                {
                    liveAccounts.Add(MT5UserMapper.MapToAccountWithMargin(cIMTUser, cIMTAccountInfo));

                    cIMTUser.Clear();
                    cIMTUser.Release();
                }
            }

            return liveAccounts;
        }

        public static BaseResponseModel<int> SetAccountActiveStatus(CIMTManagerAPI manager, Mt5AccountStatusVM entity)
        {
            foreach (ulong loginId in entity.LoginId)
            {
                CIMTUser cIMTUser = manager.UserCreate();
                MTRetCode resultCode = manager.UserGet(loginId, cIMTUser);

                if (MTRetCode.MT_RET_OK == resultCode)
                {
                    if (entity.UserStatus)
                    {
                        cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_ENABLED);
                    }
                    else
                    {
                        cIMTUser.Rights(CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED);
                    }

                    manager.UserUpdate(cIMTUser);
                }
            }

            AccountLogHelper.LogAccountStatusChange(entity.LoginId.ToArray(), entity.UserStatus);

            return new BaseResponseModel<int> { Success = true, Message = "Success" };
        }

        public static MTRetCode DepositOrWithdrawBalance(CIMTManagerAPI manager, Mt5DepositBalanceVM entity)
        {
            MTRetCode mTRetCode;
            ulong variable = 0;

            if (entity.Comment == "Withdraw")
            {
                var balance = GetBalanceForLogin(manager, entity.Login);

                if (balance <= 0 || entity.Amount <= 0 || balance < entity.Amount)
                {
                    return MTRetCode.MT_RET_ERROR;
                }

                mTRetCode = manager.DealerBalanceRaw(entity.Login, -entity.Amount, 2, entity.Comment, out variable);
            }
            else
            {
                mTRetCode = manager.DealerBalanceRaw(entity.Login, entity.Amount, 2, entity.Comment, out variable);
            }

            if (MTRetCode.MT_RET_REQUEST_DONE == mTRetCode)
            {
                return mTRetCode;
            }

            return MTRetCode.MT_RET_ERR_NOTFOUND;
        }

        public static double GetBalanceForLogin(CIMTManagerAPI manager, ulong login)
        {
            CIMTUser cIMTUser = manager.UserCreate();
            MTRetCode mTRetCode = manager.UserGet(login, cIMTUser);

            if (MTRetCode.MT_RET_OK == mTRetCode)
            {
                return cIMTUser.Balance();
            }

            return 0;
        }
    }
}

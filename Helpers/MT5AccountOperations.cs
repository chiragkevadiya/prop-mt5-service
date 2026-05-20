using MetaQuotes.MT5CommonAPI;
using MetaQuotes.MT5ManagerAPI;
using PropMT5Service.Constants;
using PropMT5Service.ViewModels;
using System.Collections.Generic;

namespace PropMT5Service.Helpers
{
    public static class MT5AccountOperations
    {
        public static Mt5LiveAccountVM GetSingleAccount(CIMTManagerAPI manager, ulong loginId)
        {
            Mt5LiveAccountVM liveAccountVM = new Mt5LiveAccountVM();

            CIMTUser cIMTUser = manager.UserCreate();
            CIMTAccount cIMTAccountInfo = manager.UserCreateAccount();

            try
            {
                MTRetCode mTRetCode = manager.UserGet(loginId, cIMTUser);
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
            }
            finally
            {
                cIMTUser.Clear();
                cIMTUser.Release();
                cIMTAccountInfo.Release();
            }

            return liveAccountVM;
        }

        public static IEnumerable<Mt5LiveAccountVM> GetAllAccounts(CIMTManagerAPI manager)
        {
            List<Mt5LiveAccountVM> accounts = new List<Mt5LiveAccountVM>();

            CIMTUserArray cIMTUserArray = manager.UserCreateArray();

            try
            {
                MTRetCode mTRetCode = manager.UserGetByGroup(MT5Constants.AccountOperations.WildcardGroup, cIMTUserArray);

                if (MTRetCode.MT_RET_OK == mTRetCode)
                {
                    for (uint i = 0; i < cIMTUserArray.Total(); i++)
                    {
                        CIMTUser cIMTUser = cIMTUserArray.Next(i);
                        accounts.Add(MT5UserMapper.MapToLiveAccountVM(cIMTUser));
                    }
                }
            }
            finally
            {
                cIMTUserArray.Clear();
                cIMTUserArray.Release();
            }

            return accounts;
        }

        public static List<Mt5LiveAccountVM> GetAccountsByLoginIds(CIMTManagerAPI manager, List<ulong> loginIds)
        {
            List<Mt5LiveAccountVM> liveAccounts = new List<Mt5LiveAccountVM>();

            foreach (ulong loginId in loginIds)
            {
                CIMTUser cIMTUser = manager.UserCreate();
                CIMTAccount cIMTAccountInfo = manager.UserCreateAccount();

                try
                {
                    MTRetCode mTRetCode = manager.UserGet(loginId, cIMTUser);
                    manager.UserAccountGet(loginId, cIMTAccountInfo);

                    if (MTRetCode.MT_RET_OK == mTRetCode)
                        liveAccounts.Add(MT5UserMapper.MapToLiveAccountVM(cIMTUser, cIMTAccountInfo));
                }
                finally
                {
                    cIMTUser.Clear();
                    cIMTUser.Release();
                    cIMTAccountInfo.Release();
                }
            }

            return liveAccounts;
        }

        public static BaseResponseModel<int> SetAccountActiveStatus(CIMTManagerAPI manager, Mt5AccountStatusVM entity)
        {
            foreach (ulong loginId in entity.LoginId)
            {
                CIMTUser cIMTUser = manager.UserCreate();

                try
                {
                    MTRetCode resultCode = manager.UserGet(loginId, cIMTUser);

                    if (MTRetCode.MT_RET_OK == resultCode)
                    {
                        cIMTUser.Rights(entity.UserStatus
                            ? CIMTUser.EnUsersRights.USER_RIGHT_ENABLED
                            : CIMTUser.EnUsersRights.USER_RIGHT_TRADE_DISABLED);

                        manager.UserUpdate(cIMTUser);
                    }
                }
                finally
                {
                    cIMTUser.Release();
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

                mTRetCode = manager.DealerBalanceRaw(entity.Login, -entity.Amount, MT5Constants.AccountOperations.DealerBalanceType, entity.Comment, out variable);
            }
            else
            {
                mTRetCode = manager.DealerBalanceRaw(entity.Login, entity.Amount, MT5Constants.AccountOperations.DealerBalanceType, entity.Comment, out variable);
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

            try
            {
                MTRetCode mTRetCode = manager.UserGet(login, cIMTUser);
                return mTRetCode == MTRetCode.MT_RET_OK ? cIMTUser.Balance() : 0;
            }
            finally
            {
                cIMTUser.Release();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.ViewModels
{
    public class MT5LiveOnlineUserActiveVM
    {
        public List<ulong> OnlineTrader { get; set; }
        public List<ulong> InActiveTrader { get; set; }
        public List<ulong> ActiveTrader { get; set; }

        // Constructor to initialize lists and hash sets
        //public MT5LiveOnlineUserActiveVM()
        //{
        //    OnlineTrader = new List<UserLogin>();
        //    InActiveTrader = new List<UserLogin>();
        //    ActiveTrader = new List<UserLogin>();

        //    // HashSets to track unique Login IDs
        //    OnlineTraderIds = new HashSet<ulong>();
        //    InActiveTraderIds = new HashSet<ulong>();
        //    ActiveTraderIds = new HashSet<ulong>();
        //}

        //public List<UserLogin> OnlineTrader { get; set; }
        //public List<UserLogin> InActiveTrader { get; set; }
        //public List<UserLogin> ActiveTrader { get; set; }

        //// HashSets to ensure unique Login IDs
        //private HashSet<ulong> OnlineTraderIds { get; set; }
        //private HashSet<ulong> InActiveTraderIds { get; set; }
        //private HashSet<ulong> ActiveTraderIds { get; set; }

        //// Methods to add UserLogin only if Login is unique
        //public void AddOnlineTrader(UserLogin user)
        //{
        //    if (OnlineTraderIds.Add(user.Login)) // Adds only if Login is not in the HashSet
        //    {
        //        OnlineTrader.Add(user);
        //    }
        //}

        //public void AddInActiveTrader(UserLogin user)
        //{
        //    if (InActiveTraderIds.Add(user.Login)) // Adds only if Login is not in the HashSet
        //    {
        //        InActiveTrader.Add(user);
        //    }
        //}

        //public void AddActiveTrader(UserLogin user)
        //{
        //    if (ActiveTraderIds.Add(user.Login)) // Adds only if Login is not in the HashSet
        //    {
        //        ActiveTrader.Add(user);
        //    }
        //}
    }


    //public class UserLogin
    //{
    //    public ulong Login { get; set; }
    //}
}

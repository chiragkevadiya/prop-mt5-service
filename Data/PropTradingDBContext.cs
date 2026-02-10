using Microsoft.EntityFrameworkCore;
using PropMT5ConnectionService.Models;

namespace PropMT5ConnectionService.Data
{
    // Minimal DbContext required for build - re-create full implementation from original repo as needed.
    public class PropTradingDBContext : DbContext
    {
        public PropTradingDBContext()
        {
        }

        public PropTradingDBContext(DbContextOptions<PropTradingDBContext> options) : base(options)
        {

        }

        public virtual DbSet<UserChallengePhase> UserChallengePhase { get; set; }
        public virtual DbSet<PropAccountMaster> PropAccountMaster { get; set; }
        public virtual DbSet<UserChallengeHistory> UserChallengeHistory { get; set; }
        public virtual DbSet<UserMaster> UserMaster { get; set; }

        // Add other DbSet<> members here if build shows more missing types.
    }
}

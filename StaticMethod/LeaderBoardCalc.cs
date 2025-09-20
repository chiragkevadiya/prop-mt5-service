using MT5ConnectionService.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT5ConnectionService.StaticMethod
{
    public static class LeaderboardCalc
    {
        // Format a TimeSpan like "3h 8m" / "2d 4h"
        public static string FormatDuration(TimeSpan ts)
        {
            if (ts.TotalMinutes < 1) return "0m";
            var parts = new List<string>();
            if (ts.Days > 0) parts.Add($"{ts.Days}d");
            if (ts.Hours > 0) parts.Add($"{ts.Hours}h");
            if (ts.Minutes > 0) parts.Add($"{ts.Minutes}m");
            return string.Join(" ", parts);
        }

        // Current streaks from most recent trades backward
        public static (int win, int loss) CurrentStreaks(IEnumerable<TradeData> closedDealsOrderedByTimeAsc)
        {
            var list = closedDealsOrderedByTimeAsc.OrderBy(d => d.Time1).ToList();
            int win = 0, loss = 0;  

            for (int i = list.Count - 1; i >= 0; i--)
            {
                var p = list[i].Profit;
                if (p > 0)
                {
                    if (loss > 0) break; // currently in a losing streak; stop at first win
                    win++;
                }
                else if (p < 0)
                {
                    if (win > 0) break;  // currently in a winning streak; stop at first loss
                    loss++;
                }
                else
                {
                    // zero profit breaks both streaks
                    break;
                }
            }
            return (win, loss);
        }

        // Average closed-position duration from deals (pair IN/OUT by PositionID)
        public static string AverageClosedDuration(IEnumerable<TradeData> dealsForLogin)
        {
            // group by position and find first IN and last OUT timestamps
            // (Entry: 0 = IN, 1 = OUT in your mapping; adjust if different)
            var groups = dealsForLogin
                .GroupBy(d => d.PositionID)
                .Select(g =>
                {
                    var ins = g.Where(x => x.Entry == 0).OrderBy(x => x.Time1).FirstOrDefault();
                    var outs = g.Where(x => x.Entry == 1).OrderByDescending(x => x.Time1).FirstOrDefault();
                    if (ins == null || outs == null) return (ok: false, dur: TimeSpan.Zero);
                    var dur = outs.Time1 - ins.Time1;
                    if (dur < TimeSpan.Zero) dur = TimeSpan.Zero;
                    return (ok: true, dur);
                })
                .Where(x => x.ok)
                .Select(x => x.dur)
                .ToList();

            if (groups.Count == 0) return "0m";
            var avgTicks = Convert.ToInt64(groups.Average(ts => ts.Value.Ticks));
            return FormatDuration(new TimeSpan(avgTicks));
        }
    }

}

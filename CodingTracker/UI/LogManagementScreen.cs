using CodingTracker.DataAccess;
using CodingTracker.Models;
using System.Text;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class LogManagementScreen
{
    internal static Screen Get(IDataAccess dataAccess)
    {
        const int headerHeight = 3;
        const int footerHeight = 5;// Actual height varies between 3-5, but we'll use a constant for regularity.
        const int promptHeight = 2;
        int lastListUsableHeight = 0;
        int listUsableHeight = 0;
        int skip = 0;

        string header(int _, int usableHeight)
        {
            listUsableHeight = usableHeight - headerHeight - footerHeight - promptHeight;
            if (listUsableHeight != lastListUsableHeight)
            {
                // If the list height changes, we reset the skip (i.e. return to page 1) to keep it simple. What should otherwise happen is a bit unclear.
                skip = 0;
            }
            lastListUsableHeight = listUsableHeight;

            var currentPage = (skip/listUsableHeight) + 1;
            var pages = (int)Math.Ceiling(dataAccess.Count()/(double)listUsableHeight);
            if (pages > 1)
            {
                return $"Coding Sessions (page {currentPage}/{pages})";
            }
            else
            {
                return "Coding Sessions";
            }
        }

        string footer(int _1, int _2)
        {
            const string escHint = "[Esc] to go back to the main menu.";
            var output = "Press ";
            if (skip > 0)
            {
                output += "[PgUp] to go to the previous page,\n";
            }
            if (dataAccess.Count() - skip > listUsableHeight)
            {
                output += "[PgDown] to go to the next page,\n";
            }
            if (skip > 0 || dataAccess.Count() - skip > listUsableHeight)
            {
                output += "or ";
            }
            output += escHint;
            return output;
        }

        string body(int _1, int _2)
        {
            var sessions = GetSubset(dataAccess, skip, listUsableHeight);
            return MakeListString(sessions);
        }

        void pgUp()
        {
            if (skip > 0)
            {
                skip = Math.Max(0 , skip - listUsableHeight);
            }
        }

        void pgDown()
        {
            if (dataAccess.Count() - skip > listUsableHeight)
            {
                skip += listUsableHeight;
            }
        }

        var screen = new Screen(header: header, body: body, footer: footer);
        screen.AddAction(ConsoleKey.D0, () => screen.ExitScreen());
        screen.AddAction(ConsoleKey.PageUp, pgUp);
        screen.AddAction(ConsoleKey.PageDown, pgDown);
        return screen;
    }

    private static List<CodingSession> GetSubset(IDataAccess dataAccess, int skip = 0, int take = int.MaxValue)
    {
        return dataAccess.GetAll().OrderBy(cs => cs.StartTime.Ticks).Skip(skip).Take(take).ToList();
    }

    private static string MakeListString(List<CodingSession> sessions)
    {
        var numberWidth = sessions.Count.ToString().Length;
        var durationStrings = sessions.ConvertAll(cs => DurationString(cs.Duration));
        var durationWidth = durationStrings.Max(ds => ds.Length);
        var sb = new StringBuilder();
        for (int i = 0; i < sessions.Count; i++)
        {
            var session = sessions[i];
            sb.Append((i+1).ToString().PadLeft(numberWidth))
                .Append(": ")
                .Append(durationStrings[i].PadLeft(durationWidth))
                .Append(" @ ")
                .AppendLine(DateRangeString(session.StartTime, session.EndTime));
        }
        return sb.ToString();
    }

    private static string DurationString(TimeSpan duration)
    {
        duration = TimeSpan.FromMinutes(Math.Round(duration.TotalMinutes));
        if (duration.TotalMinutes < 60)
        {
            return $"{duration.Minutes}m";
        }
        else
        {
            return $"{duration.Hours}h{duration.Minutes}m";
        }
    }

    private static string DateRangeString(DateTime d1, DateTime d2)
    {
        if (DateOnly.FromDateTime(d1) != DateOnly.FromDateTime(d2))
        {
            return $"{d1:yyyy-MM-dd}, from {d1:HH:mm:ss} to {d2:HH:mm:ss} on {d2:yyyy-MM-dd}";
        }
        else
        {
            return $"{d1:yyyy-MM-dd}, from {d1:HH:mm:ss} to {d2:HH:mm:ss}";
        }
    }
}

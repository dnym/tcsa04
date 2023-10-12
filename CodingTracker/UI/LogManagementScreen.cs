using CodingTracker.DataAccess;
using System.Text;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class LogManagementScreen
{
    internal static Screen Get(IDataAccess dataAccess)
    {
        int skip = 0;
        int lastTaken = 0;

        static string header(int _1, int _2)
        {
            return "Coding Sessions";
        }

        string footer(int _, int usableHeight)
        {
            const string escHint = "[Esc] to go back to the main menu.";
            var footerHeight = 1 + 2;// +2 due to bar separator and empty line.
            var output = "Press ";
            if (skip > 0)
            {
                output += "[PgUp] to go to the previous page,\n";
                footerHeight++;
            }
            if (dataAccess.Count() - skip > (usableHeight - footerHeight))
            {
                output += "[PgDown] to go to the next page,\n";
            }
            if (skip > 0 || dataAccess.Count() - skip > usableHeight)
            {
                output += "or ";
            }
            output += escHint;
            return output;
        }

        string body(int _, int usableHeight)
        {
            lastTaken = usableHeight;
            return MakeListString(dataAccess, skip, usableHeight);
        }

        void pgUp()
        {
            if (skip > 0)
            {
                skip = Math.Max(0 , skip - lastTaken);
            }
        }

        void pgDown()
        {
            if (dataAccess.Count() - skip > lastTaken)
            {
                skip += lastTaken;
            }
        }

        var screen = new Screen(header: header, body: body, footer: footer);
        screen.AddAction(ConsoleKey.D0, () => screen.ExitScreen());
        screen.AddAction(ConsoleKey.PageUp, pgUp);
        screen.AddAction(ConsoleKey.PageDown, pgDown);
        return screen;
    }

    private static string MakeListString(IDataAccess dataAccess, int skip = 0, int take = int.MaxValue)
    {
        var sessions = dataAccess.GetAll().OrderBy(cs => cs.StartTime.Ticks).Skip(skip).Take(take).ToList();
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

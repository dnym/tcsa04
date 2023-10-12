using CodingTracker.DataAccess;
using System.Text;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class LogManagementScreen
{
    internal static Screen Get(IDataAccess dataAccess)
    {
        static string header(int _1, int _2)
        {
            return "Coding Sessions";
        }

        string body(int _1, int _2)
        {
            var sessions = dataAccess.GetAll().OrderBy(cs => cs.StartTime.Ticks).ToList();
            var sb = new StringBuilder();
            for (int i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                sb.Append(i).Append(": ").Append(DurationString(session.Duration)).Append(" @ ").AppendLine(DateRangeString(session.StartTime, session.EndTime));
            }
            return sb.ToString();
        }

        var screen = new Screen(header: header, body: body);
        screen.AddAction(ConsoleKey.D0, () => screen.ExitScreen());
        return screen;
    }

    private static string DurationString(TimeSpan duration)
    {
        duration = TimeSpan.FromMinutes(Math.Round(duration.TotalMinutes));
        return $"{duration.Hours}h{duration.Minutes}m";
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

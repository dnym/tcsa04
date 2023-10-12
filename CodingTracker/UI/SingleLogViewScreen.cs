using CodingTracker.DataAccess;
using CodingTracker.Models;
using System.Diagnostics;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class SingleLogViewScreen
{
    internal static Screen Get(IDataAccess dataAccess, int id)
    {
        var session = dataAccess.Get(id);
        if (session == null)
        {
            return GetErrorScreen();
        }
        else
        {
            return GetScreen(dataAccess, session);
        }
    }

    private static Screen GetErrorScreen()
    {
        static string body(int _1, int _2)
        {
            return "There is no coding session with that id.\n\nPress any key to return.";
        }
        var screen = new Screen(body: body);
        screen.SetAnyKeyAction(() => screen.ExitScreen());
        return screen;
    }

    private static Screen GetScreen(IDataAccess dataAccess, CodingSession session)
    {
        static string header(int _1, int _2)
        {
            return "Viewing Coding Session";
        }
        static string footer(int _1, int _2)
        {
            return "Press [M] to modify the session,\n[D] to delete,\nor [Esc] to go back to the main menu.";
        }

        string body(int _1, int _2)
        {
            string start = session.StartTime.ToString("yyyy-MM-dd HH:mm:ss");
            string end = session.EndTime.ToString("yyyy-MM-dd HH:mm:ss");
            string duration;
            if (session.Duration.TotalHours >= 1)
            {
                duration = $"{(int)session.Duration.TotalHours}h {session.Duration.Minutes}m {(int)Math.Round(session.Duration.TotalSeconds % 60)}s";
            }
            else if (session.Duration.TotalMinutes >= 1)
            {
                duration = $"{session.Duration.Minutes}m {(int)Math.Round(session.Duration.TotalSeconds % 60)}s";
            }
            else
            {
                duration = $"{(int)Math.Round(session.Duration.TotalSeconds)}s";
            }

            return $"Start: {start}\nEnd: {end}\nDuration: {duration}";
        }

        var screen = new Screen(header: header, footer: footer, body: body);
        screen.AddAction(ConsoleKey.Escape, () => screen.ExitScreen());
        screen.AddAction(ConsoleKey.M, () =>
        {
            Debug.Write($"User wants to modify session with id {session.Id}");
        });
        screen.AddAction(ConsoleKey.D, () =>
        {
            dataAccess.Delete(session.Id);
            screen.ExitScreen();
        });
        return screen;
    }
}

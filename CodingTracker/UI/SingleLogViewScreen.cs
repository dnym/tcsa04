using CodingTracker.DataAccess;
using CodingTracker.Models;
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
            return GetScreen(dataAccess, id);
        }
    }

    private static Screen GetErrorScreen()
    {
        static string Body(int _1, int _2)
        {
            return "There is no coding session with that id.\n\nPress any key to return.";
        }
        var screen = new Screen(body: Body);
        screen.SetAnyKeyAction(() => screen.ExitScreen());
        return screen;
    }

    private static Screen GetScreen(IDataAccess dataAccess, int id)
    {
        static string Header(int _1, int _2)
        {
            return "Viewing Coding Session";
        }

        static string Footer(int _1, int _2)
        {
            return "Press [M] to modify the session,\n[D] to delete,\nor [Esc] to go back to the main menu.";
        }

        string Body(int _1, int _2)
        {
            var session = dataAccess.Get(id)!;
            string start = session.StartTime.ToLocalTime().ToString(Program.mainFullFormat);
            string end = session.EndTime.ToLocalTime().ToString(Program.mainFullFormat);
            string duration = DurationString(session);
            return $"Start: {start}\nEnd: {end}\nDuration: {duration}";
        }

        var screen = new Screen(header: Header, body: Body, footer: Footer);
        screen.AddAction(ConsoleKey.Escape, () => screen.ExitScreen());
        screen.AddAction(ConsoleKey.M, () =>
        {
            var session = dataAccess.Get(id)!;
            var modScreen = SessionLoggingScreen.Get(dataAccess, session);
            modScreen.Show();
        });
        screen.AddAction(ConsoleKey.D, () =>
        {
            dataAccess.Delete(id);
            screen.ExitScreen();
        });
        return screen;
    }

    private static string DurationString(CodingSession session)
    {
        if (session.Duration.TotalHours >= 1)
        {
            return $"{(int)session.Duration.TotalHours}h {session.Duration.Minutes}m {(int)Math.Round(session.Duration.TotalSeconds % 60)}s";
        }
        else if (session.Duration.TotalMinutes >= 1)
        {
            return $"{session.Duration.Minutes}m {(int)Math.Round(session.Duration.TotalSeconds % 60)}s";
        }
        else
        {
            return $"{(int)Math.Round(session.Duration.TotalSeconds)}s";
        }
    }
}

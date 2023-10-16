﻿using CodingTracker.DataAccess;
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
        static string body(int _1, int _2)
        {
            return "There is no coding session with that id.\n\nPress any key to return.";
        }
        var screen = new Screen(body: body);
        screen.SetAnyKeyAction(() => screen.ExitScreen());
        return screen;
    }

    private static Screen GetScreen(IDataAccess dataAccess, int id)
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
            var session = dataAccess.Get(id)!;
            string start = session.StartTime.ToLocalTime().ToString(Program.mainFullFormat);
            string end = session.EndTime.ToLocalTime().ToString(Program.mainFullFormat);
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

        var screen = new Screen(header: header, body: body, footer: footer);
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
}

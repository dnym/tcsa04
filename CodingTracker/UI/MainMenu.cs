using System.Diagnostics;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class MainMenu
{
    internal static Screen Get()
    {
        var screen = new Screen(
            header: (_, _) => "Coding Tracker",
            body: (_, _) => @"1. Log Coding Session
2. Manage Coding Session Logs
0. Quit",
            footer: (_, _) => "Press a number to select.");
        screen.AddAction(ConsoleKey.D1, () => SessionLoggingScreen.Get().Show());
        screen.AddAction(ConsoleKey.D2, () => LogManagementScreen.Get().Show());
        screen.AddAction(ConsoleKey.D0, () => screen.ExitScreen());
        return screen;
    }
}

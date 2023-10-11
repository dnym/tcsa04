using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class SessionLoggingScreen
{
    internal static Screen Get()
    {
        var screen = new Screen();
        screen.AddAction(ConsoleKey.D0, () => screen.ExitScreen());
        return screen;
    }
}

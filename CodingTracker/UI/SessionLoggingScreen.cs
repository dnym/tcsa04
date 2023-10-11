using CodingTracker.DataAccess;
using CodingTracker.Models;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class SessionLoggingScreen
{
    internal static Screen Get(IDataAccess dataAccess)
    {
        const string header = "Log Coding Session";
        const string mainDateFormat = "yyyy-MM-dd";
        string[] dateFormats = { mainDateFormat };
        const string mainTimeFormat = "HH:mm:ss";
        string[] timeFormats = { mainTimeFormat, "HH:mm" };

        DateOnly? startDate = null;
        TimeOnly? startTime = null;
        DateOnly? endDate = null;
        TimeOnly? endTime = null;
        DateTime now = DateTime.UtcNow;

        string body(int _1, int _2)
        {
            // The content of the body depends on which input part is currently being asked for. The ones already entered are repeated for convenience.
            var bodyString = string.Empty;
            var startDatePrompt = "At what date did you start coding? " + (startDate == null ? string.Empty : ((DateOnly)startDate).ToString(mainDateFormat));
            var startTimePrompt = "At what time did you start coding? " + (startTime == null ? string.Empty : ((TimeOnly)startTime).ToString(mainTimeFormat));
            var endDatePrompt = "At what date did you stop coding? " + (endDate == null ? string.Empty : ((DateOnly)endDate).ToString(mainDateFormat));
            var endTimePrompt = "At what time did you stop coding? " + (endTime == null ? string.Empty : ((TimeOnly)endTime).ToString(mainTimeFormat));

            if (startDate == null)
            {
                return $"{startDatePrompt}";
            }
            else if (startTime == null)
            {
                return $"{startDatePrompt}\n{startTimePrompt}";
            }
            else if (endDate == null)
            {
                return $"{startDatePrompt}\n{startTimePrompt}\n\n{endDatePrompt}";
            }
            else
            {
                return $"{startDatePrompt}\n{startTimePrompt}\n\n{endDatePrompt}\n{endTimePrompt}";
            }
        }

        string footer(int _1, int _2)
        {
            // Like the body, the footer depends on what has been input so far. Here, we build a string which has the correct hint information.
            now = DateTime.UtcNow;

            string currentInput;
            string currentFormats;
            string currentData;
            if (startDate == null)
            {
                currentInput = "date";
                currentFormats = string.Join(" or ", dateFormats);
                currentData = now.ToString(mainDateFormat);
            }
            else if (startTime == null)
            {
                currentInput = "time";
                currentFormats = string.Join(" or ", timeFormats);
                currentData = now.ToString(mainTimeFormat);
            }
            else if (endDate == null)
            {
                currentInput = "date";
                currentFormats = string.Join(" or ", dateFormats);
                currentData = now.ToString(mainDateFormat);
            }
            else
            {
                currentInput = "time";
                currentFormats = string.Join(" or ", timeFormats);
                currentData = now.ToString(mainTimeFormat);
            }

            if (endTime == null)
            {
                return @$"Input {currentInput} in the format {currentFormats},
or press [Enter] to use the current {currentInput}: {currentData}
Press [Esc] to cancel insertion.";
            }
            else
            {
                return "Press [Esc] to cancel insertion,\nor any other key to confirm.";
            }
        }

        var screen = new Screen(header: (_, _) => header, body: body, footer: footer);

        void promptHandler(string text)
        {
            // This function is called when the user presses [Enter] to submit their input. We can only tell what they were inputting by looking at what has already been input.
            if (startDate == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    startDate = DateOnly.FromDateTime(now);
                }
                else
                {
                    startDate = ParseDateOnly(text, dateFormats);
                }
            }
            else if (startTime == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    startTime = TimeOnly.FromDateTime(now);
                }
                else
                {
                    startTime = ParseTimeOnly(text, timeFormats);
                }
            }
            else if (endDate == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    endDate = DateOnly.FromDateTime(now);
                }
                else
                {
                    endDate = ParseDateOnly(text, dateFormats);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                {
                    endTime = TimeOnly.FromDateTime(now);
                }
                else
                {
                    endTime = ParseTimeOnly(text, timeFormats);
                }
                if (endTime != null)
                {
                    var newSession = new CodingSession()
                    {
                        StartTime = CombineDateTime(startDate.Value, startTime.Value),
                        EndTime = CombineDateTime(endDate.Value, endTime.Value)
                    };
                    screen.SetPromptAction(null);
                    screen.SetAnyKeyAction(() =>
                    {
                        dataAccess.Insert(newSession);
                        screen.ExitScreen();
                    });
                }
            }
        }
        screen.SetPromptAction(promptHandler);
        screen.AddAction(ConsoleKey.Escape, () => screen.ExitScreen());
        return screen;
    }

    private static DateOnly? ParseDateOnly(string text, string[] formats)
    {
        foreach (var format in formats)
        {
            if (DateOnly.TryParseExact(text, format, out var date))
            {
                return date;
            }
        }
        return null;
    }

    private static TimeOnly? ParseTimeOnly(string text, string[] formats)
    {
        foreach (var format in formats)
        {
            if (TimeOnly.TryParseExact(text, format, out var time))
            {
                return time;
            }
        }
        return null;
    }

    private static DateTime CombineDateTime(DateOnly date, TimeOnly time)
    {
        return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second);
    }
}

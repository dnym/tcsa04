using CodingTracker.DataAccess;
using CodingTracker.Models;
using TCSAHelper.Console;

namespace CodingTracker.UI;

internal static class SessionLoggingScreen
{
    internal static Screen Get(IDataAccess dataAccess, CodingSession? codingSession = null)
    {
        // If codingSession is null, the screen is for creating a new session. Otherwise, it is for modifying an existing session.
        // The body is made to ask one of four questions, depending on which info has been received through the prompt handler.
        // The footer is made to give state-based hints about what to input.
        // A prompt handling action parses the user's input and calls the data access layer to insert/update the session.

        DateOnly? startDate = null;
        TimeOnly? startTime = null;
        DateOnly? endDate = null;
        TimeOnly? endTime = null;
        DateTime now = DateTime.Now;

        string header(int _1, int _2)
        {
            if (codingSession == null)
            {
                return "Log Coding Session";
            }
            else
            {
                return "Modifying Coding Session";
            }
        }

        string body(int _1, int _2)
        {
            // The content of the body depends on which input part is currently being asked for. The ones already entered are repeated for convenience.
            var bodyString = string.Empty;
            var startDatePrompt = "At what date did you start coding? " + (startDate == null ? string.Empty : ((DateOnly)startDate).ToString(Program.mainDateFormat));
            var startTimePrompt = "At what time did you start coding? " + (startTime == null ? string.Empty : ((TimeOnly)startTime).ToString(Program.mainTimeFormat));
            var endDatePrompt = "At what date did you stop coding? " + (endDate == null ? string.Empty : ((DateOnly)endDate).ToString(Program.mainDateFormat));
            var endTimePrompt = "At what time did you stop coding? " + (endTime == null ? string.Empty : ((TimeOnly)endTime).ToString(Program.mainTimeFormat));

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
            now = DateTime.Now;
            var hintStart = codingSession?.StartTime.ToLocalTime() ?? now;
            var hintEnd = codingSession?.EndTime.ToLocalTime() ?? now;

            string currentInput;
            string currentFormats;
            string currentData;
            string[] dateFormats = Program.dateFormats.Select(f => f.ToUpper()).ToArray();
            string[] timeFormats = Program.timeFormats.Select(f => f.ToUpper()).ToArray();
            if (startDate == null)
            {
                currentInput = "date";
                currentFormats = string.Join(" or ", dateFormats);
                currentData = hintStart.ToString(Program.mainDateFormat);
            }
            else if (startTime == null)
            {
                currentInput = "time";
                currentFormats = string.Join(" or ", timeFormats);
                currentData = hintStart.ToString(Program.mainTimeFormat);
            }
            else if (endDate == null)
            {
                currentInput = "date";
                currentFormats = string.Join(" or ", dateFormats);
                currentData = hintEnd.ToString(Program.mainDateFormat);
            }
            else
            {
                currentInput = "time";
                currentFormats = string.Join(" or ", timeFormats);
                currentData = hintEnd.ToString(Program.mainTimeFormat);
            }

            if (endTime == null)
            {
                return @$"Input {currentInput} in the format {currentFormats.ToUpper()},
or press [Enter] to use the {(codingSession == null ? "current" : "stored")} {currentInput}: {currentData}
Press [Esc] to cancel {(codingSession == null ? "insertion" : "modification")}.";
            }
            else
            {
                return $"Press [Esc] to cancel {(codingSession == null ? "insertion" : "modification")},\nor any other key to confirm.";
            }
        }

        var screen = new Screen(header: header, body: body, footer: footer);

        void promptHandler(string text)
        {
            // This function is called when the user presses [Enter] to submit their input. We can only tell what they were inputting by looking at what has already been input.
            string[] dateFormats = Program.dateFormats.Select(f => f.ToUpper()).ToArray();
            string[] timeFormats = Program.timeFormats.Select(f => f.ToUpper()).ToArray();
            if (startDate == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    startDate = DateOnly.FromDateTime(codingSession?.StartTime.ToLocalTime() ?? now);
                }
                else
                {
                    startDate = ParseDateOnly(text, Program.dateFormats);
                }
            }
            else if (startTime == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    startTime = TimeOnly.FromDateTime(codingSession?.StartTime.ToLocalTime() ?? now);
                }
                else
                {
                    startTime = ParseTimeOnly(text, Program.timeFormats);
                }
            }
            else if (endDate == null)
            {
                if (string.IsNullOrEmpty(text))
                {
                    endDate = DateOnly.FromDateTime(codingSession?.EndTime.ToLocalTime() ?? now);
                }
                else
                {
                    endDate = ParseDateOnly(text, Program.dateFormats);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(text))
                {
                    endTime = TimeOnly.FromDateTime(codingSession?.EndTime.ToLocalTime() ?? now);
                }
                else
                {
                    endTime = ParseTimeOnly(text, Program.timeFormats);
                }
                if (endTime != null)
                {
                    var newSession = new CodingSession()
                    {
                        Id = codingSession?.Id ?? -1,
                        StartTime = CombineDateTime(startDate.Value, startTime.Value),
                        EndTime = CombineDateTime(endDate.Value, endTime.Value)
                    };
                    screen.SetPromptAction(null);
                    screen.SetAnyKeyAction(() =>
                    {
                        var overlappingSessions = dataAccess.CheckForOverlap(newSession);
                        if (codingSession != null)
                        {
                            overlappingSessions = overlappingSessions.Where(cs => cs.Id != codingSession.Id).ToList();
                        }
                        if (overlappingSessions.Any())
                        {
                            Console.Beep();
                            var errorScreen = new Screen(body: (_, _) => $"The session you are trying to {(codingSession == null ? "insert" : "modify")},\n\t{newSession.StartTime.ToLocalTime()} - {newSession.EndTime.ToLocalTime()},\n\n{(codingSession == null ? "" : "now ")}overlaps with the following session{(overlappingSessions.Count > 1 ? "s" : "")}:\n{string.Join("\n", overlappingSessions.Select(s => $"\t{s.StartTime.ToLocalTime()} - {s.EndTime.ToLocalTime()}"))}\n\nPress any key to cancel insertion and return to the main menu.");
                            errorScreen.SetAnyKeyAction(() => errorScreen.ExitScreen());
                            errorScreen.Show();
                            screen.ExitScreen();
                            return;
                        }
                        if (codingSession == null)
                        {
                            dataAccess.Insert(newSession);
                        }
                        else
                        {
                            dataAccess.Update(newSession);
                        }
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
        Console.Beep();
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
        Console.Beep();
        return null;
    }

    private static DateTime CombineDateTime(DateOnly date, TimeOnly time)
    {
        return new DateTime(date.Year, date.Month, date.Day, time.Hour, time.Minute, time.Second).ToUniversalTime();
    }
}

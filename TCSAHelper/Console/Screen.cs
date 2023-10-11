using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCSAHelper.Console;
public class Screen
{
    private readonly Func<int, int, string> _header;
    private readonly Func<int, int, string> _body;
    private readonly Func<int, int, string> _footer;
    private readonly IDictionary<ConsoleKey, Action> _actions;
    private Action? _anyKeyAction;
    private Action<string>? _promptHandling;
    private bool _stayInScreen;

    public Action ExitScreen => () => _stayInScreen = false;

    public Screen(Func<int, int, string>? header, Func<int, int, string> body, Func<int, int, string> footer, IDictionary<ConsoleKey, Action>? actions = null, Action? anyKeyAction = null, Action<string>? promptHandling = null)
    {
        Func<int, int, string> nullFunc = (_, _) => "";
        _header = header ?? nullFunc;
        _body = body ?? nullFunc;
        _footer = footer ?? nullFunc;
        _actions = actions ?? new Dictionary<ConsoleKey, Action>();
        _anyKeyAction = anyKeyAction;
        _promptHandling = promptHandling;
    }

    public void AddAction(ConsoleKey consoleKey, Action action)
    {
        _actions.Add(consoleKey, action);
    }

    public void SetAnyKeyAction(Action action)
    {
        _anyKeyAction = action;
    }

    public void SetPromptAction(Action<string> action)
    {
        _promptHandling = action;
    }

    private static string CapStrings(int maxLines, int maxWidth, string text)
    {
        var lines = new List<string>();
        foreach (var line in text.Split('\n'))
        {
            if (lines.Count + 1 > maxLines)
            {
                break;
            }
            else if (line.Length > maxWidth)
            {
                string l = line[..(maxWidth-3)] + "...";
                lines.Add(l);
            }
            else
            {
                lines.Add(line);
            }
        }
        return string.Join('\n', lines);
    }

    private static int CountLines(string text) => text.Split('\n').Length;

    private static int LongestLine(string text) => text.Split('\n').Max(l => l.Length);

    public void Show()
    {
        _stayInScreen = true;
        while (_stayInScreen)
        {
            System.Console.Clear();
            // "- 4" is for having space and a bar between header and body, and between body and footer.
            var (winWidth, winHeight) = (System.Console.WindowWidth - 4, System.Console.WindowHeight);
            var header = CapStrings(winWidth, winHeight, _header(winWidth, winHeight));
            var body = CapStrings(winWidth, winHeight, _body(winWidth, winHeight));
            var footer = CapStrings(winWidth, winHeight, _footer(winWidth, winHeight));
            var barLength = Math.Max(LongestLine(header), LongestLine(footer));
            System.Console.WriteLine(header);
            System.Console.WriteLine(new string('=', barLength) + "\n");
            System.Console.Write(body);
            var (bodyCursorLeft, bodyCursorTop) = (System.Console.CursorLeft, System.Console.CursorTop);
            System.Console.SetCursorPosition(0, Math.Max(0, winHeight - CountLines(footer) - 1));
            System.Console.WriteLine(new string('-', barLength));
            System.Console.Write(footer);
            System.Console.SetCursorPosition(bodyCursorLeft, bodyCursorTop);

            if (_promptHandling == null)
            {
                System.Console.CursorVisible = false;
                var pressedKeyInfo = System.Console.ReadKey(true);
                var pressedKey = pressedKeyInfo.Key;
                if (_actions.TryGetValue(pressedKey, out Action? action))
                {
                    action?.Invoke();
                }
                else
                {
                    _anyKeyAction?.Invoke();
                }
            }
            else
            {
                var takeInput = true;
                string userInput = "";
                int userInputPosition = 0;
                while (takeInput)
                {
                    var pressedKeyInfo = System.Console.ReadKey(true);
                    var pressedKey = pressedKeyInfo.Key;
                    if (_actions.TryGetValue(pressedKey, out Action? action))
                    {
                        action?.Invoke();
                    }
                    else
                    {
                        int newCursorLeft = bodyCursorLeft;
                        int newCursorTop = bodyCursorTop;
                        switch (pressedKey)
                        {
                            // TODO: Ctrl+Left/Right to move by words.
                            case ConsoleKey.Enter:
                                _promptHandling(userInput);
                                userInput = "";
                                userInputPosition = 0;
                                takeInput = false;
                                break;
                            case ConsoleKey.Home:
                                userInputPosition = 0;
                                newCursorLeft = bodyCursorLeft;
                                newCursorTop = bodyCursorTop;
                                break;
                            case ConsoleKey.End:
                                userInputPosition = userInput.Length;
                                newCursorLeft = bodyCursorLeft + userInputPosition;
                                newCursorTop = bodyCursorTop;
                                break;
                            case ConsoleKey.UpArrow:
                            case ConsoleKey.LeftArrow:
                                userInputPosition = Math.Max(0, userInputPosition - 1);
                                newCursorLeft = bodyCursorLeft + userInputPosition;
                                newCursorTop = bodyCursorTop;
                                break;
                            case ConsoleKey.DownArrow:
                            case ConsoleKey.RightArrow:
                                userInputPosition = Math.Min(userInput.Length, userInputPosition + 1);
                                newCursorLeft = bodyCursorLeft + userInputPosition;
                                newCursorTop = bodyCursorTop;
                                break;
                            case ConsoleKey.Delete:
                                if (userInputPosition < userInput.Length)
                                {
                                    userInput = userInput.Remove(userInputPosition, 1);
                                    var currentCursorLeft = bodyCursorLeft + userInputPosition;
                                    if (currentCursorLeft < winWidth)
                                    {
                                        System.Console.SetCursorPosition(currentCursorLeft, System.Console.CursorTop);
                                        Utils.ClearRestOfLine();
                                        var inputLimit = winWidth - currentCursorLeft;
                                        var restOfInput = userInput[userInputPosition..];
                                        restOfInput = restOfInput[..Math.Min(restOfInput.Length, inputLimit)];
                                        System.Console.Write(restOfInput);
                                    }
                                    newCursorLeft = bodyCursorLeft + userInputPosition;
                                    newCursorTop = bodyCursorTop;
                                }
                                break;
                            case ConsoleKey.Backspace:
                                if (userInputPosition > 0)
                                {
                                    userInputPosition--;
                                    userInput = userInput.Remove(userInputPosition, 1);
                                    var currentCursorLeft = bodyCursorLeft + userInputPosition;
                                    if (currentCursorLeft < winWidth)
                                    {
                                        System.Console.SetCursorPosition(currentCursorLeft, System.Console.CursorTop);
                                        Utils.ClearRestOfLine();
                                        var inputLimit = winWidth - currentCursorLeft;
                                        var restOfInput = userInput[userInputPosition..];
                                        restOfInput = restOfInput[..Math.Min(restOfInput.Length, inputLimit)];
                                        System.Console.Write(restOfInput);
                                    }
                                    newCursorLeft = bodyCursorLeft + userInputPosition;
                                    newCursorTop = bodyCursorTop;
                                }
                                break;
                            default:
                                {
                                    userInput = userInput.Insert(userInputPosition, $"{pressedKeyInfo.KeyChar}");
                                    userInputPosition++;
                                    var currentCursorLeft = bodyCursorLeft + userInputPosition;
                                    if (currentCursorLeft < winWidth)
                                    {
                                        System.Console.Write(pressedKeyInfo.KeyChar);
                                        Utils.ClearRestOfLine();
                                        var inputLimit = winWidth - currentCursorLeft;
                                        var restOfInput = userInput[userInputPosition..];
                                        restOfInput = restOfInput[..Math.Min(restOfInput.Length, inputLimit)];
                                        System.Console.Write(restOfInput);
                                    }
                                    newCursorLeft = bodyCursorLeft + userInputPosition;
                                    newCursorTop = bodyCursorTop;
                                }
                                break;
                        }
                        if (newCursorLeft < winWidth)
                        {
                            System.Console.CursorVisible = true;
                            System.Console.SetCursorPosition(newCursorLeft, newCursorTop);
                        }
                        else
                        {
                            System.Console.CursorVisible = false;
                            System.Console.SetCursorPosition(winWidth - 3, newCursorTop);
                            System.Console.Write("...");
                        }
                    }
                }
            }
        }
    }
}

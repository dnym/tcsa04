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
        }
        return string.Join('\n', lines);
    }

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
            System.Console.WriteLine(body);
            System.Console.WriteLine("\n" + new string('-', barLength));
            System.Console.WriteLine(footer);

            var pressedKey = System.Console.ReadKey(true).Key;
            if (_actions.TryGetValue(pressedKey, out Action? action))
            {
                action?.Invoke();
            }
            else
            {
                _anyKeyAction?.Invoke();
            }
        }
    }
}

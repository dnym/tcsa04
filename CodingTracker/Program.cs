using CodingTracker.DataAccess;

namespace CodingTracker;

internal static class Program
{
    static void Main()
    {
        IDataAccess dataAccess = new MemoryStorage();
        UI.MainMenu.Get(dataAccess).Show();
        Console.Clear();
    }
}
using CodingTracker.DataAccess;
using System.Configuration;

namespace CodingTracker;

internal static class Program
{
    const string _defaultConnectionString = "Data Source=CodingTracker.db";

    static void Main()
    {
        string connectionString = ConfigurationManager.AppSettings.Get("ConnectionString") ?? _defaultConnectionString;
        IDataAccess dataAccess = new SqliteStorage(connectionString);
        UI.MainMenu.Get(dataAccess).Show();
        Console.Clear();
    }
}
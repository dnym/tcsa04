﻿using CodingTracker.DataAccess;
using CodingTracker.Models;

namespace CodingTracker;

internal static class Program
{
    static void Main()
    {
        IDataAccess dataAccess = new SqliteStorage("Data Source=CodingTracker.db");
        InsertDummySessions(dataAccess, 50);
        UI.MainMenu.Get(dataAccess).Show();
        Console.Clear();
    }

    private static void InsertDummySessions(IDataAccess dataAccess, int n)
    {
        var random = new Random();
        var months = (int)Math.Ceiling(n / 30d);
        var earliestStartDate = DateTime.UtcNow.AddMonths(-months).Ticks;
        var greatestDiff = DateTime.UtcNow.Ticks - earliestStartDate;
        var shortestSessionLength = TimeSpan.FromMinutes(5).Ticks;
        var longestSessionLength = TimeSpan.FromHours(8).Ticks;
        while (dataAccess.Count() < n)
        {
            var startDate = new DateTime(earliestStartDate + (long)(random.NextDouble() * greatestDiff));
            var randomDuration = new TimeSpan((long)(random.NextDouble() * (longestSessionLength - shortestSessionLength)) + shortestSessionLength);
            var endDate = startDate + randomDuration;
            if (endDate > DateTime.UtcNow)
            {
                endDate = DateTime.UtcNow;
            }
            var session = new CodingSession()
            {
                StartTime = startDate,
                EndTime = endDate,
            };
            if (!dataAccess.CheckForOverlap(session).Any())
            {
                dataAccess.Insert(session);
            }
        }
    }
}
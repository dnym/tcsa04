using CodingTracker.Models;
using System.Diagnostics;
using static CodingTracker.DataAccess.IDataAccess;

namespace CodingTracker.DataAccess;

internal class MemoryStorage : IDataAccess
{
    private readonly List<CodingSession> _sessions = new();
    private int _lastId;

    public void Insert(CodingSession session)
    {
        session = Clone(session);
        session.Id = ++_lastId;
        _sessions.Add(session);
        Debug.WriteLine($"Inserted session {session.Id}: [{session.StartTime}] to [{session.EndTime}]");
    }

    public CodingSession? Get(int id)
    {
        var output = _sessions.Find(s => s.Id == id);
        if (output != null)
        {
            output = Clone(output);
        }
        Debug.WriteLine(output != null ? $"Retrieved session {id}" : $"Failed to retrieve session {id}");
        return output;
    }

    public IList<CodingSession> GetAll(Order order = Order.Ascending, int skip = 0, int limit = int.MaxValue)
    {
        IEnumerable<CodingSession> output;
        if (order == Order.Ascending)
        {
            output = _sessions.OrderBy(s => s.StartTime.Ticks).Skip(skip).Take(limit);
        }
        else
        {
            output = _sessions.OrderByDescending(s => s.StartTime.Ticks).Skip(skip).Take(limit);
        }
        Debug.WriteLine($"Retrieved {output.Count()} sessions");
        return output.ToList();
    }

    public int Count()
    {
        return _sessions.Count;
    }

    public void Update(CodingSession session)
    {
        var index = _sessions.FindIndex(s => s.Id == session.Id);
        if (index != -1)
        {
            _sessions[index] = Clone(session);
            Debug.WriteLine($"Updated session {session.Id}");
        }
        else
        {
            Debug.WriteLine($"Failed to update session {session.Id}");
        }
    }

    public void Delete(int id)
    {
        bool deleted = _sessions.RemoveAll(s => s.Id == id) > 0;
        Debug.WriteLine(deleted ? $"Deleted session {id}" : $"Failed to delete session {id}");
    }

    public IList<CodingSession> CheckForOverlap(CodingSession session)
    {
        return _sessions.Where(cs => Overlaps(cs, session)).Select(Clone).ToList();
    }

    private static CodingSession Clone(CodingSession session)
    {
        return new CodingSession
        {
            Id = session.Id,
            StartTime = session.StartTime,
            EndTime = session.EndTime
        };
    }

    private static bool Overlaps(CodingSession P, CodingSession C)
    {
        return (P.StartTime <= C.StartTime && C.StartTime <= P.EndTime)
            || (P.StartTime <= C.EndTime && C.EndTime <= P.EndTime)
            || (C.StartTime <= P.StartTime && P.StartTime <= C.EndTime)
            || (C.StartTime <= P.EndTime && P.EndTime <= C.EndTime);
    }
}

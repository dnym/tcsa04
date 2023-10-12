using CodingTracker.Models;
using System.Diagnostics;

namespace CodingTracker.DataAccess;

internal class MemoryStorage : IDataAccess
{
    private readonly List<CodingSession> _sessions = new();

    public void Insert(CodingSession session)
    {
        session = Clone(session);
        session.Id = _sessions.Count + 1;
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

    public IEnumerable<CodingSession> GetAll()
    {
        var output = _sessions.Select(Clone);
        Debug.WriteLine($"Retrieved {output.Count()} sessions");
        return output;
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

    public IEnumerable<CodingSession> CheckForOverlap(CodingSession session)
    {
        return _sessions.Where(cs => Overlaps(cs, session)).Select(Clone);
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

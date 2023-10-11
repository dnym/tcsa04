using CodingTracker.Models;

namespace CodingTracker.DataAccess;

internal class MemoryStorage : IDataAccess
{
    private readonly List<CodingSession> _sessions = new();

    public void Insert(CodingSession session)
    {
        _sessions.Add(Clone(session));
    }

    public CodingSession? Get(int id)
    {
        var output = _sessions.Find(s => s.Id == id);
        if (output != null)
        {
            output = Clone(output);
        }
        return output;
    }

    public IEnumerable<CodingSession> GetAll()
    {
        return _sessions.Select(Clone);
    }

    public void Update(CodingSession session)
    {
        var index = _sessions.FindIndex(s => s.Id == session.Id);
        if (index != -1)
        {
            _sessions[index] = Clone(session);
        }
    }

    public void Delete(int id)
    {
        _sessions.RemoveAll(s => s.Id == id);
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
}

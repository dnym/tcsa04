using CodingTracker.Models;

namespace CodingTracker.DataAccess;

internal class SqliteStorage : IDataAccess
{
    private readonly string _connectionString;

    public SqliteStorage(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Insert(CodingSession session)
    {
        throw new NotImplementedException();
    }

    public CodingSession? Get(int id)
    {
        throw new NotImplementedException();
    }

    public IList<CodingSession> GetAll(IDataAccess.Order order = IDataAccess.Order.Ascending, int skip = 0, int limit = int.MaxValue)
    {
        throw new NotImplementedException();
    }

    public int Count()
    {
        throw new NotImplementedException();
    }

    public void Update(CodingSession session)
    {
        throw new NotImplementedException();
    }

    public void Delete(int id)
    {
        throw new NotImplementedException();
    }

    public IList<CodingSession> CheckForOverlap(CodingSession session)
    {
        throw new NotImplementedException();
    }
}

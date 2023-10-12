using CodingTracker.Models;

namespace CodingTracker.DataAccess;

internal interface IDataAccess
{
    void Insert(CodingSession session);
    CodingSession? Get(int id);
    IList<CodingSession> GetAll();
    int Count();
    void Update(CodingSession session);
    void Delete(int id);
    IList<CodingSession> CheckForOverlap(CodingSession session);
}

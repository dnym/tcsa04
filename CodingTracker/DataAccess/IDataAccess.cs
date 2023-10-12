using CodingTracker.Models;

namespace CodingTracker.DataAccess;

internal interface IDataAccess
{
    void Insert(CodingSession session);
    CodingSession? Get(int id);
    IEnumerable<CodingSession> GetAll();
    void Update(CodingSession session);
    void Delete(int id);
    IEnumerable<CodingSession> CheckForOverlap(CodingSession session);
}

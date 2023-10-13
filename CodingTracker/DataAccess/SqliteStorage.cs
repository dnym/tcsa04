using CodingTracker.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace CodingTracker.DataAccess;

internal class SqliteStorage : IDataAccess
{
    private const string _createTableSql = @"CREATE TABLE IF NOT EXISTS ""sessions"" (
	""id"" INTEGER NOT NULL UNIQUE,
	""starttime"" TEXT NOT NULL,
	""endtime"" TEXT NOT NULL,
	PRIMARY KEY(""id"" AUTOINCREMENT)
);";
    private const string _insertSessionSql = "INSERT INTO \"sessions\" (\"starttime\", \"endtime\") VALUES (@starttime, @endtime);";
    private const string _selectSessionSql = "SELECT \"id\", \"starttime\", \"endtime\" FROM \"sessions\" WHERE \"id\" = @id;";
    private const string _selectSubsetSql = "SELECT \"id\", \"starttime\", \"endtime\" FROM \"sessions\" ORDER BY \"starttime\" @order LIMIT @limit OFFSET @skip;";
    private const string _countSessionsSql = "SELECT COUNT(*) FROM \"sessions\";";
    private const string _updateSessionSql = "UPDATE \"sessions\" SET \"starttime\" = @starttime, \"endtime\" = @endtime WHERE \"id\" = @id;";
    private const string _deleteSessionSql = "DELETE FROM \"sessions\" WHERE \"id\" = @id;";
    private const string _overlapTestSql = @"SELECT ""id"", ""starttime"", ""endtime"" FROM ""sessions""
WHERE (""starttime"" <= @starttime AND @starttime <= ""endtime"")
OR (""starttime"" <= @endtime AND @endtime <= ""endtime"")
OR (@starttime <= ""starttime"" AND ""starttime"" <= @endtime)
OR (@starttime <= ""endtime"" AND ""endtime"" <= @endtime)
ORDER BY ""starttime"" ASC;";

    private readonly string _connectionString;

    public SqliteStorage(string connectionString)
    {
        _connectionString = connectionString;

        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to create table: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _createTableSql;

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to create table: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
    }

    public void Insert(CodingSession session)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to insert session: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _insertSessionSql;
        cmd.Parameters.AddWithValue("@starttime", session.StartTime.ToString("O"));
        cmd.Parameters.AddWithValue("@endtime", session.EndTime.ToString("O"));

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to insert session: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
    }

    public CodingSession? Get(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get session with id={id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _selectSessionSql;
        cmd.Parameters.AddWithValue("@id", id);

        CodingSession? output = null;
        try
        {
            using var reader = cmd.ExecuteReader();
            reader.Read();
            if (reader.HasRows)
            {
                var _id = reader.GetInt32(0);
                var startTime = reader.GetString(1);
                var endTime = reader.GetString(2);
                output = new CodingSession
                {
                    Id = _id,
                    StartTime = DateTime.ParseExact(startTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    EndTime = DateTime.ParseExact(endTime, "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                };
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get session with id={id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Session with id={id} has bad data: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
        return output;
    }

    public IList<CodingSession> GetAll(IDataAccess.Order order = IDataAccess.Order.Ascending, int skip = 0, int limit = int.MaxValue)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _selectSubsetSql.Replace("@order", order == IDataAccess.Order.Ascending ? "ASC" : "DESC");
        cmd.Parameters.AddWithValue("@limit", limit);
        cmd.Parameters.AddWithValue("@skip", skip);

        var output = new List<CodingSession>();
        try
        {
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                output.Add(new CodingSession
                {
                    Id = reader.GetInt32(0),
                    StartTime = DateTime.ParseExact(reader.GetString(1), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    EndTime = DateTime.ParseExact(reader.GetString(2), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                });
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Session has bad data: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
        return output;
    }

    public int Count()
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to count sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _countSessionsSql;

        var output = 0;
        try
        {
            if (cmd.ExecuteScalar() is long result)
            {
                output = (int)result;
            }
            else
            {
                Console.WriteLine("Failed to count sessions!\nAborting!");
                Environment.Exit(1);
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to count sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
        return output;
    }

    public void Update(CodingSession session)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to update session with id={session.Id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _updateSessionSql;
        cmd.Parameters.AddWithValue("@id", session.Id);
        cmd.Parameters.AddWithValue("@starttime", session.StartTime.ToString("O"));
        cmd.Parameters.AddWithValue("@endtime", session.EndTime.ToString("O"));

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to update session with id={session.Id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
    }

    public void Delete(int id)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to delete session with id={id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _deleteSessionSql;
        cmd.Parameters.AddWithValue("@id", id);

        try
        {
            cmd.ExecuteNonQuery();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to delete session with id={id}: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
    }

    public IList<CodingSession> CheckForOverlap(CodingSession session)
    {
        using var connection = new SqliteConnection(_connectionString);

        try
        {
            connection.Open();
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get overlapping sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        var cmd = connection.CreateCommand();
        cmd.CommandText = _overlapTestSql;
        cmd.Parameters.AddWithValue("@starttime", session.StartTime.ToString("O"));
        cmd.Parameters.AddWithValue("@endtime", session.EndTime.ToString("O"));

        var output = new List<CodingSession>();
        try
        {
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                output.Add(new CodingSession
                {
                    Id = reader.GetInt32(0),
                    StartTime = DateTime.ParseExact(reader.GetString(1), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind),
                    EndTime = DateTime.ParseExact(reader.GetString(2), "O", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind)
                });
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"Failed to get overlapping sessions: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Session has bad data: {ex.Message}\nAborting!");
            Environment.Exit(1);
        }

        connection.Close();
        return output;
    }
}

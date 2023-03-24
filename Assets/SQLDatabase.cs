using Mono.Data.Sqlite;

public static class SQLDatabase
{
    public static SqliteConnection DatabaseConnection;
    public static void SetupHitTable()
    {
        DatabaseConnection = new SqliteConnection("URI=file:Assets/SQLite/playerdata.db");
        DatabaseConnection.Open();
        using(var command = DatabaseConnection.CreateCommand())
        {
            command.CommandText = "CREATE TABLE IF NOT EXISTS hits(hitCount TEXT)";
            command.ExecuteNonQuery();
            using(var checkCommand = DatabaseConnection.CreateCommand())
            {
                checkCommand.CommandText = "SELECT * FROM hits";
                if(checkCommand.ExecuteReader().HasRows == false)
                {
                    using(var insertCommand = DatabaseConnection.CreateCommand())
                    {
                        insertCommand.CommandText = "INSERT INTO hits VALUES(0)";
                        insertCommand.ExecuteNonQuery();
                    }
                }
            }
        }
    }
    public static int GetHitCount()
    {
        using(var command = DatabaseConnection.CreateCommand())
        {
            command.CommandText = "SELECT hitCount from hits";
            var reader = command.ExecuteReader();
            return int.Parse(reader.GetValue(0).ToString());
        }
    }
    public static void AddHit()
    {
        using(var command = DatabaseConnection.CreateCommand())
        {
            command.CommandText = $"UPDATE hits set hitCount = { GetHitCount() + 1}";
            command.ExecuteNonQuery();
        }
    }
    private static void CloseDatabaseConnection()
    {
        DatabaseConnection.Close();
    }
    public static void ClearHitTable()
    {
        using(var command = DatabaseConnection.CreateCommand())
        {
            command.CommandText = "DELETE FROM hits WHERE hitCount >= 0;";
            command.ExecuteNonQuery();
        }
    }
}

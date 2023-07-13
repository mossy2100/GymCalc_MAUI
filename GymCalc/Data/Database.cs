using SQLite;

namespace GymCalc.Data;

/// <summary>
/// Container for database stuff.
/// </summary>
internal static class Database
{
    /// <summary>
    /// Filename of the database file.
    /// </summary>
    private const string _DB_FILENAME = "GymCalc.db3";

    /// <summary>
    /// Flags used to define the features of database connection.
    /// </summary>
    private const SQLiteOpenFlags _FLAGS =
        // Open the database in read/write mode.
        SQLiteOpenFlags.ReadWrite |
        // Create the database if it doesn't exist.
        SQLiteOpenFlags.Create |
        // Enable multi-threaded database access.
        SQLiteOpenFlags.SharedCache;

    /// <summary>
    /// Full path to the database file.
    /// </summary>
    private static readonly string _DB_PATH =
        Path.Combine(FileSystem.AppDataDirectory, _DB_FILENAME);

    /// <summary>
    /// Single instance of the database (singleton pattern).
    /// </summary>
    private static SQLiteAsyncConnection _database;

    /// <summary>
    /// Get the database connection. Create and initialize it if needed.
    /// </summary>
    internal static SQLiteAsyncConnection GetConnection()
    {
        if (_database == null)
        {
            _database = new SQLiteAsyncConnection(_DB_PATH, _FLAGS);
        }

        return _database;
    }
}

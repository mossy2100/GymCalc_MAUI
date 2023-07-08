using GymCalc.Data.Repositories;
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
    private const string DB_FILENAME = "GymCalc.db3";

    /// <summary>
    /// Full path to the database file.
    /// </summary>
    private static string DB_PATH => Path.Combine(FileSystem.AppDataDirectory, DB_FILENAME);

    /// <summary>
    /// Flags used to define the features of database connection.
    /// </summary>
    private const SQLiteOpenFlags FLAGS =
        // Open the database in read/write mode.
        SQLiteOpenFlags.ReadWrite |
        // Create the database if it doesn't exist.
        SQLiteOpenFlags.Create |
        // Enable multi-threaded database access.
        SQLiteOpenFlags.SharedCache;

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
            _database = new SQLiteAsyncConnection(DB_PATH, FLAGS);
        }

        return _database;
    }

    /// <summary>
    /// Initialize the database and its tables.
    /// </summary>
    internal static async Task Initialize()
    {
        // Ensure the database tables exists and contains some data.
        await PlateRepository.InitializeTable();
        await BarRepository.InitializeTable();
        // await DumbbellRepository.InitializeTable();
    }
}

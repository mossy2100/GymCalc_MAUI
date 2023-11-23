using SQLite;

namespace GymCalc.Data;

/// <summary>
/// Container for database stuff.
/// </summary>
public class Database(IServiceProvider serviceProvider)
{
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
    /// Filename of the database file.
    /// </summary>
    private const string _DB_FILENAME = "GymCalc.db3";

    /// <summary>
    /// Full path to the database file.
    /// </summary>
    private static readonly string _Path =
        Path.Combine(FileSystem.AppDataDirectory, _DB_FILENAME);

    /// <summary>
    /// The database connection field.
    /// </summary>
    private SQLiteAsyncConnection _connection;

    /// <summary>
    /// The database connection property. Create and initialize when needed.
    /// </summary>
    internal SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(_Path, _FLAGS);

    /// <summary>
    /// Given a gym object type name, get the matching repository object.
    /// </summary>
    /// <param name="gymObjectTypeName">The gym object type name.</param>
    /// <returns>The single instance of the matching repository object.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// If the gym object type name is invalid.
    /// </exception>
    internal IGymObjectRepository GetRepo(string gymObjectTypeName)
    {
        var repoType = Type.GetType($"GymCalc.Data.{gymObjectTypeName}Repository");
        if (repoType != null)
        {
            return (IGymObjectRepository)serviceProvider.GetService(repoType);
        }

        throw new ArgumentOutOfRangeException(nameof(gymObjectTypeName),
            "Invalid gym object type name.");
    }
}

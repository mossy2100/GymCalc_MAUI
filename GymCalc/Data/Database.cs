using SQLite;

namespace GymCalc.Data;

/// <summary>
/// Container for database stuff.
/// </summary>
public class Database
{
    private BarRepository _barRepo;

    private PlateRepository _plateRepo;

    private DumbbellRepository _dbRepo;

    private KettlebellRepository _kbRepo;

    public Database(BarRepository barRepo, PlateRepository plateRepo,
        DumbbellRepository dbRepo, KettlebellRepository kbRepo)
    {
        _barRepo = barRepo;
        _plateRepo = plateRepo;
        _dbRepo = dbRepo;
        _kbRepo = kbRepo;
    }

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
    private static readonly string _DbPath =
        Path.Combine(FileSystem.AppDataDirectory, _DB_FILENAME);

    /// <summary>
    /// The database connection.
    /// </summary>
    private SQLiteAsyncConnection _connection;

    /// <summary>
    /// The database connection. Create and initialize it if needed.
    /// </summary>
    internal SQLiteAsyncConnection Connection =>
        _connection ??= new SQLiteAsyncConnection(_DbPath, _FLAGS);

    /// <summary>
    /// Initialize the database.
    /// </summary>
    internal async Task Initialize()
    {
        var barTask = _barRepo.Initialize();
        var plateTask = _plateRepo.Initialize();
        var dumbbellTask = _dbRepo.Initialize();
        var kettlebellTask = _kbRepo.Initialize();
        await Task.WhenAll(new Task[] { barTask, plateTask, dumbbellTask, kettlebellTask });
    }
}

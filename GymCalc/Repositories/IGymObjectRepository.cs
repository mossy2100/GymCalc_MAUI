namespace GymCalc.Repositories;

public interface IGymObjectRepository
{
    public Task InsertDefaults();

    public Task<int> DeleteAll();
}

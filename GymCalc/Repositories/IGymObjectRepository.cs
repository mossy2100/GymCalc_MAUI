namespace GymCalc.Repositories;

internal interface IGymObjectRepository
{
    public Task InsertDefaults();

    public Task<int> DeleteAll();
}

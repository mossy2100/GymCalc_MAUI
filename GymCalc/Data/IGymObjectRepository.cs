namespace GymCalc.Data;

public interface IGymObjectRepository
{
    public Task InsertDefaults();

    public Task<int> DeleteAll();
}

namespace GymCalc.Data;

public interface IGymObjectRepository
{
    internal Task InsertDefaults();

    public Task<bool> DeleteAll();
}

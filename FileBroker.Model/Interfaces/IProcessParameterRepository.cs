namespace FileBroker.Model.Interfaces
{
    public interface IProcessParameterRepository
    {
        string GetValueForParameter(int processId, string parameter);
        ProcessCodeData GetProcessCodes(int processId);
    }
}

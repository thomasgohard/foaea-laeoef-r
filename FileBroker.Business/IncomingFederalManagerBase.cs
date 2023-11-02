namespace FileBroker.Business;

public class IncomingFederalManagerBase
{
    protected APIBrokerList APIs { get; }
    protected RepositoryList DB { get; }
    protected IFileBrokerConfigurationHelper Config { get; }

    protected FoaeaSystemAccess FoaeaAccess { get; }

    public IncomingFederalManagerBase(APIBrokerList apis, RepositoryList repositories,
                                      IFileBrokerConfigurationHelper config)
    {
        APIs = apis;
        DB = repositories;
        Config = config;

        FoaeaAccess = new FoaeaSystemAccess(apis, config.FoaeaLogin);
    }

    protected async Task<FileTableData> GetFileTableData(string flatFileName)
    {
        string fileNameNoCycle = Path.GetFileNameWithoutExtension(flatFileName);

        return await DB.FileTable.GetFileTableDataForFileName(fileNameNoCycle);
    }
}

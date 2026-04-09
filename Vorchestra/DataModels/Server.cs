namespace Vorchestra.Domain.DataModels;

public class Server : BaseEntity
{
    public string Name { get; set; }
    public string IPAddress { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public int Port { get; set; }
    public string Status { get; set; }
    public string DefaultDirectory { get; set; }
    public int TotalRamGb { get; set; }
    public int TotalStorageGb { get; set; }
    public int UsedRamGb { get; set; }
    public long UsedStorageGb { get; set; }
    public double CoreCount { get; set; }

}

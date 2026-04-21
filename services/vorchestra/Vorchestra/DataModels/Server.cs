using Shared.Domain.DataModels;

namespace Vorchestra.Domain.DataModels;

public class Server : BaseEntity
{
    public string Name { get; set; } = null!;
    public string IPAddress { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string Password { get; set; } = null!;
    public int Port { get; set; }
    public string Status { get; set; } = null!;
    public string DefaultDirectory { get; set; } = null!;
    public int TotalRamGb { get; set; }
    public int TotalStorageGb { get; set; }
    public int UsedRamGb { get; set; }
    public long UsedStorageGb { get; set; }
    public double CoreCount { get; set; }

}

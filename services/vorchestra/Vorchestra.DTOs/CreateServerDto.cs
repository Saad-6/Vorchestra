namespace Vorchestra.DTOs;

public class CreateServerDto
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
    public double CoreCount { get; set; }
}

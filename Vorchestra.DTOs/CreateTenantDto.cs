using Microsoft.VisualBasic;

namespace Vorchestra.DTOs;

public class CreateTenantDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public string PhoneNumber { get; set; }
    public string AdminEmail { get; set; }
    public string BusinessEmail { get; set; }
    public string Domain { get; set; }
    public string Slug { get; set; }

}

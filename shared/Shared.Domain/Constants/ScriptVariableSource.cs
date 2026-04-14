namespace Shared.Domain.Constants;

public class ScriptVariableSource
{
    public TenantVariableSource Tenant { get; } = new TenantVariableSource();
    public ServerVariableSource Server { get; } = new ServerVariableSource();
}

public class TenantVariableSource
{
    public const string ID = "Tenant.Id";
    public const string NAME = "Tenant.Name";
    public const string SLUG = "Tenant.Slug";
    public const string PORT = "Tenant.Port";
    public const string DOMAIN = "Tenant.Domain";
    public const string CONNECTION_STRING = "Tenant.ConnectionString";
}

public class ServerVariableSource
{
    public const string ID = "Server.Id";
    public const string NAME = "Server.Name";
    public const string IP_ADDRESS = "Server.IpAddress";
    public const string PORT = "Server.Port";
    public const string USERNAME = "Server.Username";
    public const string PASSWORD = "Server.Password";
}
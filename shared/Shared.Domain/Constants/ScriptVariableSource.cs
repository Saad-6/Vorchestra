namespace Shared.Domain.Constants;

public static class ScriptVariableSource
{
    public static class Tenant
    {
        public const string ID = "Tenant.Id";
        public const string NAME = "Tenant.Name";
        public const string SLUG = "Tenant.Slug";
        public const string PORT = "Tenant.Port";
        public const string DOMAIN = "Tenant.Domain";
        public const string CONNECTION_STRING = "Tenant.ConnectionString";
    }
    public class Server
    {
        public const string ID = "Server.Id";
        public const string NAME = "Server.Name";
        public const string DEFAULT_DIR = "Server.DefaultDirectory";
        public const string IP_ADDRESS = "Server.IpAddress";
        public const string PORT = "Server.Port";
        public const string USERNAME = "Server.Username";
        public const string PASSWORD = "Server.Password";
    }
}




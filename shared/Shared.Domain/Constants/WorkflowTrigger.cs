namespace Shared.Domain.Constants;

public class WorkflowTrigger
{
    public class Project
    {
        public const string TENANT_SUBSCRIBED = "Project.TenantSubscribed";
        public const string TENANT_SUSPENDED = "Project.TenantSuspended";
        public const string TENANT_REACTIVATED = "Project.TenantReactivated";
        public const string CODE_UPDATED = "Project.CodeUpdated";
        public const string MANUAL = "Project.Manual";
    }

    public class Server
    {
        public const string MANUAL = "Server.Manual";
    }
}
namespace Shared.Domain.Constants;

public class Queues
{
    public class Tenant
    {
    }
    public class Script
    {
        public const string ScriptsByIds = "scripts-by-ids";
        public const string ScriptsByGroupId = "scripts-by-group-id";
    }
    public class WorkFlow
    {
        public const string ExecuteWorkFlow = "execute-workflow-queue";
    }
}

using Microsoft.Extensions.Logging;
using Shared.DTO;
using Vbaton.Application.Interfaces;
using Vbaton.Application.Models;
using Vbaton.Domain.DataModels;

namespace Vbaton.Infrastructure.Services;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;
    private readonly VbatonDbContext _db;
    public LogService(ILogger<LogService> logger, VbatonDbContext db)
    {
        _logger = logger;
        _db = db;
    }
    public async Task LogAsync(ExecutionRequestCommand request, Guid? groupId, string output, bool succeeded, List<ScriptOutputModel> scriptOutputs)
    {
        var executionLog = new ExecutionLog
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            ServerId = request.Server?.Id,
            TenantId = request.Tenant?.Id,
            Output = output,
            Succeeded = succeeded
        };
        
        var executionLogDetails = scriptOutputs.Select(so => new ExecutionLogDetail
        {
            Id = Guid.NewGuid(),
            ExecutionLogId = executionLog.Id,
            ScriptId = so.ScriptId,
            Output = so.Output,
            Succeeded = so.Succeeded
        }).ToList();

        _logger.LogInformation("Logging execution result for GroupId: {GroupId}, ServerId: {ServerId}, TenantId: {TenantId}, Succeeded: {Succeeded}", groupId, request.Server?.Id, request.Tenant?.Id, succeeded);
        _logger.LogDebug("Execution output: {Output}", output);

        await _db.AddAsync(executionLog);
        await _db.AddRangeAsync(executionLogDetails);
        await _db.SaveChangesAsync();
    }
}

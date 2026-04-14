using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Xcript.Application.Interfaces;
using Xcript.Domain.DataModels;

namespace Xcript.Infrastructure.Services;

public class ScriptVariableService : IScriptVariableService
{
    private readonly XcriptDbContext _context;
    public ScriptVariableService(XcriptDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<string>> AssignVariableToScriptAsync(Guid scriptId, Guid variableId, CancellationToken cancellationToken = default)
    {
        var validationResponse = await ValidateAndGetScriptVariableAsync(scriptId, variableId, cancellationToken);
        
        if (!validationResponse.Success)
        {
            return new ResponseModel<string>() { Success = false, Message = validationResponse.Message };
        }

        var scriptVariable = validationResponse.Data; 
        
        if (scriptVariable != null)
        {
            return new ResponseModel<string> { Success = false, Message = "Variable already assigned to the script" };
        }

        var newScriptVariable = new ScriptVariable
        {
            Id = Guid.NewGuid(),
            ScriptId = scriptId,
            VariableId = variableId,
            CreatedAt = DateTime.UtcNow,
        };

        await _context.AddAsync(newScriptVariable);

        await _context.SaveChangesAsync();

        return new ResponseModel<string> { Success = true, Message = "Variable assigned to the script successfully." };
    }

    public async Task<ResponseModel<string>> RemoveVariableFromScriptAsync(Guid scriptId, Guid variableId, CancellationToken cancellationToken = default)
    {
        var validationResponse = await ValidateAndGetScriptVariableAsync(scriptId, variableId, cancellationToken);

        if (!validationResponse.Success)
        {
            return new ResponseModel<string>() { Success = false, Message = validationResponse.Message };
        }

        var scriptVariable= validationResponse.Data;

        if(scriptVariable == null)
        {
            return new ResponseModel<string> { Success = false, Message = "Variable is not assigned to the script" };
        }

        _context.ScriptVariables.Remove(scriptVariable);

        await _context.SaveChangesAsync();

        return new ResponseModel<string> { Success = true, Message = "Variable removed from the script successfully." };

    }

    private async Task<ResponseModel<ScriptVariable>> ValidateAndGetScriptVariableAsync(Guid scriptId, Guid variableId, CancellationToken cancellationToken = default)
    {
        var script = await _context.Scripts.FindAsync(new object[] { scriptId }, cancellationToken);

        if (script == null)
        {
            return new ResponseModel<ScriptVariable>() { Success = false, Message = "Script not found." };
        }
        var variable = await _context.Variables.FindAsync(new object[] { variableId }, cancellationToken);

        if (variable == null)
        {
            return new ResponseModel<ScriptVariable>() { Success = false, Message = "Variable not found." };
        }

        var scriptVariableExists = await _context.ScriptVariables.Where(m => m.ScriptId == scriptId && m.VariableId == variableId).FirstOrDefaultAsync();

        return new ResponseModel<ScriptVariable> { Success = true, Message = "Validation successful.", Data = scriptVariableExists };
    }
}

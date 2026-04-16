using Microsoft.Extensions.Logging;
using Renci.SshNet;
using Shared.Application.Models;
using Shared.Contracts.ResponseModels;
using Vbaton.Application.Interfaces;
using Vbaton.Application.Models;

namespace Vbaton.Infrastructure.Services;

public class SshService : ISshService
{
    private readonly ILogger<SshService> _logger;

    public SshService(ILogger<SshService> logger)
    {
        _logger = logger;
    }

    public async Task<ResponseModel<List<ScriptOutputModel>>> ExecuteCommandsAsync(NormalizedExecutionRequest request)
    {
        var server = request.Server;
        var scripts = request.Scripts ?? [];
        var scriptOutputs = new List<ScriptOutputModel>();

        _logger.LogInformation("Connecting to server {IpAddress}:{Port} as {Username}",
            server.IpAddress, server.Port, server.Username);

        using var sshClient = new SshClient(server.IpAddress, server.Port, server.Username, server.Password);
        using var sftpClient = new SftpClient(server.IpAddress, server.Port, server.Username, server.Password);

        try
        {
            sshClient.Connect();
            sftpClient.Connect();

            _logger.LogInformation("Connected to {IpAddress}. Executing {Count} script(s)",
                server.IpAddress, scripts.Count);

            foreach (var script in scripts.OrderBy(s => s.Order))
            {
                var output = await ExecuteScriptAsync(sshClient, sftpClient, script, server.DefaultDirectory);
                scriptOutputs.Add(output);

                if (!output.Succeeded)
                {
                    _logger.LogWarning("Script {ScriptId} ({Name}) failed. Halting execution",
                        script.Id, script.Name);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SSH execution failed for server {IpAddress}", server.IpAddress);
            return new ResponseModel<List<ScriptOutputModel>>
            {
                Success = false,
                Message = $"SSH connection failed: {ex.Message}",
                Data = scriptOutputs
            };
        }
        finally
        {
            if (sftpClient.IsConnected) sftpClient.Disconnect();
            if (sshClient.IsConnected) sshClient.Disconnect();
        }

        var overallSuccess = scriptOutputs.All(s => s.Succeeded);
        return new ResponseModel<List<ScriptOutputModel>>
        {
            Success = overallSuccess,
            Message = overallSuccess ? "All scripts executed successfully" : "One or more scripts failed",
            Data = scriptOutputs
        };
    }

    private async Task<ScriptOutputModel> ExecuteScriptAsync(SshClient sshClient, SftpClient sftpClient, ScriptResponse script, string? workingDirectory = null)
    {
        var tempPath = $"/tmp/vbaton_{Guid.NewGuid():N}.sh";

        _logger.LogInformation("Executing script {ScriptId} ({Name})", script.Id, script.Name);

        try
        {
            // Prepend a cd to the server's default directory as the first line of the script.
            // Each CreateCommand opens a new channel, so a standalone cd wouldn't persist.
            // Embedding it here means it runs once at script start; subsequent cd commands
            // inside the script are free to navigate wherever they need.
            var scriptContent = string.IsNullOrWhiteSpace(workingDirectory)
                ? $"set -e\n{script.Content}"
                : $"set -e\ncd \"{workingDirectory}\"\n{script.Content}";

            // Upload to a temp file so multi-line scripts and special characters are handled safely
            using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(scriptContent));
            await Task.Run(() => sftpClient.UploadFile(stream, tempPath));

            await Task.Run(() => sshClient.CreateCommand($"chmod +x {tempPath}").Execute());

            var cmd = sshClient.CreateCommand($"bash {tempPath}");
            var stdout = await Task.Run(() => cmd.Execute());
            var stderr = cmd.Error;
            var exitCode = cmd.ExitStatus;

            var combinedOutput = string.IsNullOrWhiteSpace(stderr)
                ? stdout
                : $"{stdout}\n[stderr]: {stderr}";

            _logger.LogInformation("Script {ScriptId} ({Name}) finished with exit code {ExitCode}",
                script.Id, script.Name, exitCode);

            return new ScriptOutputModel
            {
                ScriptId = script.Id,
                Output = combinedOutput.Trim(),
                Succeeded = exitCode == 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to execute script {ScriptId} ({Name})", script.Id, script.Name);
            return new ScriptOutputModel
            {
                ScriptId = script.Id,
                Output = ex.Message,
                Succeeded = false
            };
        }
        finally
        {
            try { await Task.Run(() => sftpClient.DeleteFile(tempPath)); }
            catch { /* best-effort cleanup */ }
        }
    }
}

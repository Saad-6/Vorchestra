using Microsoft.EntityFrameworkCore;
using Shared.Application.Models;
using Shared.DTO;
using Vorchestra.Application.Interfaces;
using Vorchestra.Domain.DataModels;
using Vorchestra.DTOs;

namespace Vochestra.Infrastructure.Services;

public class ServerService : IServerService
{
    private readonly VorchestraDbContext _context;
    public ServerService(VorchestraDbContext context)
    {
        _context = context;
    }
    public async Task<ResponseModel<Guid>> CreateServerAsync(CreateServerDto server, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.Servers.AnyAsync(s => s.Name == server.Name, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A server with the same name already exists.",
                Data = Guid.Empty
            };
        }
        var ipAddressCheck = await _context.Servers.AnyAsync(s => s.IPAddress == server.IPAddress, cancellationToken);
        if (ipAddressCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A server with the same IP address already exists.",
                Data = Guid.Empty
            };
        }

        var newServer = new Server
        {
            Id = Guid.NewGuid(),
            Name = server.Name,
            IPAddress = server.IPAddress,
            UserName = server.UserName,
            Password = server.Password,
            Port = server.Port,
            DefaultDirectory = server.DefaultDirectory,
            TotalRamGb = server.TotalRamGb,
            TotalStorageGb = server.TotalStorageGb,
            CoreCount = server.CoreCount,
            Status = server.Status,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await _context.Servers.AddAsync(newServer, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Server created successfully.",
            Data = newServer.Id
        };
    }

    public async Task<ResponseModel<string>> DeleteServerAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var server = await _context.Servers.FindAsync(serverId, cancellationToken);
        if (server == null)
        {
            return new ResponseModel<string>
            {
                Success = false,
                Message = "Server not found.",
                Data = null
            };
        }

        _context.Servers.Remove(server);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<string>
        {
            Success = true,
            Message = "Server deleted successfully.",
            Data = server.Id.ToString()
        };
    }

    public async Task<PaginatedResponseModel<ServerViewDto>> GetPaginatedServersAsync(FilterModel filter, CancellationToken cancellationToken = default)
    {
        var serversQuery = _context.Servers.AsQueryable();

        serversQuery = filter.ApplyFilters(serversQuery);

        var totalCount = await serversQuery.CountAsync(cancellationToken);

        var servers = await serversQuery
            .OrderBy(s => s.Name)
            .Select(s => new ServerViewDto
            {
                Id = s.Id,
                Name = s.Name,
                IPAddress = s.IPAddress,
                UserName = s.UserName,
                Port = s.Port,
                Status = s.Status,
                DefaultDirectory = s.DefaultDirectory,
                TotalRamGb = s.TotalRamGb,
                TotalStorageGb = s.TotalStorageGb,
                UsedRamGb = s.UsedRamGb,
                UsedStorageGb = s.UsedStorageGb,
                CoreCount = s.CoreCount
            })
            .ToListAsync(cancellationToken);

        return new PaginatedResponseModel<ServerViewDto>
        {
            Success = true,
            Message = "Servers retrieved successfully.",
            Data = servers,
            TotalCount = totalCount,
            PageNumber = filter.Page,
            PageSize = filter.PageSize,
        };
    }

    public async Task<ResponseModel<ServerContextDto>> GetServerContextByIdAsync(Guid serverId, CancellationToken cancellationToken = default)
    {
        var server = await _context.Servers.FindAsync(serverId, cancellationToken);
        
        if (server == null)
        {
            return new ResponseModel<ServerContextDto>
            {
                Success = false,
                Message = "Server not found.",
                Data = null
            };
        }

        var serverContext = new ServerContextDto
        {
            Id = server.Id,
            Name = server.Name,
            IpAddress = server.IPAddress,
            UserName = server.UserName,
            Port = server.Port,
            DefaultDirectory = server.DefaultDirectory,
            Password = server.Password,
        };

        return new ResponseModel<ServerContextDto>
        {
            Success = true,
            Message = "Server context retrieved successfully.",
            Data = serverContext
        };
    }

    public async Task<ResponseModel<Guid>> UpdateServerAsync(UpdateServerDto server, CancellationToken cancellationToken = default)
    {
        var nameCheck = await _context.Servers.AnyAsync(s => s.Name == server.Name && s.Id != server.Id, cancellationToken);
        if (nameCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A server with the same name already exists.",
                Data = Guid.Empty
            };
        }
        var ipAddressCheck = await _context.Servers.AnyAsync(s => s.IPAddress == server.IPAddress && s.Id != server.Id, cancellationToken);
        if (ipAddressCheck)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "A server with the same IP address already exists.",
                Data = Guid.Empty
            };
        }

        var existingServer = await _context.Servers.FindAsync(server.Id, cancellationToken);
        if (existingServer == null)
        {
            return new ResponseModel<Guid>
            {
                Success = false,
                Message = "Server not found.",
                Data = Guid.Empty
            };
        }

        existingServer.Name = server.Name;
        existingServer.IPAddress = server.IPAddress;
        existingServer.UserName = server.UserName;
        existingServer.Password = server.Password;
        existingServer.Port = server.Port;
        existingServer.DefaultDirectory = server.DefaultDirectory;
        existingServer.TotalRamGb = server.TotalRamGb;
        existingServer.TotalStorageGb = server.TotalStorageGb;
        existingServer.CoreCount = server.CoreCount;
        existingServer.Status = server.Status;
        existingServer.UpdatedAt = DateTimeOffset.UtcNow;

        _context.Servers.Update(existingServer);
        await _context.SaveChangesAsync(cancellationToken);

        return new ResponseModel<Guid>
        {
            Success = true,
            Message = "Server updated successfully.",
            Data = existingServer.Id
        };
    }
}
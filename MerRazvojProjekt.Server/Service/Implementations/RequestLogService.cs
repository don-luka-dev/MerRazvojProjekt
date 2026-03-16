using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models.Dto;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace MerRazvojProjekt.Server.Service.Implementations
{
    public class RequestLogService : IRequestLogService
    {
        private readonly ApplicationDbContext _dbContext;

        public RequestLogService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<RequestLogDto>> GetAllAsync()
        {
            return await _dbContext.RequestLogs
                .AsNoTracking()
                .OrderByDescending(x => x.TimeStamp)
                .Select(x => new RequestLogDto
                {
                    Id = x.Id,
                    HttpMethod = x.HttpMethod,
                    Path = x.Path,
                    ResponseTimeMs = x.ResponseTimeMs,
                    TimeStamp = x.TimeStamp
                })
                .ToListAsync();
        }
    }
}

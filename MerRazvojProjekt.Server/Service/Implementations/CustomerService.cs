using FluentValidation;
using Mapster;
using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Models;
using MerRazvojProjekt.Server.Models.Dto;
using MerRazvojProjekt.Server.Models.DTO;
using MerRazvojProjekt.Server.Extensions;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerRazvojProjekt.Server.Service.Implementations
{
    public class CustomerService(ApplicationDbContext dbContext,IValidator<UpsertCustomerDto> validator) : ICustomerService
    {
        private readonly ApplicationDbContext dbContext = dbContext;
        private readonly IValidator<UpsertCustomerDto> validator = validator;

        public async Task<GetCustomerDto> AddAsync(UpsertCustomerDto dto)
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            if (await dbContext.Customers.AnyAsync(c => c.Email == dto.Email))
                throw new InvalidOperationException("Email already exists");

            var customer = dto.Adapt<Customer>();

            customer.CreatedAt = DateTime.UtcNow;
            customer.LastModifiedAt = null;
            customer.IsActive = true;

            await dbContext.Customers.AddAsync(customer);
            await dbContext.SaveChangesAsync();

            return customer.Adapt<GetCustomerDto>();
        }

        public async Task<PagedResultDto<GetCustomerDto>> GetAllAsync(CustomerFilterDto query)
        {
            var customersQuery = dbContext.Customers
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Name))
            {
                var nameFilter = query.Name.Trim();

                customersQuery = customersQuery.Where(c =>
                    EF.Functions.Like(c.FirstName, $"%{nameFilter}%") ||
                    EF.Functions.Like(c.LastName, $"%{nameFilter}%"));
            }

            if (!string.IsNullOrWhiteSpace(query.City))
            {
                var cityFilter = query.City.Trim();

                customersQuery = customersQuery.Where(c =>
                    EF.Functions.Like(c.City, $"%{cityFilter}%"));
            }

            if (!string.IsNullOrWhiteSpace(query.Country))
            {
                var countryFilter = query.Country.Trim();

                customersQuery = customersQuery.Where(c =>
                    EF.Functions.Like(c.Country, $"%{countryFilter}%"));
            }

            if (query.IsActive.HasValue)
            {
                customersQuery = customersQuery.Where(c =>
                    c.IsActive == query.IsActive.Value);
            }

            customersQuery = (query.SortBy?.Trim().ToLower(), query.SortDirection?.Trim().ToLower()) switch
            {
                ("firstname", "desc") => customersQuery.OrderByDescending(c => c.FirstName).ThenBy(c => c.Id),
                ("firstname", _) => customersQuery.OrderBy(c => c.FirstName).ThenBy(c => c.Id),

                ("lastname", "desc") => customersQuery.OrderByDescending(c => c.LastName).ThenBy(c => c.Id),
                ("lastname", _) => customersQuery.OrderBy(c => c.LastName).ThenBy(c => c.Id),

                ("createdat", "desc") => customersQuery.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
                ("createdat", _) => customersQuery.OrderBy(c => c.CreatedAt).ThenBy(c => c.Id),

                ("id", "desc") => customersQuery.OrderByDescending(c => c.Id),
                _ => customersQuery.OrderBy(c => c.Id)
            };

            var resultQuery = customersQuery.Select(c => new GetCustomerDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                Phone = c.Phone,
                City = c.City,
                Country = c.Country,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                LastModifiedAt = c.LastModifiedAt
            });

            return await resultQuery.ToPagedResultAsync(query.PageNumber, query.PageSize);
        }

        public async Task<GetCustomerDto?> GetByIdAsync(int id)
        {
            var customer = await dbContext.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            return customer?.Adapt<GetCustomerDto>();   
        }

        public async Task<bool> SoftDeleteAsync(int id)
        {
            var customer = await dbContext.Customers.FindAsync(id);
            if (customer == null) return false;

            customer.IsActive = false;
            customer.LastModifiedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();
            return true;

        }

        public async Task<GetCustomerDto?> UpdateAsync(int id, UpsertCustomerDto dto)
        {
            var customer = await dbContext.Customers.FindAsync(id);

            if (customer == null)
                return null;

            if (await dbContext.Customers.AnyAsync(c => c.Email == dto.Email && c.Id != id))
                throw new InvalidOperationException("Email already exists");

            dto.Adapt(customer);

            customer.LastModifiedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            return customer.Adapt<GetCustomerDto>();
        }

        public async Task<int> BulkDeactivateAsync(IEnumerable<int> customerIds)
        {
            if (customerIds is null)
                throw new ArgumentNullException(nameof(customerIds));

            var distinctIds = customerIds
                .Distinct()
                .ToList();

            if (distinctIds.Count == 0)
                throw new ArgumentException("At least one customer ID must be provided.", nameof(customerIds));

            if (distinctIds.Count > 1000)
                throw new ArgumentException("A maximum of 1000 customer IDs is allowed.", nameof(customerIds));

            var now = DateTime.UtcNow;

            var affectedRows = await dbContext.Customers
                .Where(c => distinctIds.Contains(c.Id) && c.IsActive)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(c => c.IsActive, false)
                    .SetProperty(c => c.LastModifiedAt, now));

            return affectedRows;
        }


        public async Task<StatsDto> GetStatsAsync()
        {
            var totalCount = await dbContext.Customers
                 .AsNoTracking()
                 .CountAsync();

            var activeCount = await dbContext.Customers
                .AsNoTracking()
                .CountAsync(c => c.IsActive);

            var topCities = await dbContext.Customers
                .AsNoTracking()
                .GroupBy(c => c.City)
                .Select(g => new CountDto
                {
                    City = g.Key,
                    Count = g.Count()
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.City)
                .Take(5)
                .ToListAsync();

            return new StatsDto
            {
                TotalCount = totalCount,
                ActiveCount = activeCount,
                InactiveCount = totalCount - activeCount,
                TopCities = topCities
            };
        }
    }
}

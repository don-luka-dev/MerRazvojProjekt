using FluentValidation;
using Mapster;
using MerRazvojProjekt.Server.Data;
using MerRazvojProjekt.Server.Extensions;
using MerRazvojProjekt.Server.Models;
using MerRazvojProjekt.Server.Models.Dto.CarDto;
using MerRazvojProjekt.Server.Models.Dto.CustomerDto;
using MerRazvojProjekt.Server.Models.Dto.MiscDto;
using MerRazvojProjekt.Server.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MerRazvojProjekt.Server.Service.Implementations
{
    public class CarService(ApplicationDbContext dbContext, IValidator<UpsertCarDto> validator) : ICarService
    {
        private readonly ApplicationDbContext dbContext = dbContext;
        private readonly IValidator<UpsertCarDto> validator = validator;


        public async Task<GetCarDto> AddCarAsync(UpsertCarDto dto)
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var car = dto.Adapt<Car>();

            await dbContext.Cars.AddAsync(car);

            await dbContext.SaveChangesAsync();

            return car.Adapt<GetCarDto>();
        }

        public async Task<int> BulkDeactivateAsync(IEnumerable<int> carIds)
        {
            ArgumentNullException.ThrowIfNull(carIds);

            var distinctIds = carIds
                .Distinct()
                .ToList();
            
            if (distinctIds.Count == 0)
                throw new ArgumentException("At least one car ID must be provided.", nameof(carIds));

            if (distinctIds.Count > 1000)
                throw new ArgumentException("A maximum of 1000 car IDs is allowed.", nameof(carIds));

            var affectedRows = await dbContext.Cars
                .Where(c => distinctIds.Contains(c.Id) && c.IsActive)
                .ExecuteUpdateAsync(c => c
                    .SetProperty(car => car.IsActive, false));

            return affectedRows;
        }

        public async Task<PagedResultDto<GetCarDto>> GetAllCarsAsync(CarFilterDto query)
        {
            var carsQuery = dbContext.Cars
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrEmpty(query.Make))
            {
                var makeFilter = query.Make.Trim();

                carsQuery = carsQuery.Where(c =>
                EF.Functions.Like(c.Make, $"%{makeFilter}%"));
            }
            if (!string.IsNullOrEmpty(query.Model))
            {
                var modelFilter = query.Model.Trim();

                carsQuery = carsQuery.Where(c =>
                EF.Functions.Like(c.Model, $"%{modelFilter}%"));
            }
            if (query.Year.HasValue)
            {
                carsQuery = carsQuery.Where(c =>
                c.Year == query.Year.Value);
            }
            if (!string.IsNullOrEmpty(query.Color))
            {
                var colorFilter = query.Color.Trim();

                carsQuery = carsQuery.Where(c =>
                EF.Functions.Like(c.Color, $"%{colorFilter}%"));
            }
            if (query.PriceMin.HasValue)
            {
                carsQuery = carsQuery.Where(c =>
                c.Price >= query.PriceMin.Value);
            }

            if (query.PriceMax.HasValue)
            {
                carsQuery = carsQuery.Where(c =>
                c.Price <= query.PriceMax.Value);
            }
            if (query.IsActive.HasValue)
            {
                carsQuery = carsQuery.Where(c =>
                c.IsActive == query.IsActive.Value);
            }

            // Sorting
            carsQuery = (query.SortBy?.Trim().ToLower(), query.SortDirection?.Trim().ToLower()) switch
            {
                ("price", "desc") => carsQuery.OrderByDescending(c => c.Price).ThenBy(c => c.Id),
                ("price", _) => carsQuery.OrderBy(c => c.Price).ThenBy(c => c.Id),

                ("year", "desc") => carsQuery.OrderByDescending(c => c.Year).ThenBy(c => c.Id),
                ("year", _) => carsQuery.OrderBy(c => c.Year).ThenBy(c => c.Id),

                ("id", "desc") => carsQuery.OrderByDescending(c => c.Id),
                _ => carsQuery.OrderBy(c => c.Id)
            };

            var resultQuery = carsQuery
                .Select(c => new GetCarDto
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    Year = c.Year,
                    Color = c.Color,
                    Price = c.Price,
                    IsActive = c.IsActive
                });

            return await resultQuery.ToPagedResultAsync(query.PageNumber, query.PageSize);
        }

        public async Task<GetCarDto> GetCarByIdAsync(int id)
        {
            var car = await dbContext.Cars
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id)
                ?? throw new KeyNotFoundException($"Car with ID {id} not found.");

            return car.Adapt<GetCarDto>();
        }

        public async Task<bool> SoftDeleteCarAsync(int id)
        {
            var car = await dbContext.Cars.FindAsync(id);
            if (car == null) return false;

            car.IsActive = false;
            await dbContext.SaveChangesAsync();

            return true;

        }

        public async Task<GetCarDto> UpdateCarAsync(int id, UpsertCarDto dto)
        {
            var validationResult = await validator.ValidateAsync(dto);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }

            var car = await dbContext.Cars
                .FindAsync(id)
                ?? throw new KeyNotFoundException($"Car with ID {id} not found.");

            dto.Adapt(car);

            await dbContext.SaveChangesAsync();

            return car.Adapt<GetCarDto>();
        }
    }
}

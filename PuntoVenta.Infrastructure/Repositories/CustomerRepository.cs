using Microsoft.EntityFrameworkCore;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Domain.Entities;
using PuntoVenta.Infrastructure.Persistence;

namespace PuntoVenta.Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _appDbContext;

    public CustomerRepository(AppDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
        => await _appDbContext.Customers
            .AsNoTracking()
            .ToListAsync();

    public async Task<IEnumerable<Customer>> SearchByLastNameAsync(string lastName)
        => await _appDbContext.Customers
            .AsNoTracking()
            .Where(customer => customer.LastName.Contains(lastName))
            .ToListAsync();

    public async Task<Customer?> GetByIdAsync(int customerId)
        => await _appDbContext.Customers
            .AsNoTracking()
            .FirstOrDefaultAsync(customer => customer.CustomerId == customerId);

    public async Task<Customer> AddAsync(Customer newCustomer)
    {
        await _appDbContext.Customers.AddAsync(newCustomer);
        await _appDbContext.SaveChangesAsync();
        return newCustomer;
    }

    /// <inheritdoc />
    public async Task<(IEnumerable<Customer> Items, int TotalCount)> SearchPagedAsync(
        int?    customerId,
        string? documentNumber,
        string? lastName,
        int     page,
        int     pageSize)
    {
        // Start from the full set (AsNoTracking for read-only queries)
        var query = _appDbContext.Customers.AsNoTracking();

        // Exact-match by primary key takes priority when supplied
        if (customerId.HasValue)
        {
            query = query.Where(customer => customer.CustomerId == customerId.Value);
        }
        else if (!string.IsNullOrWhiteSpace(documentNumber))
        {
            query = query.Where(customer => customer.DocumentNumber.Contains(documentNumber));
        }
        else if (!string.IsNullOrWhiteSpace(lastName))
        {
            // LIKE search — EF Core translates Contains to SQL LIKE '%value%'.
            query = query.Where(customer => customer.LastName.Contains(lastName));
        }

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderBy(customer => customer.LastName)
            .ThenBy(customer  => customer.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }
}
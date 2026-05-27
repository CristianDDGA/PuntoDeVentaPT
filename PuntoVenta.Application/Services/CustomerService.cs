using Mapster;
using PuntoVenta.Application.DTOs.Common;
using PuntoVenta.Application.DTOs.Customer;
using PuntoVenta.Application.Interfaces.Repositories;
using PuntoVenta.Application.Interfaces.Services;
using PuntoVenta.Domain.Entities;

namespace PuntoVenta.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Adapt<IEnumerable<CustomerDto>>();
    }

    public async Task<IEnumerable<CustomerDto>> SearchByLastNameAsync(string lastName)
    {
        var matchingCustomers = await _customerRepository.SearchByLastNameAsync(lastName);
        return matchingCustomers.Adapt<IEnumerable<CustomerDto>>();
    }

    public async Task<CustomerDto?> GetByIdAsync(int customerId)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(customerId);
        return existingCustomer?.Adapt<CustomerDto>();
    }

    public async Task<CustomerDto> CreateAsync(CreateCustomerDto createCustomerDto)
    {
        var newCustomer = Customer.Create(
            createCustomerDto.DocumentNumber,
            createCustomerDto.FirstName,
            createCustomerDto.LastName,
            createCustomerDto.Phone,
            createCustomerDto.Address,
            createCustomerDto.City,
            createCustomerDto.Email);

        var savedCustomer = await _customerRepository.AddAsync(newCustomer);
        return savedCustomer.Adapt<CustomerDto>();
    }

    public async Task<bool> ActivateAsync(int customerId)
        => await _customerRepository.ActivateAsync(customerId);

    public async Task<bool> DeactivateAsync(int customerId)
        => await _customerRepository.DeactivateAsync(customerId);

    public async Task<PagedResult<CustomerDto>> SearchPagedAsync(
        int?    customerId,
        string? documentNumber,
        string? lastName,
        int     page,
        int     pageSize,
        bool    onlyActive = false)
    {
        var (items, totalCount) = await _customerRepository.SearchPagedAsync(customerId, documentNumber, lastName, page, pageSize, onlyActive);
        var customerDtos        = items.Adapt<IEnumerable<CustomerDto>>();
        return PagedResult<CustomerDto>.Create(customerDtos, totalCount, page, pageSize);
    }
}
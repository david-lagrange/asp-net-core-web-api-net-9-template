using AutoMapper;
using CompanyEmployees.Core.Domain.Entities;
using CompanyEmployees.Core.Domain.Exceptions;
using CompanyEmployees.Core.Domain.Repositories;
using CompanyEmployees.Core.Services.Abstractions;
using LoggingService;
using Shared.DataTransferObjects;

namespace CompanyEmployees.Core.Services;

internal sealed class EmployeeService : IEmployeeService
{
    private readonly IRepositoryManager _repository;
    private readonly ILoggerManager _logger;
    private readonly IMapper _mapper;

    public EmployeeService(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
    {
        _repository = repository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges, ct);
        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);

        return employeesDto;
    }

    public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges, ct);
        if (employeeDb is null)
            throw new EmployeeNotFoundException(id);

        var employee = _mapper.Map<EmployeeDto>(employeeDb);
        return employee;
    }

    public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);

        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
        await _repository.SaveAsync(ct);

        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

        return employeeToReturn;
    }

    public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges, ct);
        if (employeeForCompany is null)
            throw new EmployeeNotFoundException(id);

        await _repository.Employee.DeleteEmployeeAsync(company, employeeForCompany, ct);

        await _repository.SaveAsync();
    }

    public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges, ct);
        if (employeeEntity is null)
            throw new EmployeeNotFoundException(id);

        _mapper.Map(employeeForUpdate, employeeEntity);

        await _repository.SaveAsync();
    }

    public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges, CancellationToken ct = default)
    {
        var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges, ct);
        if (company is null)
            throw new CompanyNotFoundException(companyId);

        var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges, ct);
        if (employeeEntity is null)
            throw new EmployeeNotFoundException(companyId);

        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);

        return (employeeToPatch, employeeEntity);
    }

    public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity, CancellationToken ct = default)
    {
        _mapper.Map(employeeToPatch, employeeEntity);

        await _repository.SaveAsync(ct);
    }
}

//public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
//{
//    var company = _repository.Company.GetCompany(companyId, trackChanges);
//    if (company is null)
//        throw new CompanyNotFoundException(companyId);

//    var employeeForCompany = _repository.Employee.GetEmployee(companyId, id, trackChanges);
//    if (employeeForCompany is null)
//        throw new EmployeeNotFoundException(id);

//    using var trans = _repository.BeginTransaction();

//    _repository.Employee.DeleteEmployee(company, employeeForCompany);

//    trans.Commit();
//}
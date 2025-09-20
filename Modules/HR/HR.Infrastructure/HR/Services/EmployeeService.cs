using AutoMapper;
using ERPSys.SharedKernel;
using ERPSys.SharedKernel.Common;
using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.IntegrationEvents.HR;
using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Domain.Events;
using HR.Domain.Events.HR.Domain.Events;
using HR.Infrastructure.HR.DbContext;
using HR.Infrastructure.Outbox;
using Logging.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedKernel.IntegrationEvents.HR;
using System.Text.Json;

namespace HR.Infrastructure.HR.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly HRDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;
        private readonly ILoggingService _logService;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IEventPublisher _eventPublisher; // 🆕 هنا

        public EmployeeService(
            HRDbContext context,
            IMapper mapper,
            ILogger<EmployeeService> logger,
            ILoggingService logService,
            IOutboxRepository outboxRepository,
            IEventPublisher eventPublisher // 🆕 Inject هنا
        )
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
            _logService = logService;
            _outboxRepository = outboxRepository;
            _eventPublisher = eventPublisher; // 🆕 Assign
        }





        public async Task<Result<PagedList<EmployeeDTO>>> GetAllAsync(int pageIndex, int pageSize)
        {
            try
            {
                var query = _context.Employees
                    .Where(e => !e.IsDeleted)
                    .Include(e => e.JobTitle)
                    .ThenInclude(j => j.Department)
                    .OrderByDescending(e => e.Id);

                var totalCount = await query.CountAsync();
                var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                var dtos = _mapper.Map<List<EmployeeDTO>>(items);

                return Result<PagedList<EmployeeDTO>>.Success(new PagedList<EmployeeDTO>(dtos, totalCount, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(GetAllAsync), ex);
                return Result<PagedList<EmployeeDTO>>.Failure(ex.Message);
            }
        }

        public async Task<Result<EmployeeDTO>> GetByIdAsync(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.JobTitle)
                    .ThenInclude(j => j.Department)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                    return Result<EmployeeDTO>.Failure("Employee not found.");

                var dto = _mapper.Map<EmployeeDTO>(employee);
                return Result<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(GetByIdAsync), ex, id);
                return Result<EmployeeDTO>.Failure(ex.Message);
            }
        }

        public async Task<Result<EmployeeDTO>> CreateAsync(EmployeeDTO employeeDto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeDto.FirstName) ||
                    string.IsNullOrWhiteSpace(employeeDto.MiddelName) ||
                    string.IsNullOrWhiteSpace(employeeDto.LastName))
                    return Result<EmployeeDTO>.Failure("First name, middle name, and last name are required.");

                var department = await _context.Departments.FirstOrDefaultAsync(d => d.Name == employeeDto.Department);
                if (department == null)
                    return Result<EmployeeDTO>.Failure("Invalid Department");

                var jobTitle = await _context.JobTitles.FirstOrDefaultAsync(j => j.Title == employeeDto.JobTitle);
                if (jobTitle == null)
                    return Result<EmployeeDTO>.Failure("Invalid Job Title");

                var employee = new Employee
                {
                    BasicSalary = employeeDto.BasicSalary,
                    Allowances = employeeDto.Allowances,
                    Deductions = employeeDto.Deductions,
                    Gender = employeeDto.Gender,
                    EmployeeCode = GenerateEmployeeCode(),
                    CreatedOn = DateTime.UtcNow
                };

                employee.SetName(employeeDto.FirstName, employeeDto.MiddelName, employeeDto.LastName);
                employee.SetBirthDate(employeeDto.DateOfBirth);
                employee.AssignDepartmentAndJobTitle(department, jobTitle);

                await _context.Employees.AddAsync(employee);
                await _context.SaveChangesAsync();

                // 🚀 سجّل الـ Event كـ Domain Event
                employee.MarkAsCreated();
                await _context.SaveChangesAsync();

                await PublishEmployeeEventAsync(employee);



                var dto = _mapper.Map<EmployeeDTO>(employee);
                return Result<EmployeeDTO>.Success(dto);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(CreateAsync), ex, employeeDto);
                return Result<EmployeeDTO>.Failure(ex.Message);
            }
        }

        private async Task PublishEmployeeEventAsync(Employee employee)
        {
            foreach (var domainEvent in employee.DomainEvents)
            {
                if (domainEvent is EmployeeCreatedEvent e)
                {
                    var integrationEvent = new EmployeeCreatedIntegrationEvent(
                        e.EmployeeId,
                        e.DepartmentId,
                        e.DepartmentName,
                        e.JobTitle,
                        e.EmployeeCode,
                        e.FullName,
                        e.NetSalary
                    );

                    if (_eventPublisher is InMemoryEventBus inMemoryBus)
                    {
                        // 🚀 InMemory → Publish Direct
                        await inMemoryBus.PublishAsync(integrationEvent, new EventMetadata
                        {
                            EventId = Guid.NewGuid(),
                            Timestamp = DateTime.UtcNow,
                            SourceModule = "HR",
                            UserId = "system"
                        });
                    }
                    else
                    {
                        // 🚀 RabbitMQ → Outbox
                        var payload = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        var outboxMessage = new OutboxMessage
                        {
                            EventId = Guid.NewGuid(),
                            Type = typeof(EmployeeCreatedIntegrationEvent).AssemblyQualifiedName!,
                            Payload = payload,
                            OccurredOn = DateTime.UtcNow,
                            Sent = false
                        };

                        await _outboxRepository.AddAsync(outboxMessage);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }

        public async Task<Result<bool>> UpdateAsync(int id, EmployeeDTO employeeDto)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null)
                    return Result<bool>.Failure("Employee not found.");

                // Load department WITH related job titles
                var department = await _context.Departments
                    .Include(d => d.JobTitles)
                    .FirstOrDefaultAsync(d => d.Name == employeeDto.Department);
                if (department == null)
                    return Result<bool>.Failure("Invalid Department");

                var jobTitle = await _context.JobTitles
                    .FirstOrDefaultAsync(j => j.Title == employeeDto.JobTitle);
                if (jobTitle == null)
                    return Result<bool>.Failure("Invalid Job Title");

                employee.SetName(employeeDto.FirstName, employeeDto.MiddelName, employeeDto.LastName);
                employee.SetBirthDate(employeeDto.DateOfBirth);

                // This will now auto-correct if the job title doesn't match the department
                employee.AssignDepartmentAndJobTitle(department, jobTitle);

                employee.BasicSalary = employeeDto.BasicSalary;
                employee.Allowances = employeeDto.Allowances;
                employee.Deductions = employeeDto.Deductions;
                employee.UpdatedOn = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                await PublishEmployeeUpdatedEventAsync(employee);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(UpdateAsync), ex, employeeDto);
                return Result<bool>.Failure(ex.Message);
            }
        }

        private async Task PublishEmployeeUpdatedEventAsync(Employee employee)
        {
            var integrationEvent = new EmployeeUpdatedIntegrationEvent(
                employee.Id,
                employee.Department?.Id ?? 0,
                employee.Department?.Name ?? string.Empty,
                employee.JobTitle?.Title ?? string.Empty,
                employee.EmployeeCode,
                employee.FirstName + " " + employee.MiddelName + " " + employee.LastName,
                employee.NetSalary
            );

            if (_eventPublisher is InMemoryEventBus inMemoryBus)
            {
                await inMemoryBus.PublishAsync(integrationEvent, new EventMetadata
                {
                    EventId = Guid.NewGuid(),
                    Timestamp = DateTime.UtcNow,
                    SourceModule = "HR",
                    UserId = "system"
                });
            }
            else
            {
                var payload = JsonSerializer.Serialize(integrationEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var outboxMessage = new OutboxMessage
                {
                    EventId = Guid.NewGuid(),
                    Type = typeof(EmployeeUpdatedIntegrationEvent).AssemblyQualifiedName!,
                    Payload = payload,
                    OccurredOn = DateTime.UtcNow,
                    Sent = false
                };

                await _outboxRepository.AddAsync(outboxMessage);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.Leaves)
                    .Include(e => e.Payslips)
                    .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

                if (employee == null)
                    return Result<bool>.Failure("Employee not found.");

                if (employee.Leaves.Any() || employee.Payslips.Any())
                    return Result<bool>.Failure("❌ Cannot delete: employee has related records.");

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(DeleteAsync), ex, id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> SoftDeleteAsync(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.JobTitle)
                    .Include(e => e.Leaves)
                    .Include(e => e.Payslips)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                    return Result<bool>.Failure("Employee not found.");

                if (employee.JobTitle?.Title == "CEO")
                    return Result<bool>.Failure("❌ Cannot delete CEO.");

                if (employee.Leaves.Any() || employee.Payslips.Any())
                    return Result<bool>.Failure("❌ Cannot delete: employee has related records.");

                employee.IsDeleted = true;
                await _context.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(SoftDeleteAsync), ex, id);
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<string>>> GetDepartmentsAsync()
        {
            try
            {
                var departments = await _context.Departments
                    .Select(d => d.Name)
                    .OrderBy(n => n)
                    .ToListAsync();
                return Result<List<string>>.Success(departments);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(GetDepartmentsAsync), ex);
                return Result<List<string>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<string>>> GetJobTitlesAsync()
        {
            try
            {
                var jobTitles = await _context.JobTitles
                    .Select(j => j.Title)
                    .OrderBy(n => n)
                    .ToListAsync();
                return Result<List<string>>.Success(jobTitles);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(GetJobTitlesAsync), ex);
                return Result<List<string>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<string>>> GetJobTitlesByDepartmentNameAsync(string dep)
        {
            try
            {
                var jobTitles = await _context.JobTitles
                    .Where(j => j.Department != null && j.Department.Name == dep)
                    .Select(j => j.Title)
                    .OrderBy(t => t)
                    .ToListAsync();
                return Result<List<string>>.Success(jobTitles);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(GetJobTitlesByDepartmentNameAsync), ex, dep);
                return Result<List<string>>.Failure(ex.Message);
            }
        }

      
        private async Task<int> GetDepartmentIdByName(string name)
        {
            var dept = await _context.Departments.FirstOrDefaultAsync(d => d.Name == name);
            if (dept == null) throw new Exception("Invalid Department");
            return dept.Id;
        }

        private async Task<int> GetJobTitleIdByName(string title)
        {
            var jt = await _context.JobTitles.FirstOrDefaultAsync(j => j.Title == title);
            if (jt == null) throw new Exception("Invalid Job Title");
            return jt.Id;
        }
        public async Task<Result<PagedList<EmployeeDTO>>> SearchAsync(string query, int pageIndex, int pageSize)
        {
            try
            {
                query = query.ToLower();

                var employeesQuery = _context.Employees
                    .Include(e => e.JobTitle)
                    .ThenInclude(j => j.Department)
                    .Where(e => !e.IsDeleted &&
                               (e.EmployeeCode.ToLower().Contains(query) ||
                               e.JobTitle.Title.ToLower().Contains(query) ||
                               e.JobTitle.Department.Name.ToLower().Contains(query) ||
                               (e.FirstName + " " + e.MiddelName + " " + e.LastName).ToLower().Contains(query)));

                var totalCount = await employeesQuery.CountAsync();
                var items = await employeesQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
                var dtos = _mapper.Map<List<EmployeeDTO>>(items);

                return Result<PagedList<EmployeeDTO>>.Success(new PagedList<EmployeeDTO>(dtos, totalCount, pageIndex, pageSize));
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(SearchAsync), ex, query);
                return Result<PagedList<EmployeeDTO>>.Failure(ex.Message);
            }
        }

        public async Task<Result<List<string>>> SearchSuggestAppendAsync(string query)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(query))
                    return Result<List<string>>.Success(new List<string>());

                query = query.ToLower();

                // Get matching employee names, codes, job titles, or departments
                var suggestions = await _context.Employees
                    //.Include(e => e.JobTitle)
                    //.ThenInclude(j => j.Department)
                    .Where(e => !e.IsDeleted &&
                                (e.EmployeeCode.ToLower().Contains(query) ||
                                 //e.JobTitle.Title.ToLower().Contains(query) ||
                                 //e.JobTitle.Department.Name.ToLower().Contains(query) ||
                                 (e.FirstName + " " + e.MiddelName + " " + e.LastName).ToLower().Contains(query)))
                    .Select(e => new
                    {
                        FullName = (e.FirstName + " " + e.MiddelName + " " + e.LastName).Trim(),
                        e.EmployeeCode
                        //,
                        //JobTitle = e.JobTitle.Title,
                        //Department = e.JobTitle.Department.Name
                    })
                    .Take(10) // Limit results for performance
                    .ToListAsync();

                // Merge into a flat list of suggestion strings
                var resultList = suggestions
                    .SelectMany(s => new[] {
                s.FullName,
                s.EmployeeCode,
                //s.JobTitle,
                //s.Department
                    })
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Distinct()
                    .ToList();

                return Result<List<string>>.Success(resultList);
            }
            catch (Exception ex)
            {
                await _logService.LogErrorAsync("HR", "EmployeeService", nameof(SearchSuggestAppendAsync), ex, query);
                return Result<List<string>>.Failure(ex.Message);
            }
        }

        private string GenerateEmployeeCode()
        {
            var random = new Random();
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "0123456789";

            string randomLetters = new string(Enumerable.Range(0, 3)
                .Select(_ => letters[random.Next(letters.Length)]).ToArray());

            string randomDigits = new string(Enumerable.Range(0, 3)
                .Select(_ => digits[random.Next(digits.Length)]).ToArray());

            return $"EMP-{randomLetters}{randomDigits}";
        }

        
    }
}

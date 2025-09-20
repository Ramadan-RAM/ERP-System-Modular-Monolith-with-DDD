using AutoMapper;
using ERPSys.SharedKernel.Events;
using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Persistence;
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using HR.Application.Interfaces;
using HR.Application.Interfaces.InterfacRepository;
using HR.Application.InterfacRepository;
using HR.Domain.HR.Entities;
using HR.Infrastructure.Outbox;
using System.Text.Json;

namespace HR.Infrastructure.HR.Services
{
    public class PayslipService : IPayslipService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAttendanceRepository _attendanceRepository;
        private readonly ILeaveRepository _leaveRepository;
        private readonly IPayslipRepository _payslipRepository;
        private readonly IPayslipCalculatorService _calculator;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOutboxRepository _outboxRepository;
        private readonly IMapper _mapper;

        public PayslipService(
            IEmployeeRepository employeeRepo,
            IAttendanceRepository attendanceRepo,
            ILeaveRepository leaveRepo,
            IPayslipRepository payslipRepo,
            IPayslipCalculatorService calculator,
            IEventPublisher eventPublisher,
            IOutboxRepository outboxRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepo;
            _attendanceRepository = attendanceRepo;
            _leaveRepository = leaveRepo;
            _payslipRepository = payslipRepo;
            _calculator = calculator;
            _eventPublisher = eventPublisher;
            _outboxRepository = outboxRepository;
            _mapper = mapper;
        }


        public async Task<Result<List<PayslipDTO>>> GetAllAsync()
        {
            try
            {
                var payslips = await _payslipRepository.GetAllAsync();
                var dtos = _mapper.Map<List<PayslipDTO>>(payslips);
                return Result<List<PayslipDTO>>.Success(dtos);
            }
            catch (Exception ex)
            {
                return Result<List<PayslipDTO>>.Failure(ex.Message);
            }
        }

        public async Task<Result<PayslipDTO?>> GetByIdAsync(int id)
        {
            try
            {
                var payslip = await _payslipRepository.GetByIdAsync(id);
                var dto = payslip is null ? null : _mapper.Map<PayslipDTO>(payslip);
                return Result<PayslipDTO?>.Success(dto);
            }
            catch (Exception ex)
            {
                return Result<PayslipDTO?>.Failure(ex.Message);
            }
        }

        public async Task<Result<PayslipDTO>> CreateAsync(PayslipDTO dto)
        {
            try
            {
                var payslip = _mapper.Map<Payslip>(dto);
                var created = await _payslipRepository.AddAsync(payslip);
                var resultDto = _mapper.Map<PayslipDTO>(created);
                return Result<PayslipDTO>.Success(resultDto);
            }
            catch (Exception ex)
            {
                return Result<PayslipDTO>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> UpdateAsync(int id, PayslipDTO dto)
        {
            try
            {
                var existing = await _payslipRepository.GetByIdAsync(id);
                if (existing is null) return Result<bool>.Success(false);

                _mapper.Map(dto, existing);
                var updated = await _payslipRepository.UpdateAsync(existing);
                return Result<bool>.Success(updated);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<bool>> DeleteAsync(int id)
        {
            try
            {
                var deleted = await _payslipRepository.DeleteAsync(id);
                return Result<bool>.Success(deleted);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }

        public async Task<Result<PayslipDTO>> GeneratePayslipAsync(int employeeId, int year, int month)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return Result<PayslipDTO>.Failure("Employee not found");

            bool alreadyExists = await _payslipRepository.ExistsAsync(employeeId, year, month);
            if (alreadyExists)
                return Result<PayslipDTO>.Failure("Payslip already exists for this employee in the given month and year");

            var attendanceRecords = await _attendanceRepository.GetByEmployeeAndMonth(employeeId, year, month);
            if (attendanceRecords == null || !attendanceRecords.Any())
                return Result<PayslipDTO>.Failure("No attendance records found for this employee in the given month and year");

            var leaves = await _leaveRepository.GetByEmployeeAndMonth(employeeId, year, month);

            var deductions = _calculator.CalculateDeductions(attendanceRecords, leaves, employee.BasicSalary);
            var totalDeductions = deductions + employee.Deductions;
            var netSalary = employee.BasicSalary + employee.Allowances - totalDeductions;

            var payslip = new Payslip
            {
                EmployeeId = employeeId,
                PayDate = new DateTime(year, month, 1),
                BasicSalary = employee.BasicSalary,
                Allowances = employee.Allowances,
                Deductions = totalDeductions,
                Notes = $"Auto generated and salary is = {netSalary:F2}"
            };

            await _payslipRepository.AddAsync(payslip);

            // 🔥 نشر PayrollCalculatedIntegrationEvent
            var payrollEvent = new PayrollCalculatedIntegrationEvent(
                employee.Id,
                employee.DepartmentId ?? 0,
                employee.EmployeeCode,
                $"{employee.FirstName} {employee.MiddelName} {employee.LastName}",
                netSalary,
                "EGP",
                $"{year}-{month:00}"
            );

            if (_eventPublisher is InMemoryEventBus inMemoryBus)
            {
                await inMemoryBus.PublishAsync(payrollEvent, new EventMetadata
                {
                    EventId = payrollEvent.EventId,
                    Timestamp = DateTime.UtcNow,
                    SourceModule = "HR",
                    UserId = "system"
                });
            }
            else
            {
                var payload = JsonSerializer.Serialize(payrollEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var outboxMessage = new OutboxMessage
                {
                    EventId = payrollEvent.EventId,
                    Type = typeof(PayrollCalculatedIntegrationEvent).AssemblyQualifiedName!,
                    Payload = payload,
                    OccurredOn = DateTime.UtcNow,
                    Sent = false
                };

                await _outboxRepository.AddAsync(outboxMessage);
            }

            var dto = new PayslipDTO
            {
                Id = payslip.Id,
                EmployeeId = employeeId,
                Amount = netSalary,
                DateIssued = payslip.PayDate,
                Notes = payslip.Notes,
                EmployeeName = $"{employee.FirstName} {employee.MiddelName} {employee.LastName}"
            };

            return Result<PayslipDTO>.Success(dto);
        }

        public async Task<Result<List<PayslipDTO>>> GeneratePayslipsForAllAsync(int year, int month)
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync();
                var payslipList = new List<PayslipDTO>();

                foreach (var employee in employees)
                {
                    var attendanceRecords = await _attendanceRepository.GetByEmployeeAndMonth(employee.Id, year, month);
                    if (attendanceRecords == null || !attendanceRecords.Any())
                        continue;

                    bool alreadyExists = await _payslipRepository.ExistsAsync(employee.Id, year, month);
                    if (alreadyExists)
                        continue;

                    var leaves = await _leaveRepository.GetByEmployeeAndMonth(employee.Id, year, month);

                    var deductions = _calculator.CalculateDeductions(attendanceRecords, leaves, employee.BasicSalary);
                    var totalDeductions = deductions + employee.Deductions;
                    var netSalary = employee.BasicSalary + employee.Allowances - totalDeductions;

                    var payslip = new Payslip
                    {
                        EmployeeId = employee.Id,
                        PayDate = new DateTime(year, month, 1),
                        BasicSalary = employee.BasicSalary,
                        Allowances = employee.Allowances,
                        Deductions = totalDeductions,
                        Notes = $"Auto generated and salary is = {netSalary:F2}"
                    };

                    await _payslipRepository.AddAsync(payslip);

                    // 🔥 Publish a PayrollCalculatedIntegrationEvent to each employee
                    var payrollEvent = new PayrollCalculatedIntegrationEvent(
                        employee.Id,
                        employee.DepartmentId ?? 0,
                        employee.EmployeeCode,
                        $"{employee.FirstName} {employee.MiddelName} {employee.LastName}",
                        netSalary,
                        "EGP",
                        $"{year}-{month:00}"
                    );

                    if (_eventPublisher is InMemoryEventBus inMemoryBus)
                    {
                        await inMemoryBus.PublishAsync(payrollEvent, new EventMetadata
                        {
                            EventId = payrollEvent.EventId,
                            Timestamp = DateTime.UtcNow,
                            SourceModule = "HR",
                            UserId = "system"
                        });
                    }
                    else
                    {
                        var payload = JsonSerializer.Serialize(payrollEvent, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                        });

                        var outboxMessage = new OutboxMessage
                        {
                            EventId = payrollEvent.EventId,
                            Type = typeof(PayrollCalculatedIntegrationEvent).AssemblyQualifiedName!,
                            Payload = payload,
                            OccurredOn = DateTime.UtcNow,
                            Sent = false
                        };

                        await _outboxRepository.AddAsync(outboxMessage);
                    }

                    payslipList.Add(new PayslipDTO
                    {
                        Id = payslip.Id,
                        EmployeeId = employee.Id,
                        Amount = netSalary,
                        DateIssued = payslip.PayDate,
                        Notes = payslip.Notes,
                        EmployeeName = $"{employee.FirstName} {employee.MiddelName} {employee.LastName}"
                    });
                }

                return Result<List<PayslipDTO>>.Success(payslipList);
            }
            catch (Exception ex)
            {
                return Result<List<PayslipDTO>>.Failure(ex.Message);
            }
        }
    }

}

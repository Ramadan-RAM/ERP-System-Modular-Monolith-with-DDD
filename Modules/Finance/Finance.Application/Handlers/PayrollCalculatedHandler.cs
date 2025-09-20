// File: Finance/Application/Handlers/PayrollCalculatedHandler.cs
using ERPSys.SharedKernel.IntegrationEvents.HR;
using ERPSys.SharedKernel.Messaging;
using ERPSys.SharedKernel.Persistence;
using Finance.Application.Interfaces;
using Finance.Application.ProcessedEvents;
using Finance.Domain.Entities;
using Finance.Domain.ValueObjects;

namespace Finance.Application.Handlers
{
    public class PayrollCalculatedHandler : IIntegrationEventHandler<PayrollCalculatedIntegrationEvent>
    {
        private readonly IProcessedEventRepository _processedRepo;
        private readonly IRepository<JournalEntry, int> _journalRepo;
        private readonly IAccountMappingService _accountMappingService;

        public PayrollCalculatedHandler(
            IProcessedEventRepository processedRepo,
            IRepository<JournalEntry, int> journalRepo,
            IAccountMappingService accountMappingService)
        {
            _processedRepo = processedRepo;
            _journalRepo = journalRepo;
            _accountMappingService = accountMappingService;
        }

        public async Task HandleAsync(PayrollCalculatedIntegrationEvent @event)
        {
            if (await _processedRepo.ExistsAsync(@event.EventId)) return;

            var entry = new JournalEntry(new DocumentNumber("PAY-" + @event.EmployeeId), DateTime.UtcNow);

            var salaryExpenseAccountId = await _accountMappingService.GetSalaryExpenseAccountId(@event.DepartmentId);
            var cashAccountId = await _accountMappingService.GetCashAccountId(@event.DepartmentId , @event.EmployeeCode);

            entry.AddLine(new JournalLine(
                glAccountId: salaryExpenseAccountId,
                debit: new Money(@event.NetSalary, @event.Currency),
                credit: null,
                costCenterId: @event.DepartmentId,
                description: $"Payroll {@event.Period}",
                externalRef: @event.EmployeeCode
            ));

            entry.AddLine(new JournalLine(
                glAccountId: cashAccountId,
                debit: null,
                credit: new Money(@event.NetSalary, @event.Currency),
                costCenterId: @event.DepartmentId,
                description: $"Payroll {@event.Period}",
                externalRef: @event.EmployeeCode
            ));

            await _journalRepo.AddAsync(entry);
            await _processedRepo.MarkProcessedAsync(@event.EventId, nameof(PayrollCalculatedHandler));
        }
    }
}

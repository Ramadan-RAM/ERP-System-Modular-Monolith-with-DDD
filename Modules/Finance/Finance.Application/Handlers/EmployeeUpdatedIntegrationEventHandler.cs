using ERPSys.SharedKernel.IntegrationEvents.HR;
using ERPSys.SharedKernel.Messaging;
using Finance.Application.Interfaces;
using Finance.Domain.Entities;
using SharedKernel.IntegrationEvents.HR;

namespace Finance.Application.Handlers
{
    public class EmployeeUpdatedIntegrationEventHandler : IIntegrationEventHandler<EmployeeUpdatedIntegrationEvent>
    {
        private readonly IFinanceOnboardingService _financeService;

        public EmployeeUpdatedIntegrationEventHandler(IFinanceOnboardingService financeService)
        {
            _financeService = financeService;
        }

        public async Task HandleAsync(EmployeeUpdatedIntegrationEvent @event)
        {
            // ✅  Finance Service 
            await _financeService.UpdateEmployeeAccountsAsync(
                @event.EmployeeId,
                @event.DepartmentId,
                @event.DepartmentName,
                @event.JobTitle,
                @event.EmployeeCode,
                @event.FullName,
                @event.NetSalary
            );
        }
    }
}

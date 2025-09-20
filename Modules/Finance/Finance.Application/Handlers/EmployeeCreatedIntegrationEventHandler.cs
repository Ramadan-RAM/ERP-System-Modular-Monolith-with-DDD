using ERPSys.SharedKernel.Messaging;
using SharedKernel.IntegrationEvents.HR;
using Finance.Application.Interfaces;

namespace Finance.Application.Handlers
{
    public class EmployeeCreatedIntegrationEventHandler : IIntegrationEventHandler<EmployeeCreatedIntegrationEvent>
    {
        private readonly IFinanceOnboardingService _financeService;

        public EmployeeCreatedIntegrationEventHandler(IFinanceOnboardingService financeService)
        {
            _financeService = financeService;
        }

        public async Task HandleAsync(EmployeeCreatedIntegrationEvent @event)
        {
            await _financeService.CreateEmployeeAccountsAsync(
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

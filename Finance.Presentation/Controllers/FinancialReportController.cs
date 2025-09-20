using Finance.Application.DTOs;
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialReportController : ControllerBase
    {
        private readonly IFinancialReportService _service;

        public FinancialReportController(IFinancialReportService service)
        {
            _service = service;
        }

        // ✅ Monthly Report
        [HttpGet("monthly/{year:int}/{month:int}")]
        public async Task<ActionResult<ProfitLossForecastDto>> GetMonthlyReport(int year, int month)
        {
            var result = await _service.GetMonthlyReportAsync(year, month);
            return Ok(result);
        }

        // ✅ Yearly Report
        [HttpGet("yearly/{year:int}")]
        public async Task<ActionResult<ProfitLossForecastDto>> GetYearlyReport(int year)
        {
            var result = await _service.GetYearlyReportAsync(year);
            return Ok(result);
        }

        // ✅ Expenses Report
        [HttpGet("expenses")]
        public async Task<ActionResult<IReadOnlyList<GeneralExpenseDto>>> GetExpensesReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var result = await _service.GetExpensesReportAsync(start, end);
            return Ok(result);
        }

        // ✅ Purchases Report
        [HttpGet("purchases")]
        public async Task<ActionResult<IReadOnlyList<PurchaseOrderDto>>> GetPurchasesReport(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var result = await _service.GetPurchasesReportAsync(start, end);
            return Ok(result);
        }

        // ✅ Net Profit (KPI)
        [HttpGet("net-profit")]
        public async Task<ActionResult<MoneyDto>> GetNetProfit(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var result = await _service.GetNetProfitAsync(start, end);
            return Ok(result);
        }

        // ✅ Total Expenses
        [HttpGet("total-expenses")]
        public async Task<ActionResult<decimal>> GetTotalExpenses(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var result = await _service.GetTotalExpensesAsync(start, end);
            return Ok(result);
        }

        // ✅ Total Purchases
        [HttpGet("total-purchases")]
        public async Task<ActionResult<decimal>> GetTotalPurchases(
            [FromQuery] DateTime start,
            [FromQuery] DateTime end)
        {
            var result = await _service.GetTotalPurchasesAsync(start, end);
            return Ok(result);
        }
    }
}

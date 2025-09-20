using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChartReportsController : ControllerBase
    {
        private readonly IChartReportService _chartService;

        public ChartReportsController(IChartReportService chartService)
        {
            _chartService = chartService;
        }

        [HttpGet("profit-loss-trend")]
        public async Task<IActionResult> GetProfitLossTrend([FromQuery] int[] years)
        {
            var result = await _chartService.GetMonthlyProfitLossTrendAsync(years);
            return Ok(result);
        }

        [HttpGet("expenses-breakdown")]
        public async Task<IActionResult> GetExpensesBreakdown([FromQuery] int year)
        {
            var result = await _chartService.GetExpenseBreakdownByCategoryAsync(year);
            return Ok(result);
        }

        [HttpGet("purchases-suppliers")]
        public async Task<IActionResult> GetPurchasesBySupplier([FromQuery] int year)
        {
            var result = await _chartService.GetPurchasesBySupplierAsync(year);
            return Ok(result);
        }

        [HttpGet("kpi-summary")]
        public async Task<IActionResult> GetKpiSummary([FromQuery] int year)
        {
            var result = await _chartService.GetKpiSummaryAsync(year);
            return Ok(result);
        }

    }
}

// Finance.API/Controllers/ComparisonReportsController.cs
using Finance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Finance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComparisonReportsController : ControllerBase
    {
        private readonly IComparisonReportService _reportService;

        public ComparisonReportsController(IComparisonReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("profitloss")]
        public async Task<IActionResult> CompareProfitLoss([FromQuery] int[] years)
        {
            var result = await _reportService.CompareProfitLossAsync(years);
            return Ok(result);
        }

        [HttpGet("expenses")]
        public async Task<IActionResult> CompareExpenses([FromQuery] int[] years)
        {
            var result = await _reportService.CompareExpensesAsync(years);
            return Ok(result);
        }

        [HttpGet("purchases")]
        public async Task<IActionResult> ComparePurchases([FromQuery] int[] years)
        {
            var result = await _reportService.ComparePurchasesAsync(years);
            return Ok(result);
        }

        [HttpGet("summary")]
        public async Task<IActionResult> CompareSummary([FromQuery] int[] years)
        {
            var result = await _reportService.CompareFinanceSummaryAsync(years);
            return Ok(result);
        }
    }
}

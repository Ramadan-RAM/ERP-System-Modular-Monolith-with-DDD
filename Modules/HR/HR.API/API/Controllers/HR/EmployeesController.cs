
using HR.Application.DTOs.HR;
using HR.Application.HR.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HR.API.API.Controllers.HR
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN , ROLE_HR")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetAll(int pageIndex = 1, int pageSize = 10)
        {
            var employees = await _employeeService.GetAllAsync(pageIndex, pageSize);
            return Ok(employees);
        }

        // ✅ Get by ID
        [HttpGet("GetById/{id}")]
        public async Task<ActionResult<EmployeeDTO>> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null) return NotFound();
            return Ok(employee);
        }

        // Replace the Create method with the following implementation
        [HttpPost("Create")]
        public async Task<ActionResult<EmployeeDTO>> Create([FromBody] EmployeeDTO dto)
        {
            var createdResult = await _employeeService.CreateAsync(dto);
            if (!createdResult.IsSuccess || createdResult.Data == null)
                return BadRequest(createdResult.Message);

            return CreatedAtAction(nameof(GetById), new { id = createdResult.Data.Id }, createdResult.Data);
        }

        // ✅ Update
        [HttpPut("Update/{id}")]
        public async Task<ActionResult<EmployeeDTO>> Update(int id, [FromBody] EmployeeDTO dto)
        {
            if (id != dto.Id) return BadRequest("ID mismatch");

            var result = await _employeeService.UpdateAsync(id, dto);
            if (!result.IsSuccess) return NotFound(result.Message);

            var updatedEmployee = await _employeeService.GetByIdAsync(id);
            if (!updatedEmployee.IsSuccess || updatedEmployee.Data == null)
                return NotFound("Employee updated but could not be retrieved.");

            return Ok(updatedEmployee.Data);
        }


        //  دالة الحذف المنطقي مع استثناء المدير التنفيذي
        [HttpDelete("{id}/soft")]
        public async Task<IActionResult> SoftDeleteEmployee(int id)
        {
            var deletedResult = await _employeeService.SoftDeleteAsync(id);
            if (!deletedResult.IsSuccess || !deletedResult.Data) return NotFound("Cannot delete CEO or employee not found.");
            return NoContent();
        }



        [HttpGet("JobTitlesByDepartment")]
        public async Task<IActionResult> GetJobTitlesByDepartment(string dep)
        {
            var jobTitles = await _employeeService.GetJobTitlesByDepartmentNameAsync(dep);
            return Ok(jobTitles);
        }



        [HttpGet("Departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var depts = await _employeeService.GetDepartmentsAsync();
            return Ok(depts);
        }

        [HttpGet("JobTitles")]
        public async Task<IActionResult> GetJobTitles()
        {
            var jobs = await _employeeService.GetJobTitlesAsync();
            return Ok(jobs);
        }

        [HttpGet("Search")]
        public async Task<IActionResult> Search(string q, int pageIndex = 1, int pageSize = 10)
        {
            var result = await _employeeService.SearchAsync(q, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get search suggestions for employees, job titles, departments, etc.
        /// </summary>
        /// <param name="query">The search text.</param>
        [HttpGet("search-suggestions")]
        public async Task<IActionResult> GetSearchSuggestions([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return BadRequest("Query is too short.");

            var result = await _employeeService.SearchSuggestAppendAsync(query);
          

            return Ok(result);
        }


        // Remove the following method since IEmployeeService does not contain SearchEmployeesAsync
        //[HttpGet("SearchEmployees")]
        //public async Task<IActionResult> SearchEmployees(string q)
        //{
        //    // This method cannot be implemented because IEmployeeService does not have SearchEmployeesAsync.
        //    // Remove or implement only if you add such a method to IEmployeeService.
        //    return BadRequest("SearchEmployeesAsync is not implemented in IEmployeeService.");
        //}

    }


}

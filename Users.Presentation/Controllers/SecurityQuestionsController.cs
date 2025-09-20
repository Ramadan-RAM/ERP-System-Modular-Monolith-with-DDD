
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Infrastructure.Persistence;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/[controller]")]
    
    [Route("api/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN")] 
    public class SecurityQuestionsController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public SecurityQuestionsController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");


        // ✅ Get All Questions
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var questions = await _context.SecurityQuestions
                .Select(q => new { q.Id, q.QuestionText })
                .ToListAsync();

            return Ok(questions);
        }

        // ✅ Add new question
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("❌ Question is required.");

            if (await _context.SecurityQuestions.AnyAsync(q => q.QuestionText == question))
                return Conflict("⚠️ Question already exists.");

            var newQuestion = new SecurityQuestion
            {
                Id = Guid.NewGuid(),
                QuestionText = question
            };

            await _context.SecurityQuestions.AddAsync(newQuestion);
            await _context.SaveChangesAsync();

            return Ok("✅ Security question added.");
        }

        // ✅ Update question
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] string updatedQuestion)
        {
            var question = await _context.SecurityQuestions.FindAsync(id);
            if (question == null) return NotFound("❌ Question not found.");

            question.QuestionText = updatedQuestion;
            await _context.SaveChangesAsync();

            return Ok("✅ Question updated.");
        }

        // ✅ Delete question
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var question = await _context.SecurityQuestions.FindAsync(id);
            if (question == null) return NotFound("❌ Question not found.");

            _context.SecurityQuestions.Remove(question);
            await _context.SaveChangesAsync();

            return Ok("✅ Question deleted.");
        }
    }
}

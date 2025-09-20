
using ERPSys.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Users.Application.DTOs.Users;
using Users.Domain.Entities;
using Users.Infrastructure.Common.Security;
using Users.Infrastructure.Persistence;
using BC = BCrypt.Net.BCrypt;

namespace ERP.API.API.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public ProfileController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId == null) return Unauthorized();

            var profile = await _context.UserProfiles
                .FirstOrDefaultAsync(p => p.UserId == Guid.Parse(userId));

            if (profile == null)
                return NotFound("❌ Profile not found.");

            return Ok(new
            {
                profile.FullName,
                profile.Gender,
                PictureBase64 = profile.ProfilePicture != null ? Convert.ToBase64String(profile.ProfilePicture) : null
            });
        }

        [HttpPut("update")]
        [Authorize]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UserProfileUpdateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId.ToString() == userId);

            if (profile == null)
                return NotFound("❌ Profile not found.");

            // ✅ تحقق من الاسم والجنس فقط
            if (string.IsNullOrWhiteSpace(dto.FullName) || string.IsNullOrWhiteSpace(dto.Gender))
                return BadRequest("❗ Full name and gender are required.");

            profile.FullName = dto.FullName;
            profile.Gender = dto.Gender;

            // ✅ حمّل الصورة فقط إذا أُرسلت
            if (dto.ProfilePicture != null)
            {
                var image = await UploadFile.ProcessUploadedFile(dto.ProfilePicture);
                profile.ProfilePicture = image;
            }

            _context.Attach(profile);
            _context.Entry(profile).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new { message = "✅ Profile updated." });
        }

        [HttpGet("security-questions")]
        [Authorize]
        public async Task<IActionResult> GetUserSecurityQuestions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            var userQuestions = user.UserAnswers.Select(a => new
            {
                id = a.QuestionId,
                questionText = a.Question.QuestionText
            }).ToList();

            return Ok(userQuestions);
        }

        [HttpGet("user-answered-questions")]
        [Authorize]
        public async Task<IActionResult> GetUserAnsweredQuestions()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            var answeredQuestions = user.UserAnswers.Select(a => new
            {
                id = a.QuestionId,
                questionText = a.Question.QuestionText
            }).ToList();

            return Ok(answeredQuestions);
        }

        [HttpPost("answer-security-questions")]
        [Authorize]
        public async Task<IActionResult> AnswerSecurityQuestions([FromBody] AnswerSecurityQuestionsDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            if (dto.Answers == null || dto.Answers.Count < 1)
                return BadRequest("❌ At least one answer is required.");

            var questionIds = dto.Answers.Select(a => a.QuestionId).ToList();
            var validQuestions = await _context.SecurityQuestions
                .Where(q => questionIds.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            if (validQuestions.Count != dto.Answers.Count)
                return BadRequest("❌ Invalid question IDs.");

            // ✅ Remove old answers safely
            if (user.UserAnswers.Any())
            {
                _context.UserSecurityAnswers.RemoveRange(user.UserAnswers);
                user.UserAnswers.Clear();
            }

            foreach (var ans in dto.Answers)
            {
                var answer = new UserSecurityAnswer
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    QuestionId = ans.QuestionId,
                    AnswerHash = BC.HashPassword(ans.Answer),
                    AnswerEncrypted = EncryptionHelper.Encrypt(ans.Answer)
                };

                _context.UserSecurityAnswers.Add(answer); // ✅ Explicitly add
            }

            await _context.SaveChangesAsync();
           
            return Ok(new { message = "✅ Answers saved successfully." });
        }


        [HttpPost("change-password-with-questions")]
        [Authorize]
        public async Task<IActionResult> ChangePasswordWithQuestions([FromBody] ChangePasswordWithQuestionsDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            if (!BC.Verify(dto.CurrentPassword, user.PasswordHash))
                return Unauthorized("❌ Incorrect current password.");

            int requiredCorrectAnswers = Math.Max(2, user.UserAnswers.Count / 2);
            int correctAnswers = 0;

            foreach (var input in dto.Answers)
            {
                var storedAnswer = user.UserAnswers.FirstOrDefault(a => a.QuestionId == input.QuestionId);
                if (storedAnswer != null && BC.Verify(input.Answer, storedAnswer.AnswerHash))
                    correctAnswers++;
            }

            if (correctAnswers < requiredCorrectAnswers)
                return Unauthorized($"❌ At least {requiredCorrectAnswers} correct answers required.");

            user.PasswordHash = BC.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
          
            return Ok(new { message = "✅ Password changed successfully." });
        }

        [HttpPost("view-my-answers")]
        [Authorize]
        public async Task<IActionResult> ViewMyAnswers([FromBody] string password)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .ThenInclude(a => a.Question)
                .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            if (!BC.Verify(password, user.PasswordHash))
                return Unauthorized("❌ Incorrect password.");

            var result = user.UserAnswers.Select(a => new
            {
                questionText = a.Question.QuestionText,  // ✅ match JavaScript name
                answer = EncryptionHelper.Decrypt(a.AnswerEncrypted) // ✅ match JS
            });

            return Ok(result);
        }



        [HttpGet("get-user-image")]
        [Authorize]
        public async Task<IActionResult> GetUserImage()
        {
            var userIdClaim = User.FindFirst("sub")?.Value ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

            if (!Guid.TryParse(userIdClaim, out var userIdGuid))
                return BadRequest("Invalid user ID");

            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == userIdGuid);
            if (profile == null || profile.ProfilePicture == null || profile.ProfilePicture.Length == 0)
                //if (profile == null)
                return NotFound();

           
            return File(profile.ProfilePicture, "image/jpeg");
        }
    }

}

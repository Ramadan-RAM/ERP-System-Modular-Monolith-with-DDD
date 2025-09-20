
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Users.Application.DTOs.Users;
using Users.Domain.Entities;
using Users.Infrastructure.Common.Security;
using Users.Infrastructure.Persistence;
using Users.Infrastructure.Services;
using BC = BCrypt.Net.BCrypt;

namespace ERP.API.API.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UsersDbContext _context;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;
    public AuthController(UsersDbContext context, TokenService tokenService, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
    }

 
    // ✅ Login 
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Include(u => u.UserAnswers)
            .FirstOrDefaultAsync(u => u.Username == dto.Username && u.IsActive && !u.IsDeleted);

        if (user == null || !BC.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("❌ Invalid credentials.");

        if (!user.HasAnsweredSecurityQuestions)
        {
            var questions = await _context.SecurityQuestions
                .Select(q => new { q.Id, q.QuestionText })
                .ToListAsync();

            return Ok(new
            {
                message = "🔐 First login – user must answer security questions.",
                firstLogin = true,
                userId = user.Id,
                questions
            });
        }

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct();

        var accessToken = _tokenService.GenerateAccessToken(user, permissions);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.LastLoginAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        _context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            Expires = DateTime.UtcNow.AddDays(1),
            IsUsed = true,
            IsRevoked = false
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = accessToken,
            refreshToken
        });
    }
    // 🔐 First Login - Answer Security Questions
   
    [HttpPost("first-login/submit-answers")]
    public async Task<IActionResult> SubmitSecurityAnswers([FromBody] SecurityAnswersSubmitDto dto)
    {
        var user = await _context.Users
            .Include(u => u.UserAnswers)
            .FirstOrDefaultAsync(u => u.Id == dto.UserId && !u.IsDeleted);

        if (user == null)
            return NotFound("❌ User not found.");

        if (user.HasAnsweredSecurityQuestions)
            return BadRequest("✅ You have already answered your security questions.");

        if (dto.Answers == null || dto.Answers.Count < 3)
            return BadRequest("❌ You must answer at least 3 questions.");

        foreach (var item in dto.Answers)
        {
            user.UserAnswers.Add(new UserSecurityAnswer
            {
                //Id = Guid.NewGuid(),
                UserId = user.Id,
                QuestionId = item.QuestionId,
                AnswerHash = BC.HashPassword(item.Answer),
                AnswerEncrypted = EncryptionHelper.Encrypt(item.Answer) // ✅ Fixed here

            });
        }

        user.HasAnsweredSecurityQuestions = true;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
     
        return Ok(new { message = "✅ Security answers saved" });
    }

    // 🔁 Refresh Access Token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
    {
        var token = await _context.RefreshTokens
            .Include(rt => rt.User)
                .ThenInclude(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

        if (token == null || token.IsRevoked || token.IsUsed || token.Expires < DateTime.UtcNow)
        {
            if (token != null)
            {
                token.IsRevoked = true;
                await _context.SaveChangesAsync();
            }

            return Unauthorized("❌ Invalid or expired refresh token.");
        }

        // ✅ Revoke current token and mark as used
        token.IsUsed = true;
        token.IsRevoked = true;

        var user = token.User;
        user.UpdatedAt = DateTime.UtcNow;

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct();

        var newAccessToken = _tokenService.GenerateAccessToken(user, permissions);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = newRefreshToken,
            Expires = DateTime.UtcNow.AddDays(1),
            IsUsed = false,
            IsRevoked = false
        });

        await _context.SaveChangesAsync();

        return Ok(new
        {
            token = newAccessToken,
            refreshToken = newRefreshToken
        });
    }

  

    // 🔐 Logout
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? User.FindFirst("sub")
                          ?? User.FindFirst("UserId");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return Unauthorized("❌ User ID missing or invalid.");

        // ✅ اجلب كل التوكنات القديمة الخاصة بالمستخدم سواء كانت مستعملة أو لا
        var allUserTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked) // يمكن إزالة !rt.IsRevoked لو أردت تكرار التحديث
            .ToListAsync();

        foreach (var token in allUserTokens)
        {
            token.IsRevoked = true;
            //token.RevokedAt = DateTime.UtcNow;
        }

        // ✅ تحديث معلومات تسجيل الخروج
        var user = await _context.Users.FindAsync(userId);
        if (user != null)
        {
            user.LastLogoutAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        return Ok("👋 Logged out and all tokens revoked.");
    }



    // 🔑 Forget Password (Step 1: Just email check)
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        if (user == null)
            return NotFound("❌ Email not found.");

        // ✅ Generate Reset Token (guid + base64 encoding)
        var resetToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var resetUrl = $"https://localhost:7188/Account/ResetPassword?token={resetToken}&email={email}";

        user.PasswordResetToken = resetToken;
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
        await _context.SaveChangesAsync();

        // ✅ Send Email
        var smtpClient = new SmtpClient(_config["EmailSettings:Host"])
        {
            Port = int.Parse(_config["EmailSettings:Port"]),
            Credentials = new NetworkCredential(_config["EmailSettings:Email"], _config["EmailSettings:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["EmailSettings:Email"]),
            Subject = "🔐 Reset Your Password",
            Body = $"Hello {user.Username},\n\nClick here to reset your password:\n{resetUrl}\n\nThis link expires in 1 hour.",
            IsBodyHtml = false,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);

        return Ok("📩 Reset link sent.");
    }

    // ✅ Reset Password (Step 2)
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] UserDto dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

        if (user == null || user.PasswordResetToken != dto.Token || user.PasswordResetTokenExpires < DateTime.UtcNow)
            return BadRequest("❌ Invalid or expired token.");

        user.PasswordHash = PasswordHelper.HashPassword(dto.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;

        await _context.SaveChangesAsync();
        return Ok("✅ Password changed successfully.");
    }

  
    [HttpGet("get-id-by-username/{username}")]
    public async Task<IActionResult> GetUserIdByUsername(string username)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

        if (user == null)
            return NotFound("❌ User not found");

        return Ok(new { userId = user.Id.ToString() });
    }


}

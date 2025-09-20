using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Users.Application.DTOs.Users;
using Users.Domain.Entities;
using Users.Infrastructure.Persistence;
using BC = BCrypt.Net.BCrypt;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/[controller]")]
    
    [Authorize(Roles = "ROLE_SUPERADMIN")]
    public class AdminController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public AdminController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == dto.Username || u.Email == dto.Email))
                return BadRequest("❌ Username or Email already exists.");

            var branch = await _context.StoreBranches.FirstOrDefaultAsync(b => b.Id == dto.StoreBranchId);
            if (branch == null)
                return BadRequest("❌ Invalid store branch.");

            var userId = Guid.NewGuid();

            var user = new User
            {
                Id = userId,
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BC.HashPassword(dto.Password),
                IsActive = true,
                IsConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                StoreBranchId = dto.StoreBranchId,
                Profile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FullName = dto.FullName,
                    Gender = dto.Gender,
                    ProfilePicture = new byte[0] 
                },
                UserRoles = dto.RoleIds.Select(rid => new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoleId = rid
                }).ToList()
            };


            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Ok("✅ User registered successfully.");
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.StoreBranch)
                .Include(u => u.UserRoles).ThenInclude(ur => ur.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    u.IsConfirmed,
                    u.CreatedAt,
                    Branch = u.StoreBranch.Location,
                    Profile = new
                    {
                        u.Profile.FullName,
                        u.Profile.Gender,
                        Picture = Convert.ToBase64String(u.Profile.ProfilePicture ?? new byte[0])
                    },
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
                    Permissions = u.UserRoles
                                  .SelectMany(r => r.Role.RolePermissions)
                                  .Select(p => p.Permission.Name)
                                  .Distinct()
                                  .ToList()
                })
                .ToListAsync();

            return Ok(users);
        }


        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.StoreBranch)
                .Include(u => u.UserRoles).ThenInclude(r => r.Role).ThenInclude(r => r.RolePermissions).ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("❌ User not found.");

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.IsActive,
                user.IsConfirmed,
                user.CreatedAt,
                Branch = user.StoreBranch.Name,
                Profile = new
                {
                    user.Profile.FullName,
                    user.Profile.Gender,
                    Picture = Convert.ToBase64String(user.Profile.ProfilePicture ?? new byte[0])
                },
                Roles = user.UserRoles.Select(r => r.Role.Name).ToList(),
                Permissions = user.UserRoles
                                 .SelectMany(r => r.Role.RolePermissions)
                                 .Select(p => p.Permission.Name)
                                 .Distinct()
                                 .ToList()
            });
        }


        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserFromAdminDto dto)
        {
            var user = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.UserRoles)
                .Include(u => u.UserPermissions)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("❌ User not found.");

            user.Email = dto.Email;
            user.UpdatedAt = DateTime.UtcNow;

            user.Profile.FullName = dto.FullName;
            user.Profile.Gender = dto.Gender;
            user.Profile.ProfilePicture = dto.ProfilePicture; 

            // ✅ Update Roles
            user.UserRoles.Clear();
            user.UserRoles = dto.RoleIds.Select(rid => new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                RoleId = rid
            }).ToList();

            // ✅ Update Permissions
            user.UserPermissions.Clear();
            if (dto.PermissionIds != null && dto.PermissionIds.Any())
            {
                user.UserPermissions = dto.PermissionIds.Select(pid => new UserPermission
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    PermissionId = pid
                }).ToList();
            }

            await _context.SaveChangesAsync();
            return Ok("✅ User updated.");
        }

        [HttpPut("users/freeze/{id}")]
        public async Task<IActionResult> FreezeUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("❌ User not found.");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("❄️ User frozen (disabled).");
        }

        [HttpPut("users/activate/{id}")]
        public async Task<IActionResult> ActivateUser(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return NotFound("❌ User not found.");

            user.IsActive = true;
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok("✅ User activated.");
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _context.Users
                .Include(u => u.UserAnswers) 
                .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
                return NotFound("❌ User not found.");

            // ✅ Soft Delete Flags
            user.IsDeleted = true;
            user.IsActive = false;
            user.IsConfirmed = false;
            user.HasAnsweredSecurityQuestions = false;
            user.DeletedAt = DateTime.UtcNow;
            user.DeletedBy = GetCurrentUserId();

            
            if (user.UserAnswers != null && user.UserAnswers.Any())
            {
                _context.UserSecurityAnswers.RemoveRange(user.UserAnswers);
                user.UserAnswers.Clear();
            }


            await _context.SaveChangesAsync();

            return Ok("🗑️ User and all related answers soft deleted.");
        }

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)
                            ?? User.FindFirst("sub")
                            ?? User.FindFirst("UserId");

            return Guid.TryParse(userIdClaim?.Value, out var userId) ? userId : (Guid?)null;
        }


        [HttpPost("users/{userId}/assign-security-questions")]
        public async Task<IActionResult> AssignSecurityQuestionsToUser(Guid userId, [FromBody] List<SecurityAnswerDto> answers)
        {
            var user = await _context.Users
                .Include(u => u.UserAnswers)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return NotFound("❌ User not found.");

            if (answers == null || answers.Count < 1)
                return BadRequest("❌ At least one security answer is required.");

            var questionIds = answers.Select(a => a.QuestionId).ToList();
            var validQuestions = await _context.SecurityQuestions
                .Where(q => questionIds.Contains(q.Id))
                .Select(q => q.Id)
                .ToListAsync();

            if (validQuestions.Count != questionIds.Count)
                return BadRequest("❌ One or more questions are invalid.");

            user.UserAnswers.Clear();

            foreach (var answer in answers)
            {
                user.UserAnswers.Add(new UserSecurityAnswer
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    QuestionId = answer.QuestionId,
                    AnswerHash = BC.HashPassword(answer.Answer)
                });
            }

            await _context.SaveChangesAsync();
            return Ok("✅ Security questions assigned successfully.");
        }



        [HttpGet("users/search")]
        public async Task<IActionResult> SearchUsers(string? keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return BadRequest("❌ Search keyword is required.");

            keyword = keyword.Trim().ToLower();

            var users = await _context.Users
                .Include(u => u.Profile)
                .Include(u => u.StoreBranch)
                .Include(u => u.UserRoles).ThenInclude(r => r.Role)
                .Include(u => u.UserPermissions).ThenInclude(up => up.Permission)
                .Where(u =>
                    u.Username.ToLower().Contains(keyword) ||
                    u.Email.ToLower().Contains(keyword) ||
                    u.Profile.FullName.ToLower().Contains(keyword) ||
                    u.Profile.Gender.ToLower().Contains(keyword) ||
                    u.StoreBranch.Name.ToLower().Contains(keyword) ||
                    u.UserRoles.Any(r => r.Role.Name.ToLower().Contains(keyword)) ||
                    u.UserPermissions.Any(p => p.Permission.Name.ToLower().Contains(keyword))
                )
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    u.CreatedAt,
                    Branch = u.StoreBranch.Name,
                    Profile = new
                    {
                        u.Profile.FullName,
                        u.Profile.Gender,
                        Picture = Convert.ToBase64String(u.Profile.ProfilePicture ?? new byte[0])
                    },
                    Roles = u.UserRoles.Select(r => r.Role.Name).ToList(),
                    Permissions = u.UserPermissions.Select(p => p.Permission.Name).ToList()
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Select(r => new
                {
                    r.Id,
                    r.Name
                })
                .ToListAsync();

            return Ok(roles);
        }

       
        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var permissions = await _context.Permissions
                .Select(p => new
                {
                    p.Id,
                    p.Name
                })
                .ToListAsync();

            return Ok(permissions);
        }

       
        [HttpGet("storebranches")]
        public async Task<IActionResult> GetBranches()
        {
            var branches = await _context.StoreBranches
                .Select(b => new
                {
                    b.Id,
                    b.Location
                })
                .ToListAsync();

            return Ok(branches);
        }


    }
}

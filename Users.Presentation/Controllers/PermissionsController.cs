
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Domain.Entities;
using Users.Infrastructure.Persistence;

namespace Users.Presentation.Controllers
{
    [ApiController]
    [Route("api/users/[controller]")]
    [Authorize(Roles = "ROLE_SUPERADMIN")]
    public class PermissionsController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public PermissionsController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet("ping")]
        public IActionResult Ping() => Ok("✅ HR Module is alive!");

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Permissions
                .Where(p => !p.IsDeleted)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.Code,
                    p.Description
                })
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var permission = await _context.Permissions
                .Where(p => !p.IsDeleted && p.Id == id)
                .FirstOrDefaultAsync();

            if (permission == null)
                return NotFound();

            return Ok(permission);
        }


        // ✅ Create Permission
        [HttpPost]
        public async Task<IActionResult> Create(Permission permission)
        {
            permission.Id = Guid.NewGuid();
            await _context.Permissions.AddAsync(permission);
            await _context.SaveChangesAsync();
            return Ok(permission);
        }

        // ✅ Update Permission
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, Permission updated)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            permission.Name = updated.Name;
            permission.Code = updated.Code;
            permission.Description = updated.Description;

            await _context.SaveChangesAsync();
            return Ok(permission);
        }

        // ✅ Delete Permission
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete(Guid id)
        //{
        //    var permission = await _context.Permissions.FindAsync(id);
        //    if (permission == null)
        //        return NotFound();

        //    _context.Permissions.Remove(permission);
        //    await _context.SaveChangesAsync();
        //    return Ok("🗑️ Deleted successfully");
        //}

        // ✅ Soft Delete Permission
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var permission = await _context.Permissions.FindAsync(id);
            if (permission == null)
                return NotFound();

            permission.IsDeleted = true;

            await _context.SaveChangesAsync();
            return Ok("🗑️ Permission soft-deleted successfully.");
        }

       

    }
}

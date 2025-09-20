
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Users.Application.DTOs.Users;
using Users.Domain.Entities;
using Users.Infrastructure.Persistence;

namespace ERP.API.API.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "ROLE_SUPERADMIN")]
    public class RolesController : ControllerBase
    {
        private readonly UsersDbContext _context;

        public RolesController(UsersDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    r.Code,
                    r.Description,
                    Permissions = r.RolePermissions.Select(rp => new
                    {
                        rp.PermissionId,
                        rp.Permission.Name,
                        rp.Permission.Code
                    })
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(Guid id)
        {
            var role = await _context.Roles
                .Where(r => !r.IsDeleted && r.Id == id)
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync();

            if (role == null)
                return NotFound();

            return Ok(new
            {
                role.Id,
                role.Name,
                role.Code,
                role.Description,
                Permissions = role.RolePermissions.Select(rp => new
                {
                    rp.PermissionId,
                    rp.Permission.Name,
                    rp.Permission.Code
                })
            });
        }


        // ✅ 3. Create Role
        [HttpPost]
        public async Task<IActionResult> CreateRole([FromBody] RoleCreateDto dto)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
                {
                    RoleId = Guid.NewGuid(), // سيتم استبداله بعد إضافة الدور
                    PermissionId = pid
                }).ToList()
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            // تحديث RoleId داخل RolePermissions
            foreach (var rp in role.RolePermissions)
                rp.RoleId = role.Id;

            await _context.SaveChangesAsync();

            return Ok("✅ Role created successfully.");
        }

        // ✅ 4. Update Role
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] RoleCreateDto dto)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            role.Name = dto.Name;
            role.Code = dto.Code;
            role.Description = dto.Description;

            // تحديث الصلاحيات (حذف القديمة ثم إضافة الجديدة)
            _context.RolePermissions.RemoveRange(role.RolePermissions);
            role.RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
            {
                RoleId = id,
                PermissionId = pid
            }).ToList();

            await _context.SaveChangesAsync();
            return Ok("✅ Role updated successfully.");
        }

        //// ✅ 5. Delete Role
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteRole(Guid id)
        //{
        //    var role = await _context.Roles
        //        .Include(r => r.RolePermissions)
        //        .FirstOrDefaultAsync(r => r.Id == id);

        //    if (role == null)
        //        return NotFound();

        //    _context.RolePermissions.RemoveRange(role.RolePermissions);
        //    _context.Roles.Remove(role);

        //    await _context.SaveChangesAsync();
        //    return Ok("✅ Role deleted successfully.");
        //}

        // ✅ 5. Soft Delete Role
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var role = await _context.Roles
                .Include(r => r.RolePermissions)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
                return NotFound();

            // Soft Delete
            role.IsDeleted = true;
            _context.RolePermissions.RemoveRange(role.RolePermissions); // الصلاحيات مازالت تُحذف

            await _context.SaveChangesAsync();
            return Ok("🗑️ Role soft-deleted successfully.");
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> CreateRolesBulk([FromBody] List<RoleCreateDto> rolesDto)
        {
            var roles = new List<Role>();

            foreach (var dto in rolesDto)
            {
                var roleId = Guid.NewGuid();

                var role = new Role
                {
                    Id = roleId,
                    Name = dto.Name,
                    Code = dto.Code,
                    Description = dto.Description,
                    RolePermissions = dto.PermissionIds.Select(pid => new RolePermission
                    {
                        RoleId = roleId,
                        PermissionId = pid
                    }).ToList()
                };

                roles.Add(role);
            }

            await _context.Roles.AddRangeAsync(roles);
            await _context.SaveChangesAsync();

            return Ok("✅ Roles created successfully.");
        }

    }


}

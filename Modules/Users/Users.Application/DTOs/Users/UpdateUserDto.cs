

namespace Users.Application.DTOs.Users
{
    public class UpdateUserDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public List<Guid> RoleIds { get; set; }
    }
}

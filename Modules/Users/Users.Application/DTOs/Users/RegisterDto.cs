namespace Users.Application.DTOs.Users
{
    public class RegisterDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public Guid StoreBranchId { get; set; }
        public List<Guid> RoleIds { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }

}

namespace Users.Application.DTOs.Users
{
    public class UpdateUserFromAdminDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public byte[] ProfilePicture { get; set; }

        public List<Guid> RoleIds { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }
}

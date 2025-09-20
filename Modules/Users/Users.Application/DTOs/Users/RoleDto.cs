namespace Users.Application.DTOs.Users
{
    public class RoleDto
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public List<Guid> PermissionIds { get; set; }
    }

}

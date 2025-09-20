using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class RolePermission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}

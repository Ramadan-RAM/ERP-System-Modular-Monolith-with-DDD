using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class UserPermission
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();       
        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; }
    }
}

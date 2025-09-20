using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{

    public class UserRole
    {
        [Key]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }

        public Guid RoleId { get; set; }
        public Role Role { get; set; }

        //public Guid? DelegatedByUserId { get; set; } // for department manager delegation
    }

}

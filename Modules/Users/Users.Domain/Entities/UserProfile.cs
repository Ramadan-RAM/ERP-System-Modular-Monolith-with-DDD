using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class UserProfile
    {
        [Key]
        public Guid Id { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public byte[] ProfilePicture { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
    }

}

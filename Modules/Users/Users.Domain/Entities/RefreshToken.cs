using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; } = false;
        public bool IsUsed { get; set; } = false;

        public Guid UserId { get; set; }
        public User User { get; set; }
    }


}

using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class UserSecurityAnswer
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

        [Required]
        public Guid QuestionId { get; set; }
        public SecurityQuestion Question { get; set; }

        [Required]
        public string AnswerHash { get; set; }

        public string AnswerEncrypted { get; set; } 
    }
}

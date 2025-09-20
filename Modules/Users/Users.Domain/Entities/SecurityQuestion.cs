using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class SecurityQuestion
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string QuestionText { get; set; }

        public ICollection<UserSecurityAnswer> UserAnswers { get; set; }
    }
}

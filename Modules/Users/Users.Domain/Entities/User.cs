using System.ComponentModel.DataAnnotations;

namespace Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;
        public bool IsConfirmed { get; set; } = false;

        // 🔐 Reset Password Token Fields
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpires { get; set; }

        // ✅ Audit
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime? LastLogoutAt { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime? DeletedAt { get; set; }
        public Guid? DeletedBy { get; set; }

        public bool HasAnsweredSecurityQuestions { get; set; } = false;

        // 🔗 Relations
        //public Guid? DepartmentId { get; set; }
        //public DepartmentManager? Department { get; set; }

        public Guid StoreBranchId { get; set; }
        public StoreBranch StoreBranch { get; set; }

        public UserProfile Profile { get; set; }

        public ICollection<UserPermission> UserPermissions { get; set; } = new List<UserPermission>();

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<UserSecurityAnswer> UserAnswers { get; set; } = new List<UserSecurityAnswer>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }


}

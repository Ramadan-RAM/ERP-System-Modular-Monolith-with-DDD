namespace Users.Application.DTOs.Users
{
    public class UserDto
    {
        public required string Email { get; set; } // Used to identify user
        public string? Token { get; set; }         // Reset token from link
        public string? NewPassword { get; set; }   // New password entered
    }


}

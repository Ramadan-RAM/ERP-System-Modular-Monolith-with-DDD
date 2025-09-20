namespace Users.Application.DTOs.Users
{
    public class ResetWithQuestionDto
    {
        public string Email { get; set; }
        public string Answer { get; set; }
        public string NewPassword { get; set; }
    }

}

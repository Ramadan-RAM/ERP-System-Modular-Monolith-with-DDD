namespace Users.Application.DTOs.Users
{
    // DTOs/Users/ChangePasswordWithQuestionsDto.cs
    public class ChangePasswordWithQuestionsDto
    {
        public string CurrentPassword { get; set; }
        public List<SecurityAnswerDto> Answers { get; set; } // The same dto we created earlier
        public string NewPassword { get; set; }
    }

}

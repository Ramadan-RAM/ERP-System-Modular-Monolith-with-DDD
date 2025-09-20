namespace Users.Application.DTOs.Users
{
    public class SecurityAnswersSubmitDto
    {
        public Guid UserId { get; set; }
        public List<SecurityAnswerDto> Answers { get; set; }
    }
}

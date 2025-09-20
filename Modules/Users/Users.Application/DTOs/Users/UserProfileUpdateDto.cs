using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;

namespace Users.Application.DTOs.Users
{
    public class UserProfileUpdateDto
    {
        public string FullName { get; set; }
        public string Gender { get; set; }

        [SwaggerSchema(Format = "binary")] 
        public IFormFile? ProfilePicture { get; set; }
    }

}

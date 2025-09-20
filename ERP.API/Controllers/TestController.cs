
using Microsoft.AspNetCore.Mvc;

namespace ERP.API.Controllers
{
   

    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get() => Ok("Swagger is working!");
    }
}

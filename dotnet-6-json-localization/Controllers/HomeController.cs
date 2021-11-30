using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace dotnet_6_json_localization.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IStringLocalizer<HomeController> _stringLocalizer;

        public HomeController(IStringLocalizer<HomeController> stringLocalizer)
        {
            _stringLocalizer = stringLocalizer;
        }
        [HttpGet]
        public IActionResult Get()
        {
            var message = _stringLocalizer["hi"].ToString();
            return Ok(message);
        }
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            var message = string.Format(_stringLocalizer["welcome"], name);
            return Ok(message);
        }
        [HttpGet("all")]
        public IActionResult GetAll()
        {
            var message = _stringLocalizer.GetAllStrings();
            return Ok(message);
        }
    }
}

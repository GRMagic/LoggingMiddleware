using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace LoggingMiddleware.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NamesController : ControllerBase
    {
        private static List<string> Names = new List<string> { "Joe", "Doe" };

        [HttpGet]
        public IEnumerable<string> Get() => Names;

        [HttpPost]
        public IActionResult Create([FromBody] ContractExample info)
        {
            if (Names.Contains(info.Name)) return BadRequest(new { Status = "Error", Message = "The name already exists." });
            Names.Add(info.Name);
            return Ok(new { Status = "Success", Message = "The name has been added." });
        }

        [HttpGet("{anything}")]
        public string Other([FromRoute]string anything, [FromQuery]string another)
        {
            return "Done";
        }

    }
}

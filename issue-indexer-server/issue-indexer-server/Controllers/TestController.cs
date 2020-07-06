using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace issue_indexer_server.Controllers {

    [Route("[controller]")]
    [ApiController]
    public class ApiController : ControllerBase {

        [HttpGet]
        public async Task<IActionResult> Get() {
            return Content("Up and running!");
        }
    }
}

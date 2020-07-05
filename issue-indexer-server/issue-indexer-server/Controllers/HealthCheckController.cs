using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace issue_indexer_server.Controllers {

    [ApiController]
    [Route("[controller]")]
    public class HealthCheckController : ControllerBase {

        [HttpGet]
        public async Task<ActionResult<HealthCheck>> Get() {
            var healthCheck = new HealthCheck() {
                Message = "Up and running!"
            };
            return healthCheck;
        }
    }
}

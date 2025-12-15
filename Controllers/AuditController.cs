using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Service;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuditController : ControllerBase
    {
        private readonly AuditService _auditService;

        public AuditController(AuditService auditService)
        {
            _auditService = auditService;
        }

        // POST /audit
        [HttpPost]
        public IActionResult AddAction([FromBody] UserAction action)
        {
            _auditService.RecordAction(action);
            return Ok(new { message = "Action recorded successfully." });
        }

        // GET /audit/user/{userId}
        [HttpGet("user/{userId}")]
        public IActionResult GetActionByUserId(int userId)
        {
            var actions = _auditService.GetActionByUserId(userId);
            return Ok(actions);
        }

        // GET /audit/time?start=yyyy-MM-ddTHH:mm:ss&end=yyyy-MM-ddTHH:mm:ss
        [HttpGet("time")]
        public IActionResult GetActionByRange([FromQuery] DateTime start, [FromQuery] DateTime end)
        {
            var actions = _auditService.GetActionByTimeRange(start, end);
            return Ok(actions);
        }
    }
}

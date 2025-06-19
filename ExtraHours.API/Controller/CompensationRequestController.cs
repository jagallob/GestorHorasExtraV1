using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ExtraHours.API.Model;
using ExtraHours.API.Service.Interface;
using Microsoft.AspNetCore.Authorization;

namespace ExtraHours.API.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class CompensationRequestController : ControllerBase
    {
        private readonly ICompensationRequestService _service;
        public CompensationRequestController(ICompensationRequestService service)
        {
            _service = service;
        }

        // POST: api/CompensationRequest
        [HttpPost]
        [Authorize(Roles = "Empleado")]
        public async Task<ActionResult<CompensationRequest>> Create([FromBody] CompensationRequest request)
        {
            var created = await _service.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // GET: api/CompensationRequest
        [HttpGet]
        [Authorize(Roles = "Manager,Superusuario")]
        public async Task<ActionResult<IEnumerable<CompensationRequest>>> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        // GET: api/CompensationRequest/{id}
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<CompensationRequest>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        // GET: api/CompensationRequest/employee/{employeeId}
        [HttpGet("employee/{employeeId}")]
        [Authorize(Roles = "Empleado,Manager,Superusuario")]
        public async Task<ActionResult<IEnumerable<CompensationRequest>>> GetByEmployeeId(long employeeId)
        {
            var list = await _service.GetByEmployeeIdAsync(employeeId);
            return Ok(list);
        }

        // PUT: api/CompensationRequest/{id}/status
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Manager,Superusuario")]
        public async Task<ActionResult<CompensationRequest>> UpdateStatus(int id, [FromBody] StatusUpdateDto dto)
        {
            var updated = await _service.UpdateStatusAsync(id, dto.Status, dto.Justification, dto.ApprovedById);
            if (updated == null) return NotFound();
            return Ok(updated);
        }

        // DELETE: api/CompensationRequest/{id}
        [HttpDelete("{id}")]
        [Authorize(Roles = "Superusuario")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound();
            return NoContent();
        }
    }

    public class StatusUpdateDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Justification { get; set; }
        public long? ApprovedById { get; set; }
    }
}

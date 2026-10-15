using AppointmentQueue.Api.Models;
using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentQueue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OffDaysController : ControllerBase
{
    private readonly IOffDayService _service;

    public OffDaysController(IOffDayService service)
    {
        _service = service;
    }

    /// <summary>List all off days (for example public holidays).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OffDay>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<OffDay>> GetAll() => Ok(_service.GetAll());

    /// <summary>Mark a date as an off day. No appointments will be given on that date.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(OffDay), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public IActionResult Add([FromBody] AddOffDayRequest request)
    {
        var offDay = _service.Add(request.Date, request.Description);
        return CreatedAtAction(nameof(GetAll), offDay);
    }

    /// <summary>Remove an off day.</summary>
    /// <param name="date">The date to open again, formatted yyyy-MM-dd.</param>
    [HttpDelete("{date}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Remove(DateOnly date)
    {
        return _service.Remove(date) ? NoContent() : NotFound();
    }
}

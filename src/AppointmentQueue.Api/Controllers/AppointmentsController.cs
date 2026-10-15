using AppointmentQueue.Api.Models;
using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentQueue.Api.Controllers;

/// <summary>
/// Thin web layer: it receives the request, calls the business service and returns the result.
/// All booking rules live in the Business project.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;
    private readonly IClock _clock;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(
        IAppointmentService service,
        IClock clock,
        ILogger<AppointmentsController> logger)
    {
        _service = service;
        _clock = clock;
        _logger = logger;
    }

    /// <summary>Book an appointment and get a queue token.</summary>
    /// <remarks>
    /// If the requested day is an off day or already full, the appointment is moved to the
    /// next available day. Check appointmentDate in the response.
    /// </remarks>
    [HttpPost]
    [ProducesResponseType(typeof(Appointment), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public IActionResult Book([FromBody] BookAppointmentRequest request)
    {
        var appointment = _service.Book(request.CustomerName, request.Date);

        _logger.LogInformation(
            "Booked token {Token} for {Customer} on {Date} (requested {Requested})",
            appointment.TokenNumber, appointment.CustomerName,
            appointment.AppointmentDate, appointment.RequestedDate);

        return CreatedAtAction(
            nameof(GetQueue),
            new { date = appointment.AppointmentDate.ToString("yyyy-MM-dd") },
            appointment);
    }

    /// <summary>See the queue (grid) of appointments for one day.</summary>
    /// <param name="date">Day to show, formatted yyyy-MM-dd. Defaults to today.</param>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Appointment>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Appointment>> GetQueue([FromQuery] DateOnly? date)
    {
        var day = date ?? _clock.Today;
        return Ok(_service.GetQueue(day));
    }
}

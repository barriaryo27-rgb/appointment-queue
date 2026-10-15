using AppointmentQueue.Api.Models;
using AppointmentQueue.Business.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AppointmentQueue.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly IAgencySettings _settings;

    public SettingsController(IAgencySettings settings)
    {
        _settings = settings;
    }

    /// <summary>Show the maximum number of appointments allowed per day.</summary>
    [HttpGet("max-appointments")]
    [ProducesResponseType(typeof(MaxAppointmentsResponse), StatusCodes.Status200OK)]
    public ActionResult<MaxAppointmentsResponse> GetMaxAppointments() =>
        Ok(new MaxAppointmentsResponse(_settings.MaxAppointmentsPerDay));

    /// <summary>Change the maximum number of appointments allowed per day.</summary>
    /// <remarks>Customers who register after a day is full are moved to the next day.</remarks>
    [HttpPut("max-appointments")]
    [ProducesResponseType(typeof(MaxAppointmentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<MaxAppointmentsResponse> SetMaxAppointments(
        [FromBody] UpdateMaxAppointmentsRequest request)
    {
        _settings.SetMaxAppointmentsPerDay(request.MaxAppointmentsPerDay);
        return Ok(new MaxAppointmentsResponse(_settings.MaxAppointmentsPerDay));
    }
}

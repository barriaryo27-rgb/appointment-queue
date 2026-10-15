namespace AppointmentQueue.Api.Models;

/// <param name="CustomerName">Name of the customer to put in the queue.</param>
/// <param name="Date">Date the customer wants, formatted yyyy-MM-dd.</param>
public record BookAppointmentRequest(string CustomerName, DateOnly Date);

/// <param name="Date">The closed date, formatted yyyy-MM-dd.</param>
/// <param name="Description">Optional reason, for example "Public holiday".</param>
public record AddOffDayRequest(DateOnly Date, string? Description);

/// <param name="MaxAppointmentsPerDay">New daily limit (at least 1).</param>
public record UpdateMaxAppointmentsRequest(int MaxAppointmentsPerDay);

/// <param name="MaxAppointmentsPerDay">Current daily limit.</param>
public record MaxAppointmentsResponse(int MaxAppointmentsPerDay);

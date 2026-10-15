using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Interfaces;

/// <summary>The booking rules: issue tokens and show the queue.</summary>
public interface IAppointmentService
{
    /// <summary>
    /// Books an appointment starting from the requested date. If that day is an off day
    /// or full, the booking moves to the next available day.
    /// </summary>
    Appointment Book(string customerName, DateOnly requestedDate);

    /// <summary>The queue (grid) for one day, ordered by token number.</summary>
    IReadOnlyList<Appointment> GetQueue(DateOnly date);
}

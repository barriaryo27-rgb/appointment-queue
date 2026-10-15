using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Interfaces;

/// <summary>
/// Storage for appointments. The in-memory version can later be replaced by a database
/// version without touching the service or the API.
/// </summary>
public interface IAppointmentRepository
{
    /// <summary>Saves the appointment and assigns its Id.</summary>
    void Add(Appointment appointment);

    /// <summary>All appointments booked for the given date.</summary>
    IReadOnlyList<Appointment> GetByDate(DateOnly date);
}

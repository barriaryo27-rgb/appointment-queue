using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Repositories;

/// <summary>Keeps appointments in memory. Data is lost when the app restarts.</summary>
public class InMemoryAppointmentRepository : IAppointmentRepository
{
    private readonly List<Appointment> _appointments = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public void Add(Appointment appointment)
    {
        lock (_lock)
        {
            appointment.Id = _nextId++;
            _appointments.Add(appointment);
        }
    }

    public IReadOnlyList<Appointment> GetByDate(DateOnly date)
    {
        lock (_lock)
        {
            return _appointments
                .Where(a => a.AppointmentDate == date)
                .OrderBy(a => a.TokenNumber)
                .ToList();
        }
    }
}

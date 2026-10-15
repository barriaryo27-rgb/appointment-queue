using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Services;

public class AppointmentService : IAppointmentService
{
    // Safety limit so the search for a free day can never loop forever.
    private const int MaxDaysToSearch = 366;

    // Shared by every request, so two customers booking at the same moment
    // cannot receive the same token number.
    private static readonly object BookingLock = new();

    private readonly IAppointmentRepository _appointments;
    private readonly IOffDayRepository _offDays;
    private readonly IAgencySettings _settings;
    private readonly IClock _clock;

    public AppointmentService(
        IAppointmentRepository appointments,
        IOffDayRepository offDays,
        IAgencySettings settings,
        IClock clock)
    {
        _appointments = appointments;
        _offDays = offDays;
        _settings = settings;
        _clock = clock;
    }

    public Appointment Book(string customerName, DateOnly requestedDate)
    {
        if (string.IsNullOrWhiteSpace(customerName))
        {
            throw new ArgumentException("Customer name is required.");
        }

        if (requestedDate < _clock.Today)
        {
            throw new ArgumentException("Appointment date cannot be in the past.");
        }

        lock (BookingLock)
        {
            var date = FindAvailableDate(requestedDate);

            // Next token = highest token already used that day + 1 (starts at 1).
            var token = _appointments.GetByDate(date)
                .Select(a => a.TokenNumber)
                .DefaultIfEmpty(0)
                .Max() + 1;

            var appointment = new Appointment
            {
                CustomerName = customerName.Trim(),
                RequestedDate = requestedDate,
                AppointmentDate = date,
                TokenNumber = token
            };

            _appointments.Add(appointment);
            return appointment;
        }
    }

    public IReadOnlyList<Appointment> GetQueue(DateOnly date) =>
        _appointments.GetByDate(date);

    /// <summary>
    /// Starting at the requested date, skips off days and full days
    /// until it finds a day that can take one more appointment.
    /// </summary>
    private DateOnly FindAvailableDate(DateOnly start)
    {
        var date = start;

        for (var i = 0; i < MaxDaysToSearch; i++)
        {
            var isOffDay = _offDays.IsOffDay(date);
            var isFull = _appointments.GetByDate(date).Count >= _settings.MaxAppointmentsPerDay;

            if (!isOffDay && !isFull)
            {
                return date;
            }

            date = date.AddDays(1);
        }

        throw new InvalidOperationException(
            $"No available day found within {MaxDaysToSearch} days of {start:yyyy-MM-dd}.");
    }
}

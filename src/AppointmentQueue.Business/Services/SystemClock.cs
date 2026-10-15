using AppointmentQueue.Business.Interfaces;

namespace AppointmentQueue.Business.Services;

/// <summary>The real clock. Uses UTC so the app behaves the same locally and on Azure.</summary>
public class SystemClock : IClock
{
    public DateOnly Today => DateOnly.FromDateTime(DateTime.UtcNow);
}

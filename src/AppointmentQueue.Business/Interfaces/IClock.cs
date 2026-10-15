namespace AppointmentQueue.Business.Interfaces;

/// <summary>
/// Gives the current date. Hidden behind an interface so unit tests can fix "today".
/// </summary>
public interface IClock
{
    DateOnly Today { get; }
}

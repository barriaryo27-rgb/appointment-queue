using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Interfaces;

/// <summary>Lets the agency manage its off days.</summary>
public interface IOffDayService
{
    OffDay Add(DateOnly date, string? description);

    /// <summary>Returns false if the date was not an off day.</summary>
    bool Remove(DateOnly date);

    IReadOnlyList<OffDay> GetAll();
}

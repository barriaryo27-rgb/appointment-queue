using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Interfaces;

/// <summary>Storage for the agency's off days.</summary>
public interface IOffDayRepository
{
    /// <summary>Adds an off day. Returns false if that date is already an off day.</summary>
    bool Add(OffDay offDay);

    /// <summary>Removes an off day. Returns false if the date was not an off day.</summary>
    bool Remove(DateOnly date);

    bool IsOffDay(DateOnly date);

    IReadOnlyList<OffDay> GetAll();
}

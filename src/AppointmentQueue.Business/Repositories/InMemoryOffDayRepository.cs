using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Repositories;

/// <summary>Keeps off days in memory. Data is lost when the app restarts.</summary>
public class InMemoryOffDayRepository : IOffDayRepository
{
    private readonly List<OffDay> _offDays = new();
    private readonly object _lock = new();

    public bool Add(OffDay offDay)
    {
        lock (_lock)
        {
            if (_offDays.Any(o => o.Date == offDay.Date))
            {
                return false;
            }

            _offDays.Add(offDay);
            return true;
        }
    }

    public bool Remove(DateOnly date)
    {
        lock (_lock)
        {
            return _offDays.RemoveAll(o => o.Date == date) > 0;
        }
    }

    public bool IsOffDay(DateOnly date)
    {
        lock (_lock)
        {
            return _offDays.Any(o => o.Date == date);
        }
    }

    public IReadOnlyList<OffDay> GetAll()
    {
        lock (_lock)
        {
            return _offDays.OrderBy(o => o.Date).ToList();
        }
    }
}

using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;

namespace AppointmentQueue.Business.Services;

public class OffDayService : IOffDayService
{
    private readonly IOffDayRepository _repository;

    // Dependencies come in through the constructor, so Autofac (or a test) can supply them.
    public OffDayService(IOffDayRepository repository)
    {
        _repository = repository;
    }

    public OffDay Add(DateOnly date, string? description)
    {
        var offDay = new OffDay
        {
            Date = date,
            Description = description?.Trim() ?? string.Empty
        };

        if (!_repository.Add(offDay))
        {
            throw new InvalidOperationException($"{date:yyyy-MM-dd} is already an off day.");
        }

        return offDay;
    }

    public bool Remove(DateOnly date) => _repository.Remove(date);

    public IReadOnlyList<OffDay> GetAll() => _repository.GetAll();
}

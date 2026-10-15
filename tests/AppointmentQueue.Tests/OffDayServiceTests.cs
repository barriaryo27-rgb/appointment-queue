using AppointmentQueue.Business.Repositories;
using AppointmentQueue.Business.Services;

namespace AppointmentQueue.Tests;

public class OffDayServiceTests
{
    private readonly OffDayService _service = new(new InMemoryOffDayRepository());

    [Fact]
    public void Add_NewDate_IsListed()
    {
        _service.Add(new DateOnly(2026, 12, 25), "Christmas");

        var all = _service.GetAll();

        Assert.Single(all);
        Assert.Equal("Christmas", all[0].Description);
    }

    [Fact]
    public void Add_SameDateTwice_Throws()
    {
        _service.Add(new DateOnly(2026, 12, 25), "Christmas");

        Assert.Throws<InvalidOperationException>(() => _service.Add(new DateOnly(2026, 12, 25), null));
    }

    [Fact]
    public void Remove_ExistingDate_ReturnsTrueAndRemovesIt()
    {
        _service.Add(new DateOnly(2026, 12, 25), "Christmas");

        Assert.True(_service.Remove(new DateOnly(2026, 12, 25)));
        Assert.Empty(_service.GetAll());
    }

    [Fact]
    public void Remove_UnknownDate_ReturnsFalse()
    {
        Assert.False(_service.Remove(new DateOnly(2026, 12, 25)));
    }

    [Fact]
    public void GetAll_ReturnsDatesInOrder()
    {
        _service.Add(new DateOnly(2026, 12, 25), "Christmas");
        _service.Add(new DateOnly(2026, 10, 20), "Cuti bersama");

        var dates = _service.GetAll().Select(o => o.Date).ToArray();

        Assert.Equal(new[] { new DateOnly(2026, 10, 20), new DateOnly(2026, 12, 25) }, dates);
    }
}

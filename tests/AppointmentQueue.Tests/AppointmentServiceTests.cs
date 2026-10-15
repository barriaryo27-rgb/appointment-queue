using AppointmentQueue.Business.Interfaces;
using AppointmentQueue.Business.Models;
using AppointmentQueue.Business.Repositories;
using AppointmentQueue.Business.Services;
using Moq;

namespace AppointmentQueue.Tests;

public class AppointmentServiceTests
{
    // "Today" is fixed in every test, so the results never depend on the real date.
    private static readonly DateOnly Today = new(2026, 10, 16); // a Friday

    private readonly InMemoryAppointmentRepository _appointments = new();
    private readonly InMemoryOffDayRepository _offDays = new();
    private readonly AgencySettings _settings = new();
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        // Moq creates a fake IClock that always says today is 16 Oct 2026.
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Today).Returns(Today);

        _service = new AppointmentService(_appointments, _offDays, _settings, clock.Object);
    }

    [Fact]
    public void Book_FirstCustomerOfTheDay_GetsTokenOne()
    {
        var result = _service.Book("Andi", Today);

        Assert.Equal(1, result.TokenNumber);
        Assert.Equal(Today, result.AppointmentDate);
    }

    [Fact]
    public void Book_SameDay_TokensIncreaseInOrder()
    {
        var first = _service.Book("Andi", Today);
        var second = _service.Book("Budi", Today);
        var third = _service.Book("Citra", Today);

        Assert.Equal(new[] { 1, 2, 3 }, new[] { first.TokenNumber, second.TokenNumber, third.TokenNumber });
    }

    [Fact]
    public void Book_DifferentDays_EachDayStartsAtTokenOne()
    {
        var today = _service.Book("Andi", Today);
        var tomorrow = _service.Book("Budi", Today.AddDays(1));

        Assert.Equal(1, today.TokenNumber);
        Assert.Equal(1, tomorrow.TokenNumber);
    }

    [Fact]
    public void Book_OnOffDay_MovesToNextDay()
    {
        _offDays.Add(new OffDay { Date = Today, Description = "Public holiday" });

        var result = _service.Book("Andi", Today);

        Assert.Equal(Today.AddDays(1), result.AppointmentDate);
        Assert.Equal(Today, result.RequestedDate);
        Assert.Equal(1, result.TokenNumber);
    }

    [Fact]
    public void Book_WhenDayIsFull_MovesToNextDay()
    {
        _settings.SetMaxAppointmentsPerDay(2);
        _service.Book("Andi", Today);
        _service.Book("Budi", Today);

        var overflow = _service.Book("Citra", Today);

        Assert.Equal(Today.AddDays(1), overflow.AppointmentDate);
        Assert.Equal(1, overflow.TokenNumber);
    }

    [Fact]
    public void Book_WhenDayIsFullAndNextDayIsOff_SkipsToTheFollowingDay()
    {
        _settings.SetMaxAppointmentsPerDay(1);
        _offDays.Add(new OffDay { Date = Today.AddDays(1) });
        _service.Book("Andi", Today);

        var result = _service.Book("Budi", Today);

        Assert.Equal(Today.AddDays(2), result.AppointmentDate);
    }

    [Fact]
    public void Book_SavesTheAppointmentExactlyOnce()
    {
        // A Moq fake repository lets us check how the service talks to storage.
        var repository = new Mock<IAppointmentRepository>();
        repository.Setup(r => r.GetByDate(It.IsAny<DateOnly>())).Returns(new List<Appointment>());
        var clock = new Mock<IClock>();
        clock.Setup(c => c.Today).Returns(Today);
        var service = new AppointmentService(repository.Object, _offDays, _settings, clock.Object);

        service.Book("Andi", Today);

        repository.Verify(r => r.Add(It.Is<Appointment>(a => a.CustomerName == "Andi")), Times.Once);
    }

    [Fact]
    public void Book_DateInThePast_Throws()
    {
        Assert.Throws<ArgumentException>(() => _service.Book("Andi", Today.AddDays(-1)));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Book_WithoutCustomerName_Throws(string? name)
    {
        Assert.Throws<ArgumentException>(() => _service.Book(name!, Today));
    }

    [Fact]
    public void GetQueue_ReturnsOnlyThatDay_OrderedByToken()
    {
        _service.Book("Andi", Today);
        _service.Book("Budi", Today);
        _service.Book("Citra", Today.AddDays(1));

        var queue = _service.GetQueue(Today);

        Assert.Equal(new[] { "Andi", "Budi" }, queue.Select(a => a.CustomerName).ToArray());
        Assert.Equal(new[] { 1, 2 }, queue.Select(a => a.TokenNumber).ToArray());
    }

    [Fact]
    public void GetQueue_DayWithNoAppointments_IsEmpty()
    {
        Assert.Empty(_service.GetQueue(Today));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void SetMaxAppointmentsPerDay_BelowOne_Throws(int value)
    {
        Assert.Throws<ArgumentException>(() => _settings.SetMaxAppointmentsPerDay(value));
    }
}

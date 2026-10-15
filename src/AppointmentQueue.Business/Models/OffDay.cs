namespace AppointmentQueue.Business.Models;

/// <summary>A day the agency is closed (for example a public holiday).</summary>
public class OffDay
{
    public DateOnly Date { get; set; }

    public string Description { get; set; } = string.Empty;
}

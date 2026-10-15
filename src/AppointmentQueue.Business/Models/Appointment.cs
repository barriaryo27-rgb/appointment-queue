namespace AppointmentQueue.Business.Models;

/// <summary>
/// One booked appointment. The token number is the customer's place in that day's queue.
/// </summary>
public class Appointment
{
    public int Id { get; set; }

    public string CustomerName { get; set; } = string.Empty;

    /// <summary>The date the customer asked for.</summary>
    public DateOnly RequestedDate { get; set; }

    /// <summary>
    /// The date actually given. It differs from RequestedDate when the requested day
    /// was an off day or already full.
    /// </summary>
    public DateOnly AppointmentDate { get; set; }

    /// <summary>Queue number for AppointmentDate (1, 2, 3, ...).</summary>
    public int TokenNumber { get; set; }
}

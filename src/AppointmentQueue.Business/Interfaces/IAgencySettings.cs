namespace AppointmentQueue.Business.Interfaces;

/// <summary>Agency-level settings that the agency can change at runtime.</summary>
public interface IAgencySettings
{
    /// <summary>Maximum number of appointments allowed on a single day.</summary>
    int MaxAppointmentsPerDay { get; }

    /// <summary>Changes the daily limit. The value must be at least 1.</summary>
    void SetMaxAppointmentsPerDay(int value);
}

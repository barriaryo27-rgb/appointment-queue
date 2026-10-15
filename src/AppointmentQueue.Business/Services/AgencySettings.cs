using AppointmentQueue.Business.Interfaces;

namespace AppointmentQueue.Business.Services;

public class AgencySettings : IAgencySettings
{
    private int _maxAppointmentsPerDay = 10;

    public int MaxAppointmentsPerDay => _maxAppointmentsPerDay;

    public void SetMaxAppointmentsPerDay(int value)
    {
        if (value < 1)
        {
            throw new ArgumentException("Maximum appointments per day must be at least 1.");
        }

        _maxAppointmentsPerDay = value;
    }
}

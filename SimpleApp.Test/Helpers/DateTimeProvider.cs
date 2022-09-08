using SimpleApp.Test.Helpers.Interfaces;

namespace SimpleApp.Test.Helpers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetNow() => DateTime.Now;

        public DateTime GetToday() => DateTime.Today;
    }
}

namespace SimpleApp.Test.Helpers
{
    public class DateTimeProvider
    {
        virtual public DateTime Now { get; } = DateTime.Now;

        virtual public DateTime Today { get; } = DateTime.Today;
    }
}

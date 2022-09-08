using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleApp.Test.Helpers.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime GetNow();

        DateTime GetToday();
    }
}

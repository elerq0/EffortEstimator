using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffortEstimator.Helpers.Exceptions
{
    public class NameAlreadyTakenException: Exception
    {
        public NameAlreadyTakenException(string message) : base(message) { }
    }
}

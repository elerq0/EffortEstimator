using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffortEstimator.Models
{
    public class UserEntity
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Hash { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public bool Active { get; set; }
    }
}

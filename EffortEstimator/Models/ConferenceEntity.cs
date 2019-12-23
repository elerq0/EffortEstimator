using System;

namespace EffortEstimator.Models
{
    public class ConferenceEntity
    {
        public int ConferenceId { get; set; }
        public string GroupName { get; set; }
        public string Topic { get; set; }
        public DateTime StartDate { get; set; }
        public string Description { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EffortEstimator.Models
{
    public class EstimationForm
    {
        public string Topic { get; set; }
        public string Description { get; set; }
        public int Iteration { get; set; }
        public double UserResultValue { get; set; }
        public double MinResultValue { get; set; }
        public double MaxResultValue { get; set; }
        public double AvgResultValue { get; set; }
        public double ProposedValue { get; set; }
        public List<double> ResultValues { get; set; }
    }
}

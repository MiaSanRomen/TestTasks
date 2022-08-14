using RegulatorApplication.Library.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Models
{
    public class RegulatorSettings : IRegulatorSettings
    {
        public RegulatorSettings(int budget, int interval)
        {
            Budget = budget;
            Interval = interval;
        }

        public int Budget { get; set; }
        public int Interval { get; set; }
    }
}

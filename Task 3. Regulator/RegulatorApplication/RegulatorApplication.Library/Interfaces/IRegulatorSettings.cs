using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Interfaces
{
    public interface IRegulatorSettings
    {
        public int Budget { get; set; }
        public int Interval { get; set; }
    }
}

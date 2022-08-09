using RegulatorApplication.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Interfaces
{
    internal interface IRegulator : ITransaction, IRegulatorLoad
    {
        EventHandler<decimal> OnLoadChanged { get; set; }

        void Propose(Request request);

        void Ignore(Request request);

        decimal GetEstimatedLoad();
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegulatorApplication.Library.Interfaces
{
    internal interface ITransaction
    {
        void Commit();
        void Rollback();
    }
}

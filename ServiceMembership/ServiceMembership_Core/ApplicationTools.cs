using ServiceMembership_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Core
{
    internal class ApplicationTools
    {
        internal static ISprocCalls InitSprocCall(ISprocCalls sprocCalls)
        {
            if (sprocCalls == null)
                return new SprocCalls();
            else
                return sprocCalls;
        }
    }
}

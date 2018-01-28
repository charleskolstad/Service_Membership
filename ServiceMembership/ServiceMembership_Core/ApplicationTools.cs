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

        internal static ISprocCalls InitSprocCall(bool isTest)
        {
            if (isTest)
                return new FakeSprocCalls();
            else
                return new SprocCalls();
        }

        internal static IProvider InitProvider(bool isTest)
        {
            if (isTest)
                return new FakeProvider();
            else
                return new Provider();
        }
    }
}

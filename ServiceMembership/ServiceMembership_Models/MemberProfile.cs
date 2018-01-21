using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Models
{
    public class MemberProfile
    {
        public string UserName { get; set; }
        public int ProfileID { get; set; }
        public bool Active { get; set; }
    }
}

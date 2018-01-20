using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Models
{
    public class Profile
    {
        public int ProfileID { get; set; }
        public string ProfileName { get; set; }
        public string ProfileDescription { get; set; }
        public int ProfileLevel { get; set; }
    }
}

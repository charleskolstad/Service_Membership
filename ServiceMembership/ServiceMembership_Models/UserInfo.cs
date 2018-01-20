using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ServiceMembership_Models
{
    public class UserInfo: MembershipUser
    {
        public UserInfo()
        {
            this.UserProfiles = new List<string>();
        }

        public int UserInfoID { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<string> UserProfiles { get; set; }
    }
}

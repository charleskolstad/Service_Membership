using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Core
{
    public class ProfileManager
    {
        public static List<Profile> GetAllProfiles(bool isTest = false)
        {
            try
            {
                ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);
                DataTable profileTable = sprocCalls.ProfileGetAll();
                List<Profile> profileList = new List<Profile>();
                Profile profile;

                foreach (DataRow row in profileTable.Rows)
                {
                    profile = new Profile();
                    profile.ProfileDescription = row["ProfileDescription"].ToString();
                    profile.ProfileID = Convert.ToInt32(row["ProfileID"]);
                    profile.ProfileLevel = Convert.ToInt32(row["ProfileLevel"]);
                    profile.ProfileName = row["ProfileName"].ToString();
                    profile.Active = false;

                    profileList.Add(profile);
                }

                return profileList;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return null;
            }
        }
    }
}

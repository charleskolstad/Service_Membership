using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ServiceMembership_Core
{
    public class UserManager
    {
        public static string CreateUser(string userName, string email)
        {
            try
            {
                Provider provider = new Provider();
                Membership.CreateUser(userName, "p@ssword1", email);
                return userName + "'s profile created successfully!";
            }
            catch (MembershipCreateUserException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return "Error occured creating the user";
            }
        }

        public static string UpdateUser(UserInfo uInfo, ISprocCalls sprocCalls = null)
        {
            try
            {
                Provider provider = new Provider();

                if (UpdateMembershipEmail(uInfo))
                {
                    sprocCalls = ApplicationTools.InitSprocCall(sprocCalls);

                    DataTable profileTable = ConvertProfileToTable(uInfo.UserProfiles);
                    string adminUser = Membership.GetUser().UserName;
                    if (sprocCalls.UserInfoUpdate(uInfo, profileTable, adminUser))
                        return uInfo.UserName + "'s profile updated successfully!";
                }

                return "Error updating " + uInfo.UserName;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return "Error - " + ex.Message;
            }
        }

        private static DataTable ConvertProfileToTable(List<MemberProfile> profileList)
        {
            DataTable profileTable = new DataTable();
            profileTable.Columns.Add("ProfileID");
            profileTable.Columns.Add("UserName");
            profileTable.Columns.Add("Active");
            DataRow row;

            foreach (MemberProfile profile in profileList)
            {
                row = profileTable.NewRow();
                row["ProfileID"] = profile.ProfileID;
                row["UserName"] = profile.UserName;
                row["Active"] = profile.Active;

                profileTable.Rows.Add(row);
            }

            return profileTable;
        }

        private static bool UpdateMembershipEmail(UserInfo uInfo)
        {
            try
            {
                MembershipUser user = Membership.GetUser(uInfo.UName);
                if (uInfo.Email != user.Email)
                {
                    user.Email = uInfo.Email;
                    Membership.UpdateUser(user);
                }

                return true;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
            }
        }

        public static string DeleteUser(string userName)
        {
            try
            {
                Provider provider = new Provider();

                if (Membership.DeleteUser(userName))
                    return "Deleted user successfully.";
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }

            return "Error deleting user.";
        }

        public static bool RecoverPassword(RecoverModel model)
        {

        }
    }
}

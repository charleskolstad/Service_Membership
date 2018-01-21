using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ServiceMembership_Core
{
    public class UserManager
    {
        public static string CreateUser(UserInfo userInfo, string adminUser, ISprocCalls sprocCalls = null)
        {
            try
            {
                Provider provider = new Provider();
                Membership.CreateUser(userInfo.UserName, "p@ssword1", userInfo.Email);

                sprocCalls = ApplicationTools.InitSprocCall(sprocCalls);
                if (!string.IsNullOrEmpty(sprocCalls.UserInfoInsert(userInfo, adminUser)))
                {
                    SendNotificationMail(userInfo, UserManagerActions.create);
                    return userInfo.UserName + "'s profile created successfully!";
                }
            }
            catch (MembershipCreateUserException ex)
            {
                return ex.Message;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }

            return "Error occured creating the user";
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
                    {
                        SendNotificationMail(uInfo, UserManagerActions.update);
                        return uInfo.UserName + "'s profile updated successfully!";
                    }
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

        public static string DeleteUser(UserInfo uInfo)
        {
            try
            {
                Provider provider = new Provider();

                if (Membership.DeleteUser(uInfo.UName))
                {
                    SendNotificationMail(uInfo, UserManagerActions.delete);
                    return "Deleted user successfully.";
                }
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }

            return "Error deleting user.";
        }

        public static bool RecoverPassword(RecoverModel model)
        {
            MembershipUser user = Membership.GetUser(model.UserName);
            if (user.Email == model.Email)
            {

            }
        }

        public static bool ChangePassword(string userName, string password)
        {
            try
            {
                MembershipUser user = Membership.GetUser(userName);
                user.ChangePassword(user.GetPassword(), password);
                return true;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
            }
        }

        private static void SendNotificationMail(UserInfo user, UserManagerActions action)
        {
            MailAddress fromAddress = new MailAddress("charlespkolstad@gmail.com", "Charles");
            MailAddress toAddress = new MailAddress(user.Email, user.UName);
            const string subject = "Subject";
            const string body = "Body";

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.EnableSsl = false;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential("charlespkolstad@gmail.com", "charles020810kolstad");
            MailMessage message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = body;

            smtp.Send(message);
        }

        private static string SetMessageBody(UserManagerActions action)
        {
            StringBuilder message = new StringBuilder();
            switch (action)
            {
                case UserManagerActions.create:
                    message.Append("A new account has been created for you!");
                    message.Append("")
                    break;
                case UserManagerActions.delete:
                    break;
                case UserManagerActions.recoverPass:
                    break;
                case UserManagerActions.update:
                    break;
            }

            return message;
        }
    }

    internal enum UserManagerActions
    {
        create,
        update,
        recoverPass,
        delete
    }
}

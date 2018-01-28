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
        public static string CreateUser(UserInfo userInfo, string adminUser, bool isTest = false)
        {
            try
            {
                IProvider provider = ApplicationTools.InitProvider(isTest);
                ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);

                string providerMessage = provider.MembershipActionCreate(userInfo.UName, userInfo.Email);
                if (string.IsNullOrEmpty(providerMessage))
                {
                    if (string.IsNullOrEmpty(sprocCalls.UserInfoInsert(userInfo, adminUser)))
                        return "Error saving user's info";
                }
                else
                    return providerMessage;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return "Error creating user.";
            }

            return string.Empty;
        }

        public static string UpdateUser(UserInfo uInfo, string adminUser, bool isTest = false)
        {
            IProvider provider = ApplicationTools.InitProvider(isTest);
            ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);

            if (provider.MembershipActionChangeEmail(uInfo.UName, uInfo.Email))
            {
                if (!sprocCalls.UserInfoUpdate(uInfo, ConvertProfileToTable(uInfo.UserProfiles), adminUser))
                    return "Error updating user info.";
            }
            else
                return "Error changing email";

            return string.Empty;
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

        public static string DeleteUser(string user, string adminUser)
        {
            try
            {
                Provider provider = new Provider();
                
                if (Membership.DeleteUser(user))
                {
                    UserInfo userInfo = GetUserInfo(Membership.GetUser(adminUser), new SprocCalls());
                    if(userInfo != null)
                        SendNotificationMail(userInfo, UserManagerActions.delete);

                    return "Deleted user successfully.";
                }
                else
                    throw new Exception("Error deleting user");
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return "Error deleting user.";   
            }            
        }

        public static bool RecoverPassword(RecoverModel model, ISprocCalls sprocCalls = null)
        {
            try
            {
                Provider provider = new Provider();
                sprocCalls = ApplicationTools.InitSprocCall(sprocCalls);

                MembershipUser user = Membership.GetUser(model.UserName);
                if (user.Email == model.Email)
                {
                    user.ChangePassword(user.GetPassword(), Membership.GeneratePassword(9, 1));
                    SetMessageBody(GetUserInfo(user, sprocCalls), UserManagerActions.recoverPass);
                    return true;
                }
                else
                    throw new Exception("Email entered for user is not valid.");
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
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
            try
            {
                MailAddress fromAddress = new MailAddress("charlespkolstad@gmail.com", "Charles");
                MailAddress toAddress = new MailAddress(user.Email, user.UName);
                string subject = "New message from Service Membership.";
                string body = SetMessageBody(user, action);

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
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }
        }

        private static string SetMessageBody(UserInfo user, UserManagerActions action)
        {
            StringBuilder message = new StringBuilder();
            switch (action)
            {
                case UserManagerActions.create:
                    message.Append("A new account has been created for you!");
                    message.Append("<hr />Your username is " + user.UName);
                    message.Append("<br />Your initial password is p@ssword1<br /><br />");
                    message.Append("Please login using these credentials.  You will need to ");
                    message.Append("update your information and password when after you first ");
                    message.Append("login.<br />");

                    break;
                case UserManagerActions.delete:
                    message.Append("The user " + user.UName + " has been deleted and will no");
                    message.Append(" longer be able to access the system.");
                    break;
                case UserManagerActions.recoverPass:
                    message.Append("A new temporary password has been generated for you.<br />");
                    message.Append("Your temporary password is " + Membership.GetUser(user.UName).GetPassword());
                    message.Append(".<hr />  Please login using your temporary password.  You ");
                    message.Append("will need to change your password once you successfully login.");
                    break;
                case UserManagerActions.update:
                    message.Append("Thank you for updating your user account!");
                    break;
            }

            if (message.Length > 0)
            {
                message.Append("<hr />Thank you.  Please contact --- if you need additional help.");
                return "<h2>New message from Service Membership</h2><br /><br />" + message.ToString();
            }
            else
                throw new Exception("No message.");
        }

        private static UserInfo GetUserInfo(MembershipUser user, ISprocCalls sprocCalls)
        {
            try
            {
                UserInfo uMember = new UserInfo();
                UserInfo uInfo = sprocCalls.GetUserInfoByUser(user.UserName);

                uMember = (UserInfo)user;
                uMember.UserInfoID = uInfo.UserInfoID;
                uMember.PhoneNumber = uInfo.PhoneNumber;
                uMember.FirstName = uInfo.FirstName;
                uMember.LastName = uInfo.LastName;
                uMember.UserProfiles = uInfo.UserProfiles;

                return uMember;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return null;
            }
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

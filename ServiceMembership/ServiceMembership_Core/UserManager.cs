using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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

                string providerMessage = ValidateUserInfo(userInfo);

                if (string.IsNullOrEmpty(providerMessage))
                {
                    providerMessage = provider.MembershipActionCreate(userInfo.UName, userInfo.Email);
                    if (string.IsNullOrEmpty(providerMessage))
                    {
                        if (string.IsNullOrEmpty(sprocCalls.UserInfoInsert(userInfo, adminUser)))
                            providerMessage = "Error saving user's info";
                        else
                            SendNotificationMail(userInfo, UserManagerActions.create, isTest);
                    }
                }

                return providerMessage;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return "Error creating user.";
            }
        }

        private static string ValidateUserInfo(UserInfo uInfo)
        {
            string message = string.Empty;

            if (!string.IsNullOrEmpty(uInfo.PhoneNumber))
            {
                Regex regex = new Regex(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");
                Match match = regex.Match(uInfo.PhoneNumber);
                if (!match.Success)
                {
                    message = "Please enter valid phone number.";
                }
            }

            bool oneActive = false;
            foreach (MemberProfile profile in uInfo.UserProfiles)
            {
                if (!oneActive)
                    oneActive = profile.Active;
            }
            if (!oneActive)
            {
                message = "At least one permission box must be checked.";
            }

            if (string.IsNullOrEmpty(uInfo.FirstName))
            {
                message = "Please enter your first name.";
            }
            if (string.IsNullOrEmpty(uInfo.LastName))
            {
                message = "Please enter last name.";
            }

            return message;
        }

        public static string UpdateUser(UserInfo uInfo, string adminUser, bool isTest = false)
        {
            IProvider provider = ApplicationTools.InitProvider(isTest);
            ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);

            if (provider.MembershipActionChangeEmail(uInfo.UName, uInfo.Email))
            {
                if (!sprocCalls.UserInfoUpdate(uInfo, ConvertProfileToTable(uInfo.UserProfiles), adminUser))
                    return "Error updating user info.";
                else
                    SendNotificationMail(uInfo, UserManagerActions.update, isTest);
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

        public static string DeleteUser(string user, string adminUser, bool isTest)
        {
            try
            {
                IProvider provider = ApplicationTools.InitProvider(isTest);

                if (provider.MembershipActionDelete(user))
                {
                    UserInfo adminInfo = GetUserInfo(adminUser, isTest);
                    SendNotificationMail(adminInfo, UserManagerActions.delete, isTest);

                    return string.Empty;
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

        public static string RecoverPassword(RecoverModel model, bool isTest = false)
        {
            try
            {
                IProvider provider = ApplicationTools.InitProvider(isTest);
                ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);

                UserInfo user = GetUserInfo(model.UserName, isTest);
                if (user.Email == model.Email)
                {
                    string newPassword;
                    if (provider.MembershipActionRecoverPass(user.UserName, user.Email, out newPassword))
                        SendNotificationMail(user, UserManagerActions.recoverPass, isTest, newPassword);
                }
                else
                    throw new Exception("Email entered for user is not valid.");
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return ex.Message;
            }

            return string.Empty;
        }

        public static bool SendNotificationMail(UserInfo user, UserManagerActions action, bool isTest, string addMessage = null)
        {
            try
            {
                INotificationTools notificationTools = ApplicationTools.InitNotification(isTest);

                string body = SetMessageBody(user, action, addMessage);
                string name = (user.FirstName != null && user.LastName != null) ? 
                    user.FirstName + " " + user.LastName : user.UName;

                notificationTools.SendNotificationMail(user.Email, name, body);
                return true;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
            }
        }

        private static string SetMessageBody(UserInfo user, UserManagerActions action, string addMessage)
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
                    message.Append("Your temporary password is " + addMessage);
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

        public static UserInfo GetUserInfo(string user, bool isTest)
        {
            try
            {
                IProvider provider = ApplicationTools.InitProvider(isTest);
                ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);
                UserInfo uInfo = sprocCalls.GetUserInfoByUser(user);

                return provider.GetUserByName(uInfo);
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return null;
            }
        }

        public static List<UserInfo> GetAllUsers(bool isTest = false)
        {
            try
            {
                List<UserInfo> userList = new List<UserInfo>();
                UserInfo userInfo;
                ISprocCalls sprocCalls = ApplicationTools.InitSprocCall(isTest);
                IProvider provider = ApplicationTools.InitProvider(isTest);
                DataTable userTable = sprocCalls.UserProfileGetAll();

                foreach (DataRow row in userTable.Rows)
                {
                    userInfo = new UserInfo();
                    userInfo.FirstName = row["FirstName"].ToString();
                    userInfo.LastName = row["LastName"].ToString();
                    userInfo.UName = row["UserName"].ToString();
                    userInfo.PhoneNumber = row["PhoneNumber"].ToString();
                    userInfo = provider.GetUserByName(userInfo);

                    userList.Add(userInfo);
                }

                return userList;
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return null;
            }
        }
    }

    public enum UserManagerActions
    {
        create,
        update,
        recoverPass,
        delete
    }
}

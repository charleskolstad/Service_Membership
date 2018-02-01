using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ServiceMembership_Core
{
    public class Provider : IProvider
    {
        public Provider()
        {
            try
            {
                SqlMembershipProvider sqlMembershipProvider = new SqlMembershipProvider();
                NameValueCollection config = new NameValueCollection();
                config.Add("connectionStringName", "myConnection");
                config.Add("enablePasswordRetrieval", "false");
                config.Add("enablePasswordReset", "true");
                config.Add("requiresQuestionAndAnswer", "false");
                config.Add("applicationName", "MyApp");
                config.Add("requiresUniqueEmail", "true");
                config.Add("maxInvalidPasswordAttempts", "3");
                config.Add("passwordAttemptWindow", "5");
                config.Add("commandTimeout", "30");
                config.Add("name", "AspNetSqlMembershipProvider");
                config.Add("minRequiredPasswordLength", "9");
                config.Add("minRequiredNonalphanumericCharacters", "1");
                sqlMembershipProvider.Initialize(config["name"], config);

                MembershipProviderCollection membershipProviders = new MembershipProviderCollection();
                membershipProviders.Add(sqlMembershipProvider);
                membershipProviders.SetReadOnly();

                BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Static;
                Type membershipType = typeof(Membership);
                membershipType.GetField("s_Initialized", bindingFlags).SetValue(null, true);
                membershipType.GetField("s_InitializeException", bindingFlags).SetValue(null, null);
                membershipType.GetField("s_HashAlgorithmType", bindingFlags).SetValue(null, "SHA-512");
                membershipType.GetField("s_HashAlgorithmFromConfig", bindingFlags).SetValue(null, false);
                membershipType.GetField("s_UserIsOnlineTimeWindow", bindingFlags).SetValue(null, 15);
                membershipType.GetField("s_Provider", bindingFlags).SetValue(null, sqlMembershipProvider);
                membershipType.GetField("s_Providers", bindingFlags).SetValue(null, membershipProviders);

                var connectionString = Membership.Provider.GetType().GetField("_sqlConnectionString", BindingFlags.Instance | BindingFlags.NonPublic);
                if (connectionString != null)
                    connectionString.SetValue(Membership.Provider, DBCommands._Connection);

                NewPasswordNeeded();
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }
        }

        private static void NewPasswordNeeded()
        {
            MembershipUser user = Membership.GetUser();

            //if (user != null)
            //{
            //    if (HttpContext.Current.Request.Path != "/ManageAccount.aspx")
            //    {
            //        if (user.LastPasswordChangedDate == user.CreationDate || user.LastPasswordChangedDate < DateTime.Now.AddMonths(-1))
            //            HttpContext.Current.Response.Redirect("~/ManageAccount.aspx", false);
            //    }
            //}
        }

        public string MembershipActionCreate(string uName, string email)
        {
            string result = string.Empty;

            try
            {
                Membership.CreateUser(uName, "p@ssword1", email);
            }
            catch (MembershipCreateUserException ex)
            {
                return ex.Message;
            }

            return result;
        }

        public bool MembershipActionDelete(string userName)
        {
            try
            {
                return (Membership.DeleteUser(userName));
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
            }
        }

        public bool MembershipActionRecoverPass(string userName, string email, out string newPassword)
        {
            newPassword = string.Empty;

            try
            {
                MembershipUser user = Membership.GetUser(userName);
                if (user.Email == email)
                {
                    newPassword = Membership.GeneratePassword(9, 1);
                    user.ChangePassword(user.GetPassword(), newPassword);
                    return true;
                }
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
            }

            return false;
        }

        public bool MembershipActionChangeEmail(string uName, string email)
        {
            try
            {
                MembershipUser user = Membership.GetUser(uName);
                if (email != user.Email)
                {
                    user.Email = email;
                    Membership.UpdateUser(user);
                }
            }
            catch (Exception ex)
            {
                DBCommands.RecordError(ex);
                return false;
            }

            return true;
        }

        public UserInfo GetUserByName(UserInfo uInfo)
        {
            UserInfo uMember = new UserInfo();
            MembershipUser user = Membership.GetUser(uInfo.UName);
            uMember.Comment = user.Comment;
            uMember.Email = user.Email;
            uMember.IsApproved = user.IsApproved;
            uMember.LastActivityDate = user.LastActivityDate;
            uMember.LastLoginDate = user.LastLoginDate;

            uMember.UserInfoID = uInfo.UserInfoID;
            uMember.PhoneNumber = uInfo.PhoneNumber;
            uMember.FirstName = uInfo.FirstName;
            uMember.LastName = uInfo.LastName;
            uMember.UserProfiles = uInfo.UserProfiles;

            return uMember;
        }
    }

    public class FakeProvider : IProvider
    {
        public UserInfo GetUserByName(UserInfo uInfo)
        {
            return new UserInfo();
        }

        public bool MembershipActionChangeEmail(string uName, string email)
        {
            return true;
        }

        public string MembershipActionCreate(string uName, string email)
        {
            return string.Empty;
        }

        public bool MembershipActionDelete(string userName)
        {
            return true;
        }

        public bool MembershipActionRecoverPass(string userName, string email, out string newPassword)
        {
            newPassword = "NewPassword";
            return true;
        }
    }

    public interface IProvider
    {
        string MembershipActionCreate(string uName, string email);

        bool MembershipActionDelete(string userName);

        bool MembershipActionRecoverPass(string userName, string email, out string newPassword);

        bool MembershipActionChangeEmail(string uName, string email);

        UserInfo GetUserByName(UserInfo uInfo);
    }
}

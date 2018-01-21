using ServiceMembership_Data;
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
    public class Provider
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

        static void NewPasswordNeeded()
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
    }
}

using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Data
{
    public class SprocCalls : ISprocCalls
    {
        #region profile
        public DataTable ProfileGetAll()
        {
            return DBCommands.AdapterFill("p_Profile_GetActive");
        }
        #endregion

        #region userinfo
        public UserInfo GetUserInfoByUser(string userName)
        {
            DBCommands.PopulateParams("@UserName", userName);
            var userInfoTable = DBCommands.AdapterFill("p_UserInfo_GetByUserName").AsEnumerable();
            UserInfo userInfo = null;

            if (userInfoTable.Any())
            {
                userInfo = new UserInfo();
                userInfo.FirstName = userInfoTable.Select(u => u.Field<string>("FirstName")).FirstOrDefault();
                userInfo.LastName = userInfoTable.Select(u => u.Field<string>("LastName")).FirstOrDefault();
                userInfo.PhoneNumber = userInfoTable.Select(u => u.Field<string>("PhoneNumber")).FirstOrDefault();
                userInfo.UName = userName;
                userInfo.UserInfoID = userInfoTable.Select(u => u.Field<int>("UserInfoID")).FirstOrDefault();

                foreach (var row in userInfoTable)
                {
                    MemberProfile profile = new MemberProfile();
                    profile.Active = row.Field<bool>("Active");
                    profile.ProfileID = row.Field<int>("ProfileID");
                    profile.UserName = userName;

                    userInfo.UserProfiles.Add(profile);
                }
            }

            return userInfo;
        }

        public string UserInfoInsert(UserInfo userInfo, string adminUser)
        {
            DBCommands.PopulateParams("@UserName", userInfo.UName);
            DBCommands.PopulateParams("@PhoneNumber", userInfo.PhoneNumber);
            DBCommands.PopulateParams("@FirstName", userInfo.FirstName);
            DBCommands.PopulateParams("@LastName", userInfo.LastName);
            DBCommands.PopulateParams("@AdminUser", adminUser);

            return DBCommands.ExecuteScalar("p_UserInfo_Insert");
        }

        public bool UserInfoUpdate(UserInfo userInfo, DataTable profileTable, string adminUser)
        {
            DBCommands.PopulateParams("@AdminUser", adminUser);
            DBCommands.PopulateParams("@UserInfoID", userInfo.UserInfoID);
            DBCommands.PopulateParams("@UserName", userInfo.UName);
            DBCommands.PopulateParams("@PhoneNumber", userInfo.PhoneNumber);
            DBCommands.PopulateParams("@FirstName", userInfo.FirstName);
            DBCommands.PopulateParams("@LastName", userInfo.LastName);
            DBCommands.PopulateParams("@UserProfileTable", profileTable);

            return DBCommands.ExecuteNonQuery("p_UserInfo_Update");
        }
        #endregion

        #region userprofile
        public DataTable UserProfileGetAll()
        {
            return DBCommands.AdapterFill("p_UserInfo_GetActive");
        }
        #endregion
    }

    public class FakeSprocCalls : ISprocCalls
    {
        #region profile
        public DataTable ProfileGetAll()
        {
            return new DataTable();
        }
        #endregion

        #region userinfo
        public UserInfo GetUserInfoByUser(string userName)
        {
            return new UserInfo();
        }

        public string UserInfoInsert(UserInfo userInfo, string adminUser)
        {
            return "1";
        }

        public bool UserInfoUpdate(UserInfo userInfo, DataTable profileTable, string adminUser)
        {
            return true;
        }
        #endregion

        #region userprofile
        public DataTable UserProfileGetAll()
        {
            return new DataTable();
        }
        #endregion
    }

    public interface ISprocCalls
    {
        #region profile
        DataTable ProfileGetAll();
        #endregion

        #region userinfo
        UserInfo GetUserInfoByUser(string userName);
        bool UserInfoUpdate(UserInfo userInfo, DataTable profileTable, string adminUser);
        string UserInfoInsert(UserInfo userInfo, string adminUser);
        #endregion

        #region userprofile
        DataTable UserProfileGetAll();
        #endregion
    }
}

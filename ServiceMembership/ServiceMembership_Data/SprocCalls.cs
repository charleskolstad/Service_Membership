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
        public bool DeleteProfile(int profileID, string userName)
        {
            DBCommands.PopulateParams("@UserName", userName);
            DBCommands.PopulateParams("@ProfileID", profileID);

            return DBCommands.ExecuteNonQuery("p_Profile_Delete");            
        }

        public UserInfo GetUserInfoByUser(string userName)
        {
            DBCommands.PopulateParams("@UserName", userName);

            return (UserInfo)DBCommands.DataReader("p_UserInfo_GetByUserName", DBCommands.ObjectTypes.UserInfo);
        }

        public string InsertProfile(Profile profile, string userName)
        {
            DBCommands.PopulateParams("@UserName", userName);
            DBCommands.PopulateParams("@ProfileName", profile.ProfileName);
            DBCommands.PopulateParams("@ProfileDescription", profile.ProfileDescription);
            DBCommands.PopulateParams("@ProfileLevel", profile.ProfileLevel);

            return DBCommands.ExecuteScalar("p_Profile_Insert");
        }

        public DataTable ProfileGetAll()
        {
            return DBCommands.AdapterFill("p_Profile_GetActive");
        }

        public bool UpdateProfile(Profile profile, string userName)
        {
            DBCommands.PopulateParams("@UserName", userName);
            DBCommands.PopulateParams("@ProfileID", profile.ProfileID);
            DBCommands.PopulateParams("@ProfileName", profile.ProfileName);
            DBCommands.PopulateParams("@ProfileDescription", profile.ProfileDescription);
            DBCommands.PopulateParams("@ProfileLevel", profile.ProfileLevel);

            return DBCommands.ExecuteNonQuery("p_Profile_Update");
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

        public DataTable UserProfileGetByUser(string userName)
        {
            return DBCommands.AdapterFill("p_UserProfile_GetByUserName");
        }

        //public bool UserProfileUpdate(MemberProfile profile, string adminUser)
        //{
        //    DBCommands.PopulateParams("@AdminUser", adminUser);
        //    DBCommands.PopulateParams("@UserName", profile.UserName);
        //    DBCommands.PopulateParams("@Active", profile.Active);
        //    DBCommands.PopulateParams("@ProfileID", profile.ProfileID);

        //    return DBCommands.ExecuteNonQuery("p_UserProfile_Update");
        //}
    }

    public class FakeSprocCalls : ISprocCalls
    {
        public bool DeleteProfile(int profileID, string userName)
        {
            return true;
        }

        public UserInfo GetUserInfoByUser(string userName)
        {
            return new UserInfo();
        }

        public string InsertProfile(Profile profile, string userName)
        {
            return "1";
        }

        public DataTable ProfileGetAll()
        {
            return new DataTable();
        }

        public bool UpdateProfile(Profile profile, string userName)
        {
            return true;
        }

        public string UserInfoInsert(UserInfo userInfo, string adminUser)
        {
            return "1";
        }

        public bool UserInfoUpdate(UserInfo userInfo, DataTable profileTable, string adminUser)
        {
            return true;
        }

        public DataTable UserProfileGetByUser(string userName)
        {
            return new DataTable();
        }

        //public bool UserProfileUpdate(MemberProfile profile, string adminUser)
        //{
        //    return true;
        //}
    }

    public interface ISprocCalls
    {
        DataTable ProfileGetAll();
        string InsertProfile(Profile profile, string userName);
        bool UpdateProfile(Profile profile, string userName);
        bool DeleteProfile(int profileID, string userName);
        UserInfo GetUserInfoByUser(string userName);
        bool UserInfoUpdate(UserInfo userInfo, DataTable profileTable, string adminUser);
        DataTable UserProfileGetByUser(string userName);
        //bool UserProfileUpdate(MemberProfile profile, string adminUser);
        string UserInfoInsert(UserInfo userInfo, string adminUser);
    }
}

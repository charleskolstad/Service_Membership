using NUnit.Framework;
using ServiceMembership_Core;
using ServiceMembership_Data;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace ServiceMembership_Test
{
    [TestFixture]
    public class UserManagerTest
    {
        [Test]
        public void CreateUser_SuccessfullyCreateUser_ReturnNonEmptyString()
        {
            //arrange
            UserInfo userInfo = new UserInfo();
            userInfo.UName = "NewUser";
            string adminUser = "AdminUser";
            string result;

            //act
            result = UserManager.CreateUser(userInfo, adminUser, true);

            //assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [Test]
        public void UpdateUser_SuccessfullyUpdateUser_ReturnNonEmptyString()
        {
            //arrange
            UserInfo userInfo = new UserInfo();
            userInfo.UName = "NewUser";
            string adminUser = "AdminUser";
            string result;

            //act
            result = UserManager.UpdateUser(userInfo, adminUser, true);

            //assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [Test]
        public void GetUserInfo_SuccessfullyCreateObject_ReturnsNonNull()
        {
            //arrange
            string userName = "Test-User";
            UserInfo user;

            //act
            user = UserManager.GetUserInfo(userName, true);

            //assert
            Assert.IsNotNull(user);
        }

        [Test]
        public void SendNotificationMail_SendEmailSuccessfully_ReturnTrue()
        {
            //arrange
            UserInfo user = new UserInfo();
            bool result;

            //act
            result = UserManager.SendNotificationMail(user, UserManagerActions.create, true);

            //assert
            Assert.IsTrue(result);
        }

        [Test]
        public void DeleteUser_SuccessfullyDeleteUser_ReturnEmptyString()
        {
            //arrange
            string user = "user";
            string adminUser = "adminUser";
            string result;

            //act
            result = UserManager.DeleteUser(user, adminUser, true);

            //assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [Test]
        public void RecoverPassword_SucessfullyRecoverPassword_ReturnEmptyString()
        {
            //arrange
            RecoverModel model = new RecoverModel();
            string result;

            //act
            result = UserManager.RecoverPassword(model, true);

            //assert
            Assert.IsTrue(string.IsNullOrEmpty(result));
        }

        [Test]
        public void GetAllUsers_SucessfullyGetListOfUsers_ReturnEmptyList()
        {
            //arrange
            List<UserInfo> userList;

            //act
            userList = UserManager.GetAllUsers(true);

            //assert
            Assert.IsTrue(userList != null);
        }
    }
}

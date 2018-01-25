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
            result = UserManager.CreateUser(userInfo, adminUser, new FakeSprocCalls());

            //assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
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
            result = UserManager.UpdateUser(userInfo, adminUser, new FakeSprocCalls());

            //assert
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}

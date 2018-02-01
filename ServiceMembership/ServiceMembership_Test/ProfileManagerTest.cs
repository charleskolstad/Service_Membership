using NUnit.Framework;
using ServiceMembership_Core;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceMembership_Test
{
    [TestFixture]
    public class ProfileManagerTest
    {
        [Test]
        public void GetAllProfiles_GetAllProfiles_ReturnEmptyList()
        {
            //arrange
            List<Profile> profiles;

            //act
            profiles = ProfileManager.GetAllProfiles(true);

            //assert
            Assert.IsNotNull(profiles);
        }
    }
}

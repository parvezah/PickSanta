using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PickSanta.DB.Tests
{
    [TestClass]
    public class UserTests
    {
        private IDataAccess dataAccess;

        [TestInitialize]
        public void TestInit()
        {
            dataAccess = new TableAccess("UnitTestTable2", "<insert key>");
        }

        [TestCleanup]
        public void TestCleanup()
        {
            dataAccess.Dispose();
        }

        [TestMethod]
        public void CreateUserTest()
        {
            string email = "parvezah@microsoft.com";
            string name = "Parvez";

            dataAccess.CreateUser(name, email);
            var allUsers = dataAccess.GetAllUsers();
            Assert.IsNotNull(allUsers);
            Assert.AreEqual(1, allUsers.Count, "There should be only one user");
            Assert.AreEqual(allUsers[0].Email, email, "The email address should be same");
            Assert.AreEqual(allUsers[0].Name, name, "The email address should be same");
            var user = dataAccess.GetUser(email);
            Assert.IsNotNull(user);
            Assert.AreEqual(user.Email, email, "The email address should be same");
            Assert.AreEqual(user.Name, name, "The email address should be same");

            dataAccess.RemoveUser(email);
            user = dataAccess.GetUser(email);
            Assert.IsNull(user);
        }

        [TestMethod]
        public void CreateSantaMapTest()
        {
            string giftee = "gifter@sc.com";
            string santa = "santa@sc.com";

            dataAccess.CreateUser(giftee, giftee);
            dataAccess.CreateUser(santa, santa);

            dataAccess.CreateSantaMap(giftee, santa);

            var allMaps = dataAccess.GetAllSantaMaps();

            Assert.IsNotNull(allMaps);
            Assert.AreEqual(1, allMaps.Count);
            Assert.AreEqual(giftee, allMaps[0].Giftee);
            Assert.AreEqual(santa, allMaps[0].Santa);

            var santaMap = dataAccess.GetGifteeForSanta(santa);
            Assert.IsNotNull(santaMap);
            Assert.AreEqual(giftee, santaMap.Giftee);
            Assert.AreEqual(santa, santaMap.Santa);

            dataAccess.ResetSantaMap(santa);
            santaMap = dataAccess.GetGifteeForSanta(santa);
            Assert.IsNull(santaMap);
        }
    }
}

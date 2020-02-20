using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Transactions;
using Capstone.DAL;
using System.Collections.Generic;
using Capstone.Models;

namespace Capstone.Tests
{
    [TestClass]
    public class SiteDAOTests : TestInitCleanup
    {
        private string connectionString = "Server=.\\SqlExpress;Database=npcampground;Trusted_Connection=True;";

        [TestMethod]
        public void GetSitesTests()
        {
            SetupDatabase(); //Setup data
            SiteDAO dao = new SiteDAO(connectionString); //arrange
            IList<Site> sites = dao.GetSites(); //act
            Assert.AreEqual(4, sites.Count); //assert
            CleanupDatabase(); //reset database
        }
    }
}
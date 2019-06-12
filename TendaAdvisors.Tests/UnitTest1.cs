using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TendaAdvisors.Controllers;
namespace TendaAdvisors.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            AdvisorsController ac = new AdvisorsController();
            ac.AdvisorToCommision(1, 1);
        }
    }
}

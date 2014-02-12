using System;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FamilyCourseExtensions.Tests
{
    [TestClass]
    public class TestAffiliatesFunctionality
    {
        [TestMethod]
        public void TestAffiliateTracking()
        {
            WebRequest webRequest = WebRequest.Create("https://scripts.affiliatefuture.com/AFSaleNoCookie.asp?orderID=Test4321&orderValue=10.99&merchant=6194&programmeID=17155&bannerID=0&affiliateSiteID=278388&ref=&payoutCodes= &offlineCode=&r=&img=0");
            WebResponse webResp = webRequest.GetResponse();
        }
    }
}

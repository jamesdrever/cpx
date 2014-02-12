using System.Net;
using System.Text;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Interfaces.ECommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.OrderQueue
{
    public class FamilyAdditionalQueueProcessingHandler : IAdditionalQueueProcessingHandler
    {
        public Order PerformAdditionalProcessing(Order order, Domain.Contacts.Contact contact)
        {
            var affiliateUrl = new StringBuilder();
            affiliateUrl.AppendFormat("https://scripts.affiliatefuture.com/AFSaleNoCookie.asp?orderID={0}&orderValue={1}&merchant=6202&programmeID=17173&bannerID=0&affiliateSiteID={2}&ref=&payoutCodes=&offlineCode=&r=&img=0",
                order.OrderId, order.ProductSubTotal,contact.ReferrerId);
            WebRequest webRequest = WebRequest.Create(affiliateUrl.ToString());
            WebResponse webResp = webRequest.GetResponse();
            return order;
        }
    }
}
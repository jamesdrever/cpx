using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;

namespace CustomerPortalExtensions.Interfaces.ECommerce
{
    public interface IAdditionalQueueProcessingHandler
    {
        Order PerformAdditionalProcessing(Order order, Contact contact);
    }
}
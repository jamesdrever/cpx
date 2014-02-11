using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain
{
    public class ContactOperationStatus : OperationStatus
    {
        public bool ContactCreated { get; set; }
        public bool ContactQueued { get; set; }
        public Contact Contact { get; set; }
    }
}
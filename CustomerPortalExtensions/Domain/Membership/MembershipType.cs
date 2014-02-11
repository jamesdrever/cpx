using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerPortalExtensions.Domain.Membership
{
    public class MembershipType
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
    }
}
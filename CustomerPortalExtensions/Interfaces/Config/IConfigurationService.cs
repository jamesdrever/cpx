using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CustomerPortalExtensions.Interfaces.Config
{
    public interface IConfigurationService
    {
        ICustomerPortalConfiguration GetConfiguration();
    }
}
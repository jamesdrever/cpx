using System.Configuration;
using CustomerPortalExtensions.Interfaces.Config;

namespace CustomerPortalExtensions.Helper.Config
{
    public class Configuration : IConfigurationService
    {
        public ICustomerPortalConfiguration GetConfiguration()
        {
            return (ICustomerPortalConfiguration)ConfigurationManager.GetSection("CustomerPortalExtensions");
        }
    }
}
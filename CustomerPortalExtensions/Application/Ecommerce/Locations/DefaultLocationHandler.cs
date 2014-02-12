using System;
using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Ecommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Locations
{
    public class DefaultLocationHandler : ILocationHandler
    {
        public Location GetLocation(string locationCode)
        {
            return new Location {Title = locationCode, Code=locationCode };
        }

        public List<Location> GetLocations()
        {
            throw new NotImplementedException();
        }
    }
}
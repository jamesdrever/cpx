using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Examine;
using UmbracoExamine;
using CustomerPortalExtensions.Domain.Ecommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Locations
{
    public class LocationHandler : ILocationHandler
    {
        public Location GetLocation(string locationCode)
        {
            //TODO: none of the Examine config should be hard-coded
            var locSearcher = ExamineManager.Instance.SearchProviderCollection["LearningLocationsSearcher"];
            var locSearchCriteria = locSearcher.CreateSearchCriteria();
            var locQuery = locSearchCriteria.NodeTypeAlias("FSCCentreHomePage");
            locQuery = locQuery.And().Field("centreInitials", locationCode);
            var locSearchResults = locSearcher.Search(locQuery.Compile());
            Location location = new Location();
            if (locSearchResults.Any())
            {
                //TODO:make sure the fields exist!
                location.Title = locSearchResults.First().Fields["nodeName"];
                location.Code = locSearchResults.First().Fields["centreInitials"];
                location.Email = locSearchResults.First().Fields["centreEmail"];
            }
            else
            {
                location.Title = locationCode;
                locationCode = locationCode;
            }
            return location;
        }

        public List<Location> GetLocations()
        {
            //TODO: none of the Examine config should be hard-coded
            var locSearcher = ExamineManager.Instance.SearchProviderCollection["LearningLocationsSearcher"];
            var locSearchCriteria = locSearcher.CreateSearchCriteria();
            var locQuery = locSearchCriteria.NodeTypeAlias("FSCCentreHomePage");
            //locQuery = locQuery.And().Field("centreInitials", locationCode);
            var locSearchResults = locSearcher.Search(locQuery.Compile());
            List<Location> locations=new List<Location>();
            foreach (var location in locSearchResults)
            {
                string locationEmail = location.Fields.ContainsKey("centreEmail") ? location.Fields["centreEmail"] : "";
                string locationCode = location.Fields.ContainsKey("centreInitials")
                                          ? location.Fields["centreInitials"]
                                          : "";

                locations.Add(new Location { Title = location.Fields["nodeName"], Email = locationEmail, Code = locationCode });
            }
            return locations;
        }
}
}
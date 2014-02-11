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
    public class FamilyLocationHandler : ILocationHandler
    {
        public Location GetLocation(string locationCode)
        {


            const string searchProvider = "FamilyLocationsSearcher";
            const string nodeTypeAlias = "FAMLocationPage";

            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[searchProvider];
            var locationSearchCriteria = locationSearcher.CreateSearchCriteria();
            var locationQuery = locationSearchCriteria.Field("locationName", locationCode);
            var locationSearchResults = locationSearcher.Search(locationQuery.Compile());

            var location = new Location();

            if (locationSearchResults.Count() == 1)
            {
                location.Title = locationSearchResults.First().Fields["nodeName"];
                location.Email = locationSearchResults.First().Fields["emailAddress"];
                location.Code = locationSearchResults.First().Fields["nodeName"];
            }
            return location;
        }


        public List<Location> GetLocations()
        {
            const string searchProvider = "FamilyLocationsSearcher";
            const string nodeTypeAlias = "FAMLocationPage";
            var locationList = new List<Location>();

            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[searchProvider];
            var locationSearchCriteria = locationSearcher.CreateSearchCriteria();
            var locationQuery = locationSearchCriteria.NodeTypeAlias(nodeTypeAlias);
            var locationSearchResults = locationSearcher.Search(locationQuery.Compile());

            foreach (var location in locationSearchResults)
            {
                var locationtoAdd =
                    new Location
                        {
                            Title = location.Fields["nodeName"],
                            Email = location.Fields["emailAddress"],
                            Code = location.Fields["nodeName"]
                        };
                locationList.Add(locationtoAdd);
            }

            return locationList;
        }
    }
}
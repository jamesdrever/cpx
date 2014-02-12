using System.Collections.Generic;
using System.Linq;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Examine;
using CustomerPortalExtensions.Domain.Ecommerce;

namespace CustomerPortalExtensions.Application.Ecommerce.Locations
{
    public class FamilyLocationHandler : ILocationHandler
    {
        private const string SearchProvider = "FamilyLocationsSearcher";
        private const string NodeTypeAlias = "FAMLocationPage";
        public Location GetLocation(string locationCode)
        {




            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[SearchProvider];
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
            var locationList = new List<Location>();

            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[SearchProvider];
            var locationSearchCriteria = locationSearcher.CreateSearchCriteria();
            var locationQuery = locationSearchCriteria.NodeTypeAlias(NodeTypeAlias);
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
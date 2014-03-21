using System;
using System.Collections.Generic;
using System.Linq;
using Examine;
using FamilyCourseExtensions.Models;

namespace FamilyCourseExtensions.Infrastructure
{
    public class LocationRepository
    {
        public List<Location> GetLocations()
        {
            const string searchProvider = "FamilyLocationsSearcher";
            const string nodeTypeAlias = "FAMLocationPage";
            var locationList = new List<Location>();

            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[searchProvider];
            var locationSearchCriteria = locationSearcher.CreateSearchCriteria();
            var locationQuery = locationSearchCriteria.Field("bookingStatus","O");
            var locationSearchResults = locationSearcher.Search(locationQuery.Compile());

            foreach (var location in locationSearchResults)
            {
                var locationtoAdd =
                    new Location
                        {
                            Name = location.Fields["nodeName"],
                            Id=Convert.ToInt32(location.Fields["id"]),
                            StartDate1 = Convert.ToDateTime(location.Fields["startDate1"]),
                            FinishDate1 = Convert.ToDateTime(location.Fields["finishDate1"]),
                            StartDate2 = location.Fields.ContainsKey("startDate2") ? Convert.ToDateTime(location.Fields["startDate2"]) : (DateTime?)null,
                            FinishDate2 = location.Fields.ContainsKey("finishDate2") ? Convert.ToDateTime(location.Fields["finishDate2"]) : (DateTime?)null
                            
                        };
                locationList.Add(locationtoAdd);
            }

            return locationList;
        }

        public Location GetLocation(int id)
        {
            const string searchProvider = "FamilyLocationsSearcher";
            const string nodeTypeAlias = "FAMLocationPage";

            var locationSearcher = ExamineManager.Instance.SearchProviderCollection[searchProvider];
            var locationSearchCriteria = locationSearcher.CreateSearchCriteria();
            var locationQuery = locationSearchCriteria.Field("id", id.ToString());
            var locationSearchResults = locationSearcher.Search(locationQuery.Compile());

            var locationtoReturn = new Location();

            if (locationSearchResults.Count() == 1)
            {
                locationtoReturn.Name = locationSearchResults.First().Fields["nodeName"];
                locationtoReturn.Id = id;
                locationtoReturn.StartDate1 = Convert.ToDateTime(locationSearchResults.First().Fields["startDate1"]);
                locationtoReturn.StartDate2 = locationSearchResults.First().Fields.ContainsKey("startDate2") ? Convert.ToDateTime(locationSearchResults.First().Fields["startDate2"]) :  (DateTime?)null;
                locationtoReturn.FinishDate1 = Convert.ToDateTime(locationSearchResults.First().Fields["finishDate1"]);
                locationtoReturn.FinishDate2 = locationSearchResults.First().Fields.ContainsKey("finishDate2") ? Convert.ToDateTime(locationSearchResults.First().Fields["finishDate2"]) : (DateTime?)null;
            }
            return locationtoReturn;



        }
    }
}
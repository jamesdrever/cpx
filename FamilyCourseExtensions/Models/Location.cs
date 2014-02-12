using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FamilyCourseExtensions.Models
{
    public class Location
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public DateTime StartDate1 { get; set; }
        public DateTime FinishDate1 { get; set; }
        public DateTime? StartDate2 { get; set; }
        public DateTime? FinishDate2 { get; set; }
    }
}
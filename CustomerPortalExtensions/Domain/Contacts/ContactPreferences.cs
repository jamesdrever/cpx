using System;
using System.Collections.Generic;

namespace CustomerPortalExtensions.Domain
{
    public class ContactPreferences
    {
        public int ExternalContactNumber { get; set; }
        public int ExternalAddressNumber { get; set; }
        public List<Preference> Preferences { get; set; }
    }
    public class Preference
    {
        public string Category { get; set; }
        public string Value { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public bool Selected { get; set; }
    }
}

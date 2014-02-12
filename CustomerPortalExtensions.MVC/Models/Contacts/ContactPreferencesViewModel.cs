using CustomerPortalExtensions.Domain;
using System.Collections.Generic;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.Models.Contacts
{
    public class ContactPreferencesViewModel 
    {
        [HiddenInput(DisplayValue = false)]
        public int ExternalContactNumber { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int ExternalAddressNumber { get; set; }
        public List<Preference> Preferences { get; set; }
    }
}
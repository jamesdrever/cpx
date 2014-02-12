using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using Umbraco.Web.Models;

namespace CustomerPortal.Models
{
    public class QueuedContactViewModel : RenderModel
    {
        public QueuedContactViewModel(RenderModel model)
        : base(model.Content, model.CurrentCulture)
    { }
        public string Title { get ; set;  }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public int QueueID { get; set; }
        public List<Contact> PossibleDuplicates { get; set; }
    }
}
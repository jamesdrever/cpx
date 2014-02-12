using System.Collections.Generic;
using CustomerPortalExtensions.Domain.Contacts;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Models.Admin
{
    public class ContactQueueViewModel : RenderModel
    {
        public ContactQueueViewModel(RenderModel model)
            : base(model.Content, model.CurrentCulture)
        {
            QueuedContacts = new List<ContactQueueItem>();
        }

        public List<ContactQueueItem> QueuedContacts { get; set; }
    }
}
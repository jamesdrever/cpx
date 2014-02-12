using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Models.Email
{
    public class EmailSubscriptionsViewModel : RenderModel
    {

        public EmailSubscriptionsViewModel(RenderModel model)
            : base(model.Content, model.CurrentCulture) { Subscriptions = new List<string>(); }

        [Required(ErrorMessage = "Please enter your email address")]
        public string Email { get; set; }
        public string ForeName { get; set; }
        public string LastName { get; set; }
        public List<string> Subscriptions { get; set; }
    }
}
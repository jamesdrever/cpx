using Umbraco.Web.Models;

namespace CustomerPortalExtensions.MVC.Models.Admin
{
    public class OperationStatusViewModel : RenderModel
    {
        public OperationStatusViewModel(RenderModel model)
            : base(model.Content, model.CurrentCulture)
        {
        }
        public string Message;
        public string FullErrorDetails;
    }
}
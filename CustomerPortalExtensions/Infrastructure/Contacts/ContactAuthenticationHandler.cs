using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Profile;
using System.Web.Security;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Interfaces.Contacts;

namespace CustomerPortalExtensions.Infrastructure.Contacts
{
    public class ContactAuthenticationHandler : IContactAuthenticationHandler
    {
        public ContactAuthenticationOperationStatus Authenticate(string userName, string password)
        {
            var operationStatus = new ContactAuthenticationOperationStatus();
            try
            {
                if (Membership.ValidateUser(userName, password))
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(userName, true);
                    operationStatus.Status=true;
                }
                else
                {
                    operationStatus.Status = false;
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactAuthenticationOperationStatus>
                    .CreateFromException("An error has occurred authenticating the user", e);
            }
            return operationStatus;

        }

        public ContactAuthenticationOperationStatus AuthenticateWithEmailAddress(string email, string password)
        {
            string userName = Membership.GetUserNameByEmail(email);
            //if member with that email address exists..
            if (userName == null)
            {
                return new ContactAuthenticationOperationStatus{Status=false, Message="A user with this email address could not be found"};
            }
            return Authenticate(userName, password);
        }


        public ContactAuthenticationOperationStatus CreateContact(Contact contact)
        {

            var operationStatus= new ContactAuthenticationOperationStatus();
            try
            {
                var createStatus = new MembershipCreateStatus();
                var newUser = Membership.CreateUser(contact.UserName, contact.Password, contact.Email, "Question", "Answer", true, out createStatus);

                switch (createStatus)
                {
                    case MembershipCreateStatus.Success:
                        System.Web.Security.FormsAuthentication.SetAuthCookie(contact.UserName, true);
                        operationStatus.Status = true;
                        Roles.AddUserToRole(contact.UserName, "Customer");
                        operationStatus.Message = "Your account has been successfully created.";
                        break;
                    case MembershipCreateStatus.DuplicateUserName:
                        operationStatus.Status = false;
                        operationStatus.Message =
                            "That email address is already registered with us. If you know what your password is, please just login using your email address and password.  If you have already registered with us and can't remember your password, please use the Password recovery link to reset your password.";
                        break;
                    case MembershipCreateStatus.DuplicateEmail:
                        operationStatus.Status = false;
                        operationStatus.Message =
                            "A user with that Email address already exists.  If you have already registered with us and can't remember your password, please use the Password recovery link to reset your password.";
                        break;
                    case MembershipCreateStatus.InvalidEmail:
                        operationStatus.Status=false;
                        operationStatus.Message = "Please enter a VALID email address.";
                        break;
                    case MembershipCreateStatus.InvalidAnswer:
                        operationStatus.Status = false;
                        operationStatus.Message = "The security answer is not valid";
                        break;
                    case MembershipCreateStatus.InvalidPassword:
                        operationStatus.Status = false;
                        operationStatus.Message = "The password entered is invalid. Please enter a passoword with at least 7 characters and one non-alphanumeric.";
                        break;
                    default:
                        operationStatus.Status = false;
                        operationStatus.Message = "Unknown Error: Account NOT created." ;
                        break;
                }
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<ContactAuthenticationOperationStatus>
                    .CreateFromException("An error has occurred creating the user", e);
            }
            return operationStatus;
        }



        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }

        public bool IsCurrentUserLoggedIn()
        {
            return (HttpContext.Current.Profile.UserName != null);
        }

        public string GetUserId()
        {
            // Obtain current HttpContext of ASP+ Request
            System.Web.HttpContext context = System.Web.HttpContext.Current;

            // If user is not authenticated, either fetch (or issue) a new temporary cartID
            if (context.Request.Cookies["UserId"] != null)
            {
                return context.Request.Cookies["UserId"].Value;
            }
            else
            {
                // Generate a new random GUID using System.Guid Class
                Guid tempCartId = Guid.NewGuid();

                // Send tempCartId back to client as a cookie
                context.Response.Cookies["UserId"].Value = tempCartId.ToString();

                // Return tempCartId
                return tempCartId.ToString();
            }
        }


        public string GetReferrerId()
        {
            System.Web.HttpContext context = System.Web.HttpContext.Current;
            return (context.Request.Cookies["cpxReferrer"] != null) ? context.Request.Cookies["cpxReferrer"].Value : "";
        }


        public string GetUserName()
        {
            return HttpContext.Current.Profile.UserName;
        }


        public int GetContactId()
        {
            string userName = GetUserName();
            ProfileBase profile = ProfileBase.Create(userName);
            int contactId = 0;
            int.TryParse(profile.GetPropertyValue("contactId").ToString(), out contactId);
            return contactId;
        }

        


        public void SetContactId(string userName, int contactId)
        {
            ProfileBase profile = ProfileBase.Create(userName);
            profile.SetPropertyValue("contactId", contactId.ToString());
            profile.Save();
            //TODO: do we need all this?
            try
            {

                //having to update membership in this convoluted way because of Umbraco bug
                //see http://umbraco.codeplex.com/workitem/30789

                var updateMember = Membership.GetUser(userName);
                var m = new umbraco.cms.businesslogic.member.Member((int)updateMember.ProviderUserKey);
                //m.LoginName = contact.Email;
                //m.Email = contact.Email;
                m.Save();
                FormsAuthentication.SetAuthCookie(userName, true);

            }
            catch (Exception e)
            {
                string err = e.ToString();
            }

            //var updateMember = Membership.GetUser(contact.UserName);
        }

        public string GetPropertyValue(string propertyName)
        {
            string userName = GetUserName();
            ProfileBase profile = ProfileBase.Create(userName);
            return profile.GetPropertyValue(propertyName).ToString();
        }


    }
}
using System.Web;
using System.Web.Security;
using System.Web.Profile;
using System;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Interfaces.Contacts;
using umbraco.cms.businesslogic.member;


namespace CustomerPortalExtensions.Infrastructure.Contacts
{
    public class ContactSecondaryRepository : IContactSecondaryRepository
    {

        public bool RepositoryAvailable()
        {
            return true;
        }


        /// <summary>
        /// get existing contact by username
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public ContactOperationStatus Get(string userName)
        {
            var operationStatus=new ContactOperationStatus();
            try
            {
                Contact contact = LoadFromProfile(ProfileBase.Create(userName, true));
                contact.UserId = GetUserId();
                operationStatus.Status = true;
                operationStatus.Contact = contact;
            }
            catch (Exception e)
            {
                operationStatus.Status = false;
                operationStatus.Message = "An error occured while retrieving your contact information.";
            }
            return operationStatus;
        }


        public ContactOperationStatus SaveContact(Contact contact)
        {
            //if user already exists
            if (!String.IsNullOrEmpty(contact.ExistingUserName)&&Membership.GetUser(contact.ExistingUserName)!=null)
            {
                //update the profile
                return UpdateContact(contact);
            }
            else
            {
                //otherwise create a new profile
                return CreateContact(contact);
            }
        }

        private ContactOperationStatus CreateContact(Contact contact)
        {
            MembershipCreateStatus createStatus = new MembershipCreateStatus();
            MembershipUser newUser = Membership.CreateUser(contact.UserName, contact.Password,contact.Email, "Question", "Answer", true, out createStatus);

            ContactOperationStatus returnStatus;

            switch (createStatus)
            {
                case MembershipCreateStatus.Success:
                    System.Web.Security.FormsAuthentication.SetAuthCookie(contact.UserName, true);
                    CreateProfile(contact);
                    returnStatus = new ContactOperationStatus { Status = true, Message = "Your account has been successfully created." };
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "That username already exists.  If you have already registered with us and can't remember your password, please use the Password recovery link to reset your password." };
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "A user with that Email address already exists.  If you have already registered with us and can't remember your password, please use the Password recovery link to reset your password." };
                    break;
                case MembershipCreateStatus.InvalidEmail:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "Please enter a VALID email address." };
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "The security answer is not valid" };
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "The password entered is invalid. Please enter a passoword with at least 7 characters and one non-alphanumeric." };
                    break;
                default:
                    returnStatus = new ContactOperationStatus { Status = false, Message = "Unknown Error: Account NOT created." };
                    break;
            }

            return returnStatus;
        }

        private ContactOperationStatus UpdateContact(Contact contact)
        {
            try
            {
                var checkMember = Membership.GetUserNameByEmail(contact.Email);
                //var profile = ProfileBase.Create(contact.Email);
                if (!String.IsNullOrEmpty(checkMember) && contact.ExistingEmail != contact.Email)
                {
                    return new ContactOperationStatus
                        {
                            Status = false,
                            MessageCode = "CP.EmailAlreadyExists",
                            Message = "This email address is already registered on our system."
                        };

                }
                SaveToProfile(contact,ProfileBase.Create(contact.ExistingUserName));
                return new ContactOperationStatus 
                {
                    Status = true, Message = "Your account has been successfully updated.", Contact = contact
                };
            }
            catch (Exception e)
            {
                return OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("An error has occurred updating your information", e);
            }
        }
        
        
        private Contact LoadFromProfile(ProfileBase profile)
        {
            Contact contact = new Contact();
            if (profile.UserName == null)
            {
                contact.Status = false;
            }
            else
            {             
                contact.UserName = profile.UserName;
                contact.Title = (string)profile.GetPropertyValue("customer_title");   
                contact.Address1 = (string)profile.GetPropertyValue("customer_address_1");
                contact.Address2 = (string)profile.GetPropertyValue("customer_address_2");
                contact.Address3 = (string)profile.GetPropertyValue("customer_address_3");
                contact.Town = (string)profile.GetPropertyValue("customer_town");
                contact.County = (string)profile.GetPropertyValue("customer_county");
                contact.Country = (string)profile.GetPropertyValue("customer_country");
                if (contact.Country == "United Kingdom") contact.Country = "UK";
                if (contact.Country.Length > 5) contact.Country = "";
                contact.Postcode = (string)profile.GetPropertyValue("customer_postcode");
                contact.FirstName = (string)profile.GetPropertyValue("customer_first_name");
                contact.LastName = (string)profile.GetPropertyValue("customer_last_name");
                contact.Email = Membership.GetUser(profile.UserName).Email;
                contact.Mobile = (string)profile.GetPropertyValue("customer_mobile");
                contact.Telephone = (string)profile.GetPropertyValue("customer_telephone");
                contact.SeparateDeliveryAddress = (profile.GetPropertyValue("customer_separate_delivery_address").ToString() == "1");
                contact.DeliveryAddress1 = (string)profile.GetPropertyValue("customer_delivery_address1");
                contact.DeliveryAddress2 = (string)profile.GetPropertyValue("customer_delivery_address2");
                contact.DeliveryAddress3 = (string)profile.GetPropertyValue("customer_delivery_address3");
                contact.DeliveryTown = (string)profile.GetPropertyValue("customer_delivery_town");
                contact.DeliveryCounty = (string)profile.GetPropertyValue("customer_delivery_county");
                contact.DeliveryPostcode = (string)profile.GetPropertyValue("customer_delivery_postcode");
                contact.DeliveryCountry = (string)profile.GetPropertyValue("customer_delivery_country");
                if (contact.DeliveryCountry == "United Kingdom") contact.DeliveryCountry = "UK";
                if (contact.DeliveryCountry.Length > 5) contact.DeliveryCountry = "";
                int externalContactNumber = 0;
                int.TryParse(profile.GetPropertyValue("care_contact_number").ToString(), out externalContactNumber);
                int externalAddressNumber = 0;
                int.TryParse(profile.GetPropertyValue("care_address_number").ToString(), out externalAddressNumber);
                contact.ExternalContactNumber = externalContactNumber;
                contact.ExternalAddressNumber = externalAddressNumber;
                contact.Status = true;

                //get the contact id
                //var getMember = Membership.GetUser(contact.UserName);
                //var m = new umbraco.cms.businesslogic.member.Member((int)getMember.ProviderUserKey);
                //contact.ContactId = m.Id;
            }
            return contact;
        }

        private void CreateProfile(Contact contact)
        {
            SaveToProfile(contact, ProfileBase.Create(contact.UserName));
            Roles.AddUserToRole(contact.UserName, "Customer");
        }

        private ProfileBase SaveToProfile(Contact contact,ProfileBase profile)
        {
            profile.SetPropertyValue("customer_title",contact.Title);
            profile.SetPropertyValue("customer_first_name", contact.FirstName);
            profile.SetPropertyValue("customer_last_name",contact.LastName);
            profile.SetPropertyValue("customer_address_1",contact.Address1);
            profile.SetPropertyValue("customer_address_2", contact.Address2);
            profile.SetPropertyValue("customer_address_3", contact.Address3);
            profile.SetPropertyValue("customer_town",contact.Town);
            profile.SetPropertyValue("customer_county",contact.County);
            profile.SetPropertyValue("customer_country", contact.Country);
            profile.SetPropertyValue("customer_postcode",contact.Postcode);

            profile.SetPropertyValue("customer_mobile",contact.Mobile);
            profile.SetPropertyValue("customer_telephone",contact.Telephone);
            profile.SetPropertyValue("customer_separate_delivery_address", contact.SeparateDeliveryAddress);            
            profile.SetPropertyValue("customer_delivery_address1",contact.DeliveryAddress1);
            profile.SetPropertyValue("customer_delivery_address2", contact.DeliveryAddress2);
            profile.SetPropertyValue("customer_delivery_address3", contact.DeliveryAddress3);
            profile.SetPropertyValue("customer_delivery_town", contact.DeliveryTown);
            profile.SetPropertyValue("customer_delivery_county", contact.DeliveryCounty);
            profile.SetPropertyValue("customer_delivery_postcode", contact.DeliveryPostcode);
            profile.SetPropertyValue("customer_delivery_country",contact.DeliveryCountry);
            profile.SetPropertyValue("care_contact_number", contact.ExternalContactNumber.ToString());
            profile.SetPropertyValue("care_address_number", contact.ExternalAddressNumber.ToString());
            profile.Save();

            //TODO: what to do with updating email addresses?
            try
            {

                //having to update membership in this convoluted way because of Umbraco bug
                //see http://umbraco.codeplex.com/workitem/30789

                var updateMember = Membership.GetUser(contact.UserName);
                var m = new umbraco.cms.businesslogic.member.Member((int)updateMember.ProviderUserKey);
                m.LoginName = contact.Email;
                m.Email = contact.Email;
                m.Save();
                FormsAuthentication.SetAuthCookie(contact.Email, true);

             }
            catch (Exception e)
            {
                string err=e.ToString();
            }
            return profile;
        }

        /// <summary>
        /// get the current contact
        /// </summary>
        /// <returns></returns>
        public ContactOperationStatus GetContact()
        {
            try
            {
                Contact contact = LoadFromProfile(HttpContext.Current.Profile);
                contact.UserId = GetUserId();
                return new ContactOperationStatus { Status = true, Contact = contact };
            }
            catch (Exception e)
            {

                return new ContactOperationStatus { Status = false, Message = "An error occured while retrieving your contact information." };
            }
        }

  
        //TODO:no longer needed?
        public Contact GetContact(int UserID)
        {
            Member m = new Member(UserID);
            return LoadFromProfile(ProfileBase.Create(m.LoginName, true));
        }


        //TODO: get rid of this method, no longer required?
        public ContactCredentials GetContactCredentials()
        {
            ContactCredentials contactCredentials=new ContactCredentials();
            contactCredentials.UserId = GetUserId();
            contactCredentials.UserName = GetUserName();
            return contactCredentials;
        }

        private string GetUserName()
        {
            return (HttpContext.Current.Profile).UserName;
        }

        private string GetUserId()
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



       

        public bool IsUniqueUserName(string username)
        {
            return Member.GetMemberFromLoginName(username) == null;
        }

        public bool IsUniqueEmail(string email)
        {
            return Member.GetMemberFromEmail(email) == null;
        }



        public string GetDeliveryCountry()
        {
            ///TODO: should be a config setting to determine default country if user not logged in
            ///TODO: plus nasty hack bypasses OperationStatus/exception handling..
            string Country="United Kingdom";
            if (!HttpContext.Current.Profile.IsAnonymous)
            {
                Country=GetContact().Contact.GetDeliveryCountry();
            }
            return Country;
        }


        public bool Authenticate(string userName, string password)
        {
            if (Membership.ValidateUser(userName, password))
            {
                System.Web.Security.FormsAuthentication.SetAuthCookie(userName, true);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool AuthenticateWithEmailAddress(string email, string password)
        {
            string userName = Membership.GetUserNameByEmail(email);
            //if member with that email address exists..
            if (userName != null)
            {
                if (Membership.ValidateUser(userName, password))
                {
                    System.Web.Security.FormsAuthentication.SetAuthCookie(userName, true);
                    return true;
                }
            }
            return false;
        }
        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }

        public bool CurrentUserLogggedIn()
        {
            return (!(HttpContext.Current.Profile.UserName == null));
        }

    }
}


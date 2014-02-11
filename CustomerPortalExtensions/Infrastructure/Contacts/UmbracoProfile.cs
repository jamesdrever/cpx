using System.Web.Profile;
using System;
using System.Web.Security;

namespace CustomerPortalExtensions.Infrastructure.Contacts
{
    public class UmbracoProfile : ProfileBase
    {
        public string getValue(string alias)
        {
            var value = base.GetPropertyValue(alias);
            if (value == DBNull.Value)
            {
                    return string.Empty;
            }
            return value.ToString();
        }

        public string getUserName()
        {
            return UserName;
        }

        public string getEmail()
        {
            string email="";
            if (Membership.GetUser()!=null)
            {
                MembershipUser mu=Membership.GetUser();
                if (mu.Email!=null) { email=mu.Email; }
            }
            return email;
        }

        public void setEmail(string email)
        {
            MembershipUser mu = Membership.GetUser();
            mu.Email = email;
            Membership.UpdateUser(mu);
        }


    }
}
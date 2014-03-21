using System.Collections.Generic;
using CustomerPortalExtensions.Infrastructure.Services.Database;

namespace CustomerPortalExtensions.Domain.Contacts
{
    [TableName("cpxContact")]
    [PrimaryKey("ContactId")]
    public class Contact
    {
        public int ContactId { get; set;  }
        public string UserId { get; set; }
        [Ignore]
        public string ExistingUserName { get; set; }
        [Ignore]
        public string ExistingEmail { get; set; }
        public string UserName { get; set; }
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
        [Ignore]
        public string CountryDesc { get; set; }
        public bool SeparateDeliveryAddress { get ; set;  }
        public string DeliveryAddress1 { get ; set;  }
        public string DeliveryAddress2 { get ; set;  }
        public string DeliveryAddress3 { get ; set;  }
        public string DeliveryTown { get ; set;  }
        public string DeliveryCounty { get ; set;  }
        public string DeliveryPostcode { get ; set;  }
        public string DeliveryCountry { get; set; }
        [Ignore]
        public string DeliveryCountryDesc { get; set; }
        public string Email { get ; set;  }
        public string Telephone { get ; set;  }
        public string Mobile { get ; set;  }
        [Ignore]
        public string Password { get; set; }
        [Ignore]
        public string[] Preferences { get; set; }

        public string GetDeliveryCountry()
        {
            return SeparateDeliveryAddress ? DeliveryCountry: Country ;
        }
        public int ExternalContactNumber { get; set; }
        [Ignore]
        public int ExternalAddressNumber { get; set; }
        [Ignore]
        public bool ExternalContactCreate { get; set; }
        [Ignore]
        public bool Queued { get; set; }
        [Ignore]
        public bool Status { get; set; }
        public bool GiftAidAgreement { get; set; }
        public string ReferrerId { get; set; } 
    }
    public class ContactCredentials
    {
        public string UserName { get; set; }
        public string UserId { get; set; }
    }

    public class ContactQueueItem
    {
        public ContactQueueItem()
        {
            PossibleDuplicates=new List<Contact>();
        }
        public int ContactId { get; set; }
        public string Title { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Town { get; set; }
        public string County { get; set; }
        public string Postcode { get; set; }
        public string Country { get; set; }
        public List<Contact> PossibleDuplicates { get; set; }
    }
}
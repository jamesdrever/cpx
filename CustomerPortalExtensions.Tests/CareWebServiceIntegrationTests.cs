using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Infrastructure.Synchronisation;

namespace CustomerPortal.Tests
{
    [TestClass]
    public class CareWebServiceIntegrationTests
    {
        [TestMethod]
        public void CreateContact()
        {
            /**
            Contact contact = new Contact();

            contact.Title = "Mr";
            contact.FirstName = "James";
            contact.LastName = "Drever";
            contact.Address1 = "83 Oakfield Road";
            contact.Town = "Shrewsbury";
            contact.County = "Shropshire";
            contact.Country = "UK";

            var contactQueue = new Mock<IContactQueue>();
            contactQueue.Setup(x => x.AddToQueue(It.IsAny<Contact>())).Returns(new ContactOperationStatus());
            CareContactSynchroniser test = new CareContactSynchroniser(contactQueue.Object);
            test.SaveContact(contact);
            **/
        }

        [TestMethod]
        public void AddContactToQueue()
        {
            //TODO: PetaPoco dependency 
            /**
            ContactQueue test = new ContactQueue();

            int initialNumberInQueue = test.GetQueue().QueuedContacts.Count;

            Contact contact = new Contact();;

            contact.Title = "Mr";
            contact.FirstName = "James";
            contact.LastName = "Drever";
            contact.Address1 = "83 Oakfield Road";
            contact.Town = "Shrewsbury";
            contact.County = "Shropshire";
            contact.Country = "UK";

 
            var opStatus=test.AddToQueue(contact);
            Assert.IsTrue(opStatus.Contact.QueueID>0);

            int numberInQueue = test.GetQueue().QueuedContacts.Count;
            Assert.IsTrue(numberInQueue==initialNumberInQueue+1);
             * **/
        }






        [TestMethod]
        public void CreateContactWithEmailAddressAndTelephone()
        {
            Contact contact = new Contact();

            contact.Title = "Mr";
            contact.FirstName = "James";
            contact.LastName = "Drever";
            contact.Address1 = "83 Oakfield Road";
            contact.Town = "Shrewsbury";
            contact.County = "Shropshire";
            contact.Country = "UK";
            contact.Email = "james@jamesdrever.co.uk";
            contact.Telephone = "01743 852107";
            CareContactSynchroniser test = new CareContactSynchroniser();
            ContactOperationStatus operationStatus=test.SaveContact(contact);
            Assert.IsTrue(operationStatus.ContactQueued == true);

        }


        [TestMethod]
        public void GetContact()
        {
            CareContactSynchroniser test = new CareContactSynchroniser();
            Contact updatedContact = test.GetContact(3).Contact;
            Assert.IsTrue(updatedContact.FirstName == "Care");
            Assert.IsTrue(updatedContact.LastName == "Admin");
        }

        [TestMethod]
        public void UpdateContact()
        {
            Contact contact = new Contact();

            contact.Title = "Mr";
            contact.FirstName = "James";
            contact.LastName = "Drever";
            contact.Address1 = "83 Oakfield Road";
            contact.Town = "Shrewsbury";
            contact.County = "Shropshire";
            contact.Country = "UK";

            CareContactSynchroniser test = new CareContactSynchroniser();
            ContactOperationStatus createdContact = test.SaveContact(contact);
        }

        [TestMethod]
        public void CreateMembership()
        {
            //TODO: need to add in contact/addres numbers
            Membership contact = new Membership();
            contact.MembershipType = "IM";
            contact.MembershipPaymentFrequency = "A";
            contact.MembershipPaymentMethod = "CC";

            CareContactSynchroniser test = new CareContactSynchroniser();
            MembershipOperationStatus createdContact = test.CreateMembership(contact);
        }

        [TestMethod]
        public void CreateDDMembership()
        {
            //TODO: need to add in contact/addres numbers
            Membership contact = new Membership();


            contact.MembershipType = "IM";
            contact.MembershipPaymentFrequency = "A";
            contact.MembershipPaymentMethod = "DD";
            contact.BankAccountName = "Mr J G Drever";
            contact.BankAccountNumber = "61079212";
            contact.SortCode = "401723";

            CareContactSynchroniser test = new CareContactSynchroniser();
            MembershipOperationStatus contacts = test.CreateMembership(contact);
        }

        [TestMethod]
        public void GetDuplicates()
        {
            CareContactSynchroniser test = new CareContactSynchroniser();
            ContactDuplicatesOperationStatus operationStatus= test.GetDuplicates("Mr", "Drever", "sy3 8al");
        }

    }
}

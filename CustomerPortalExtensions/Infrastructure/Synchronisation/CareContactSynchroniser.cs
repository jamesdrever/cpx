using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using AutoMapper;
using CustomerPortal.Domain.Contacts;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.Membership;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces;
using CustomerPortalExtensions.Domain;
using CustomerPortalExtensions.Interfaces.Synchronisation;

namespace CustomerPortalExtensions.Infrastructure.Synchronisation
{
    public class CareContactSynchroniser : IContactSynchroniser
    {

        #region ContactDetails

        public ContactOperationStatus SaveContact(Contact contact)
        {
            /**
            if (contact.ExternalContactCreate)
            {
                return CreateContact(contact);
            }
            **/
            if (contact.ExternalContactNumber == 0)
            {
                CreateContact(contact);
            }
            return UpdateContact(contact);
        }

        private ContactOperationStatus CreateContact(Contact contact)
        {
            var opStatus = new ContactOperationStatus { Status = true };

            try
            {
                
                CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices =
                    new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();

                //TODO: do we need the registered users table?
                //TODO: CARE - set up fscweb (FSC Web) in users table
                //TODO: CARE - set up WCP (Website Customer Portal) in source table
                string sendXML = String.Format("<Parameters>" +
                                               "<Title>{0}</Title>" +
                                               "<Forenames>{1}</Forenames>" +
                                               "<Surname>{2}</Surname>" +
                                               "<Address>{3}</Address>" +
                                               "<Town>{4}</Town>" +
                                               "<County>{5}</County>" +
                                               "<Country>{6}</Country>" +
                                               "<Postcode>{7}</Postcode>" +
                                               "<DirectNumber>{8}</DirectNumber>" +
                                               "<MobileNumber>{9}</MobileNumber>" +
                                               "<EmailAddress>{10}</EmailAddress>" +
                                               "<UserLogname>fscweb</UserLogname>" +
                                               "<Source>WCP</Source>" +
                                               "</Parameters>",
                                               contact.Title, contact.FirstName, contact.LastName,
                                               contact.Address1 + "\n" + contact.Address2 + "\n" + contact.Address3,
                                               contact.Town, contact.County, contact.Country, contact.Postcode,contact.Telephone,
                                               contact.Mobile, contact.Email
                    );

                //Load xmldata into XDocument Object
                XDocument sendingXMLDoc = XDocument.Parse(sendXML);

                string returnXML = careWebServices.AddContact(sendingXMLDoc.ToString());




                XDocument receivingXMLDoc = XDocument.Parse(returnXML);

                XElement resultElement = receivingXMLDoc.Element("Result");
                if (resultElement.Element("ErrorMessage") != null)
                {
                    string errorMessage = resultElement.Element("ErrorMessage").Value;
                    opStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                        .CreateFromException("Error storing contact in CARE: " + errorMessage, new CareException());
                    opStatus.Status = false;
                }
                else
                {

                    int careContactNumber = Convert.ToInt32(receivingXMLDoc.Element("Result")
                                                                           .Element("ContactNumber").Value);
                    int careAddressNumber = Convert.ToInt32(receivingXMLDoc.Element("Result")
                                                                           .Element("AddressNumber").Value);
                    contact.ExternalContactNumber = careContactNumber;
                    contact.ExternalAddressNumber = careAddressNumber;
                    opStatus.Message = "The contact has been successfully created.";
                    opStatus.Contact = contact;
                }
            }
            catch (Exception e)
            {
                opStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("Error storing contact in CARE: " + e.Message, e);
                opStatus.Status = false;
            }
            return opStatus;
 


        }

        private ContactOperationStatus UpdateContact(Contact contact)
        {

            var opStatus = new ContactOperationStatus { Status = true };

            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();

            //first update contact information
            string sendXML = String.Format("<Parameters>" +
                "<ContactNumber>{0}</ContactNumber>" +
                "<Title>{1}</Title>" +
                "<Forenames>{2}</Forenames>" +
                "<Surname>{3}</Surname>" +
                "<Address>{4}</Address>" +
                "<Town>{5}</Town>" +
                "<County>{6}</County>" +
                "<Country>{7}</Country>" +
                "<DirectNumber>{8}</DirectNumber>" +
                "<MobileNumber>{9}</MobileNumber>" +
                "<EmailAddress>{10}</EmailAddress>" +
                 "<UserLogname>fscweb</UserLogname>" +
                "</Parameters>",
                contact.ExternalContactNumber, contact.Title, contact.FirstName,contact.LastName, contact.Address1 + "\n" + contact.Address2 + "\n" + contact.Address3,
                contact.Town, contact.County, contact.Country,contact.Telephone,contact.Mobile,contact.Email);

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            string returnXML = careWebServices.UpdateContact(sendingXMLDoc.ToString());


            XDocument receivingXMLDoc = XDocument.Parse(returnXML);

            try
            {
                //TODO: what to do about lack of address number returned?
                int careContactNumber = Convert.ToInt32(receivingXMLDoc.Element("Result")
                    .Element("ContactNumber").Value);
                contact.ExternalContactNumber = careContactNumber;
                opStatus.Contact = contact;
                opStatus.Message = "The contact has been successfully updated.";
            }
            catch (Exception e)
            {
                opStatus = OperationStatusExceptionHelper<ContactOperationStatus>
                    .CreateFromException("Error updating contact in CARE: " + e.Message, e);
                opStatus.Status = false;

            }
            //if update of contact information worked
            //* if (opStatus.Status)
            //{
                //update address information
            //    sendXML = String.Format("<Parameters>" +
            //        "<ContactNumber>{0}</ContactNumber>" +
            //        "<AddressNumber>{1}</AddressNumber>" +
            //        "<Address>{2}</Address>" +
            //        "<Town>{3}</Town>" +
            //        "<County>{4}</County>" +
             //       "<Country>{5}</Country>" +
              //      contact.ExternalContactNumber, contact.ExternalAddressNumber, contact.Address1 + "\n" + contact.Address2 + "\n" + contact.Address3,
             //       contact.Town, contact.County, contact.Country);

                //Load xmldata into XDocument Object
  //              sendingXMLDoc = XDocument.Parse(sendXML);
//
                //returnXML = careWebServices.UpdateAddress(sendingXMLDoc.ToString());
    //            receivingXMLDoc = XDocument.Parse(returnXML);
                //try
                //{
               //     int careAddressNumber = Convert.ToInt32(receivingXMLDoc.Element("Result")
               //         .Element("AddressNumber").Value);
                //}
                //catch (Exception e)
                //{
                //    opStatus = ContactOperationStatus.CreateFromException("Error updating address in CARE: " + e.Message, e);
                //    opStatus.Status = false;

                //}
            //}
            return opStatus;
        }

        public ContactOperationStatus GetContact(int externalContactNumber)
        {
            var opStatus = new ContactOperationStatus { Status = true };
            Contact contact = new Contact();


            string sendXML = String.Format("<Parameters>" +
                "<ContactNumber>{0}</ContactNumber>" +
                //"<AddressNumber>{1}</AddressNumber>" +
                "</Parameters>",
                externalContactNumber);//, externalAddressNumber

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
            string returnXML = careWebServices.SelectContactData(CustomerPortalExtensions.CareWebServices.XMLContactDataSelectionTypes.xcdtContactInformation, sendingXMLDoc.ToString());

            XDocument receivingXMLDoc = XDocument.Parse(returnXML);
            try
            {
                contact.ExternalContactNumber = externalContactNumber;
                
                XElement contactRow = (receivingXMLDoc.Element("ResultSet").Element("DataRow"));
                contact.ExternalAddressNumber = Convert.ToInt32(contactRow.Element("AddressNumber").Value);
                contact.Title = contactRow.Element("Title").Value;
                contact.FirstName = contactRow.Element("Forenames").Value;
                contact.LastName = contactRow.Element("Surname").Value;
                //TODO: need to split address back out into Address1,2,3 etc       
                contact.Address1 = contactRow.Element("Address").Value;
                contact.Town = contactRow.Element("Town").Value;
                contact.County = contactRow.Element("County").Value;
                contact.Postcode = contactRow.Element("Postcode").Value;
                contact.Country = contactRow.Element("CountryCode").Value;
                contact.Status = true;
                opStatus.Contact = contact;
            }
            catch (Exception e)
            {
                opStatus = OperationStatusExceptionHelper<ContactOperationStatus>.
                    CreateFromException("Error retrieving contact from CARE: " + e.Message, e);
                opStatus.Status = false;
            }
            return opStatus;
        }
        #endregion
        #region Membership

        public MembershipOperationStatus CreateMembership(Membership membership)
        {
            var opStatus = new MembershipOperationStatus { Status = true };

            //if there is not an existing CARE contact for this membership, create one..
            //if (membership.ExternalContactNumber == 0)
            //{
            //    opStatus = CreateContact(contact);
            //    if (!opStatus.Status)
            //    {
            //        return opStatus;
            //    }
            //    else
            //    {
            //        contact.ExternalContactNumber = opStatus.UpdatedContact.ExternalContactNumber;
            //        contact.ExternalAddressNumber = opStatus.UpdatedContact.ExternalAddressNumber;
            //    }
            //}

            //set up the web service
            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
            string sendXML = String.Format("<Parameters>" +
                "<PayerContactNumber>{0}</PayerContactNumber>" +
                "<PayerAddressNumber>{1}</PayerAddressNumber>" +
                "<StartDate>{2}</StartDate>" +
                "<PaymentFrequency>{3}</PaymentFrequency>" +
                "<Source>FSC</Source>" +
                "<ReasonForDespatch>{5}</ReasonForDespatch>" +
                "<PaymentMethod>{4}</PaymentMethod>" +
                "<MembershipType>{5}</MembershipType>" +
                "<Joined>{2}</Joined>"  +
                "<BankAccount>B1</BankAccount>" +
                "<SortCode>{6}</SortCode>"  +
                "<AccountNumber>{7}</AccountNumber>"  +
                "<AccountName>{8}</AccountName>" +
                "<Branch>B1</Branch>" +
                "<UserLogname>fscweb</UserLogname>" +
                "</Parameters>",
                membership.ExternalContactNumber, membership.ExternalAddressNumber, DateTime.Now.ToShortDateString(), membership.MembershipPaymentFrequency, membership.MembershipPaymentMethod, membership.MembershipType, membership.SortCode, membership.BankAccountNumber, membership.BankAccountName
                );

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            string returnXML = careWebServices.AddMembership(sendingXMLDoc.ToString());


            XDocument receivingXMLDoc = XDocument.Parse(returnXML);
            XElement resultElement = receivingXMLDoc.Element("Result");
            if (resultElement.Element("ErrorMessage") != null)
            {
                string errorMessage = resultElement.Element("ErrorMessage").Value;
                opStatus = MembershipOperationStatus.CreateFromException("Error creating membership in CARE: " + errorMessage, new CareException());
                opStatus.Status = false;
            }
            else
            {
                try
                {
                    int careMemberNumber = Convert.ToInt32(receivingXMLDoc.Element("Result")
                        .Element("MemberNumber").Value);
                    string careMessage = receivingXMLDoc.Element("Result")
                        .Element("InformationMessage").Value.ToString();
                    membership.ExternalMemberNumber = careMemberNumber;
                    opStatus.Message = "Your membership has been created successfully";
                    opStatus.UpdatedMembership = membership;

                }
                catch (Exception e)
                {
                    opStatus = MembershipOperationStatus.CreateFromException("Error creating membership in CARE: " + e.Message, e);
                    opStatus.Status = false;
                }
            }
            return opStatus;

        }
        #endregion
        #region ContactPreferences

        public ContactPreferencesOperationStatus GetContactPreferences(int externalContactNumber)
        {
            return GetContactPreferences(externalContactNumber,false);
        }
        public ContactPreferencesOperationStatus GetCurrentContactPreferences(int externalContactNumber)
        {
            return GetContactPreferences(externalContactNumber,true);
        }
        private ContactPreferencesOperationStatus GetContactPreferences(int externalContactNumber, bool currentRecordsOnly)
        {
            var opStatus = new ContactPreferencesOperationStatus { Status = true };
            ContactPreferences preferences = new ContactPreferences();
            preferences.ExternalContactNumber = externalContactNumber;
            //preferences.ExternalAddressNumber = externalAddressNumber;


            string sendXML = String.Format("<Parameters>" +
                "<ContactNumber>{0}</ContactNumber>" +
                //"<AddressNumber>{1}</AddressNumber>" +
                "</Parameters>",
                externalContactNumber);//, externalAddressNumber

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
            string returnXML = careWebServices.SelectContactData(CustomerPortalExtensions.CareWebServices.XMLContactDataSelectionTypes.xcdtContactCategories, sendingXMLDoc.ToString());

            XDocument receivingXMLDoc = XDocument.Parse(returnXML);
            List<Preference> contactPreferences = new List<Preference>();

            try
            {

                foreach (XElement preferenceNode in receivingXMLDoc.Descendants("DataRow"))
                {
                    string status=preferenceNode.Element("Status").Value;
                    if (!currentRecordsOnly||status=="Current")
                    {
                    contactPreferences.Add(new Preference
                    {
                        Category = preferenceNode.Element("ActivityCode").Value,
                        Value = preferenceNode.Element("ActivityValueCode").Value,
                        ValidFrom=Convert.ToDateTime(preferenceNode.Element("ValidFrom").Value),                        
                        ValidTo=Convert.ToDateTime(preferenceNode.Element("ValidTo").Value)
                    });
                    }
                }
                
            }
            catch (Exception e)
            {
                opStatus = ContactPreferencesOperationStatus.CreateFromException("Error retrieving contact preferences from CARE: " + e.Message, e);
                opStatus.Status = false;   
            }
            preferences.Preferences = contactPreferences;
            opStatus.UpdatedPreferences = preferences;
            return opStatus;
        }

        public ContactPreferencesOperationStatus SaveContactPreferences(ContactPreferences preferences)
        {
            var opStatus = GetContactPreferences(preferences.ExternalContactNumber);

            if (opStatus.Status)
            {
                ContactPreferences existingPreferences = opStatus.UpdatedPreferences;

                try
                {
                    //loop through each new preference added
                    foreach (var preference in preferences.Preferences)
                    {

                        if (preference.Selected)
                        {
                            SavePreference(preference, preferences.ExternalContactNumber,preferences.ExternalAddressNumber);
                        }
                        else
                        {
                            //loop through any matching existing preferences ..
                            foreach (var existingPreference in existingPreferences.Preferences.FindAll(s => s.Category.Equals(preference.Category) && s.Value.Equals(preference.Value)))
                            {
                                //... and make them historical
                                MakePreferenceHistorical(existingPreference,preferences.ExternalContactNumber, preferences.ExternalAddressNumber);
                            }
                        }                        
                    }
                    //get the update preferences
                    opStatus = GetContactPreferences(preferences.ExternalContactNumber);

                }
                catch (Exception e)
                {
                    opStatus = ContactPreferencesOperationStatus.CreateFromException("Error retrieving contact preferences from CARE: " + e.Message, e);
                    opStatus.Status = false;
                }
            }
            //preferences.Preferences = contactPreferences;
            //opStatus.UpdatedPreferences = preferences;
            return opStatus;
        }

        private void SavePreference(Preference preference, int externalContactNumber, int externalAddressNumber)
        {
            string sendXML = String.Format("<Parameters>" +
                "<ContactNumber>{0}</ContactNumber>" +
                "<AddressNumber>{1}</AddressNumber>" +
                "<Activity>{2}</Activity>" +
                "<ActivityValue>{3}</ActivityValue>" +
                "<Source>FSC</Source>" +
                "<ValidTo>{4}</ValidTo>" +
                "</Parameters>",
                externalContactNumber, externalAddressNumber,preference.Category,preference.Value, DateTime.Now.AddYears(100));

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
            //NOTE: no returned XML from this web service
            string returnXML =  careWebServices.AddActivity(sendingXMLDoc.ToString());
        }

        private void MakePreferenceHistorical(Preference preference, int externalContactNumber, int externalAddressNumber)
        {
            string sendXML = String.Format("<Parameters>" +
                //"<ContactNumber>{0}</ContactNumber>" +
                "<OldContactNumber>{0}</OldContactNumber>" +
                "<OldActivity>{1}</OldActivity>" +
                "<OldActivityValue>{2}</OldActivityValue>" +
                "<OldValidFrom>{3}</OldValidFrom>" +
                "<OldValidTo>{4}</OldValidTo>" +
                "<OldSource>FSC</OldSource>" +
                //"<Activity>{1}</Activity>" +
                //"<ActivityValue>{2}</ActivityValue>" +
                //"<ValidFrom>{3}</ValidFrom>" +
                "<ValidTo>{5}</ValidTo>" +
                //"<Source>FSC</Source>" +
                "</Parameters>",
                externalContactNumber, preference.Category, preference.Value,
                preference.ValidFrom,preference.ValidTo,DateTime.Now.AddDays(-1));

            //Load xmldata into XDocument Object
            XDocument sendingXMLDoc = XDocument.Parse(sendXML);

            CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
            //NOTE: no returned XML from this web service
            string returnXML = careWebServices.UpdateActivity(sendingXMLDoc.ToString());
        }

        #endregion  
        #region SynchronisationStatus


        public bool CanContactBeSynced(Contact contact)
        {
            //TODO:returning true if contact number is set - what about address number?
            //NOTE: not returned by CARE UpdateContact webservice
            return (contact.ExternalContactNumber>0);
        }


        #endregion
        #region Internal

        private bool IsNumeric(string stringToTest)
        {
            int result;
            return int.TryParse(stringToTest, out result);
        }

    

        #endregion



        public ContactDuplicatesOperationStatus GetDuplicates(string title, string surname, string postcode)
        {
            ContactDuplicatesOperationStatus opStatus=new ContactDuplicatesOperationStatus();
            try
            {
                string sendXML = String.Format("<Parameters>" +
                                           "<Title>{0}</Title>" +
                                           "<Surname>{1}</Surname>" +
                                           "<Postcode>{2}</Postcode>" +
                                           "</Parameters>",
                                           title, surname, postcode);

                //Load xmldata into XDocument Object
                XDocument sendingXMLDoc = XDocument.Parse(sendXML);

                CareWebServices.NDataAccessSoapClient careWebServices = new CustomerPortalExtensions.CareWebServices.NDataAccessSoapClient();
                //NOTE: no returned XML from this web service
                string returnXML = careWebServices.FindDuplicateContacts(sendingXMLDoc.ToString());

                XDocument receivingXMLDoc = XDocument.Parse(returnXML);

                List<Contact> possibleDuplicates = new List<Contact>();
                foreach (XElement duplicateNode in receivingXMLDoc.Descendants("DataRow"))
                {
                    Contact duplicateContact = new Contact();
                    duplicateContact.ExternalContactNumber = Convert.ToInt32(duplicateNode.Element("ContactNumber").Value);
                    duplicateContact.ExternalAddressNumber = Convert.ToInt32(duplicateNode.Element("AddressNumber").Value);

                    duplicateContact.Title = duplicateNode.Element("Title").Value;
                    duplicateContact.FirstName = duplicateNode.Element("Forenames").Value;
                    duplicateContact.LastName = duplicateNode.Element("Surname").Value;
                    //TODO: need to split address back out into Address1,2,3 etc       
                    duplicateContact.Address1 = duplicateNode.Element("Address").Value;
                    duplicateContact.Town = duplicateNode.Element("Town").Value;
                    duplicateContact.County = duplicateNode.Element("County").Value;
                    duplicateContact.Postcode = duplicateNode.Element("Postcode").Value;
                    duplicateContact.Country = duplicateNode.Element("Country").Value;
                    possibleDuplicates.Add(duplicateContact);

                }
                opStatus.PossibleDuplicates = possibleDuplicates;
                opStatus.Status = true;
            }
            catch (Exception e)
            {
                opStatus = OperationStatusExceptionHelper<ContactDuplicatesOperationStatus>
                    .CreateFromException("Error retrieving contact preferences from CARE: " + e.Message, e);
                opStatus.Status = false;
            }
           
            return opStatus;
        }

        public bool SynchronisationAvailable()
        {
            return true;
        }

        //TODO: should this be a web service call rather than a direct database call
        //faster as database call, but creates need for db connection
        public List<ContactTitle> GetContactTitles()
        {
            var db = new Database(ConfigurationManager.AppSettings["CP_SyncDSN"], "System.Data.SqlClient");
            //hacky way to map database fields using PetaPoco
            return db.Fetch<ContactTitle>("SELECT title AS code,title AS name FROM titles");
        }


        public List<Country> GetCountries()
        {
            var db = new Database(ConfigurationManager.AppSettings["CP_SyncDSN"], "System.Data.SqlClient");
            //hacky way to map database fields using PetaPoco
            return db.Fetch<Country>("SELECT country AS code,country_desc AS name FROM countries ORDER BY country_desc");
        }
    }
    public class CareException : Exception
    {

    }
}
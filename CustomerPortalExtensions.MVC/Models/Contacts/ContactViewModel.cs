using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Contacts;

namespace CustomerPortalExtensions.MVC.Models.Contacts
{
    public class ContactViewModel :  IValidatableObject
    {
        [HiddenInput(DisplayValue = false)]
        public string ExistingUserName { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string ExistingEmail { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string UserId { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int ContactId { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string UserName { get; set; }
        [HiddenInput(DisplayValue = true)]
        public int ExternalContactNumber { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int ExternalAddressNumber { get; set; }
        [HiddenInput(DisplayValue = false)]
        public bool Status { get; set; }
        public List<ContactTitle> Titles{ get; set; }
        [Required(ErrorMessage = "Please enter your title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Please tell us your first name")]
        public string FirstName { get; set; }
         [Required(ErrorMessage = "Please tell us your last name name")]
        public string LastName { get; set; }
         [Required(ErrorMessage = "Please enter your address")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
         [Required(ErrorMessage = "Please tell us your town")]
        public string Town { get; set; }
         [Required(ErrorMessage = "Please tell us your county")]
        public string County { get; set; }
         [Required(ErrorMessage = "Please tell us your postcode")]
        public string Postcode { get; set; }
         public List<Country> Countries{ get; set; }
         [Required(ErrorMessage = "Please tell us your country")]
        public string Country { get; set; }
        public bool SeparateDeliveryAddress { get; set; }
        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
        public string DeliveryAddress3 { get; set; }
        public string DeliveryTown { get; set; }
        public string DeliveryCounty { get; set; }
        public string DeliveryPostcode { get; set; }
        public string DeliveryCountry { get; set; }
         [Required(ErrorMessage = "Please tell us your email address")]
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Message { get; set; }
        [HiddenInput(DisplayValue = false)]
        public string Url { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            if (SeparateDeliveryAddress)
            {
                if (DeliveryAddress1 == null)
                    validationResults.Add(new ValidationResult("Please tell us your delivery address.", new[] { "DeliveryAddress1" }));
                if (DeliveryTown == null)
                    validationResults.Add(new ValidationResult("Please tell us the town for your delivery address.", new[] { "DeliveryTown" }));
                if (DeliveryCounty == null)
                    validationResults.Add(new ValidationResult("Please tell us the county for your delivery address", new[] { "DeliveryCounty" }));
                if (DeliveryPostcode == null)
                    validationResults.Add(new ValidationResult("Please tell us the postcode for your delivery address.", new[] { "DeliveryPostcode" }));
                if (DeliveryCountry == null)
                    validationResults.Add(new ValidationResult("Please tell us the country for your delivery address.", new[] { "DeliveryCountry" }));
            }
            if (String.IsNullOrEmpty(ExistingUserName))
            {
                if (Password == null)
                    validationResults.Add(new ValidationResult("Please tell us your password.", new[] { "Password" }));
                if (ConfirmPassword == null)
                    validationResults.Add(new ValidationResult("Please confirm your password.", new[] { "ConfirmPassword" }));
                if (Password!=null&&ConfirmPassword!=null&&Password!=ConfirmPassword)
                    validationResults.Add(new ValidationResult("The password and confirmation password do not match.", new[] { "ConfirmPassword" }));
            }
            return validationResults;
        }
    }
}
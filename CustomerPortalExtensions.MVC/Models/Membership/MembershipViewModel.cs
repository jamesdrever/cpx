using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CustomerPortalExtensions.Domain.Membership;

namespace CustomerPortalExtensions.MVC.Models.Membership
{
    public class MembershipViewModel : IValidatableObject
    {
        [HiddenInput(DisplayValue = false)]
        public int ExternalContactNumber { get; set; }
        [HiddenInput(DisplayValue = false)]
        public int ExternalAddressNumber { get; set; }
        [Required(ErrorMessage = "Please select a membership type")]
        public string MembershipType { get; set; }
        [Required(ErrorMessage = "Please select a payment method")]
        public string MembershipPaymentMethod { get; set; }
        public string MembershipPaymentFrequency { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string SortCode { get; set; }
        public List<MembershipType> AvailableMembershipTypes { get; set; }

        IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
        {
            var validationResults = new List<ValidationResult>();
            if (MembershipPaymentMethod == "DD")
            {
                if (MembershipPaymentFrequency==null)
                    validationResults.Add(new ValidationResult("You must tell us when you want to pay.", new[] { "MembershipPaymentFrequency" }));
                if (BankAccountName == null)
                    validationResults.Add(new ValidationResult("You must tell us the name of your bank account.", new[] { "BankAccountName" }));
                if (BankAccountNumber == null)
                    validationResults.Add(new ValidationResult("You must tell us the number of your bank account.", new[] { "BankAccountNumber" }));
                if (SortCode == null)
                {
                    validationResults.Add(new ValidationResult("You must tell us the sort code of your bank account.", new[] { "SortCode" }));
                }
                else
                {
                    SortCode = SortCode.Replace("-", "");
                }
            }
            
            return validationResults;
        }
    }
}
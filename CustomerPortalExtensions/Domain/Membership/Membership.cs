namespace CustomerPortalExtensions.Domain.Membership
{
    public class Membership
    {
        public int ExternalContactNumber { get; set; }
        public int ExternalMemberNumber { get; set; }
        public int ExternalAddressNumber { get; set; }
        public string MembershipType { get; set; }
        public string MembershipPaymentMethod { get; set; }
        public string MembershipPaymentFrequency { get; set; }
        public string BankAccountName { get; set; }
        public string BankAccountNumber { get; set; }
        public string SortCode { get; set; }
    }
}
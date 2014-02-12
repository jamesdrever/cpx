using System;
using CustomerPortalExtensions.Domain.Operations;

namespace CustomerPortalExtensions.Domain.Contacts
{
    public class ContactPreferencesOperationStatus : OperationStatus
    {
        public ContactPreferences UpdatedPreferences { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// TODO: doesn't confirm to DRY princple - must be a better way to do this using generics
        public static new ContactPreferencesOperationStatus CreateFromException(string message, Exception ex)
        {
            ContactPreferencesOperationStatus opStatus = new ContactPreferencesOperationStatus
            {
                Status = false,
                Message = message,
                OperationId = null
            };

            if (ex != null)
            {
                opStatus.ExceptionMessage = ex.Message;
                opStatus.ExceptionStackTrace = ex.StackTrace;
                opStatus.ExceptionInnerMessage = (ex.InnerException == null) ? null : ex.InnerException.Message;
                opStatus.ExceptionInnerStackTrace = (ex.InnerException == null) ? null : ex.InnerException.StackTrace;
            }
            return opStatus;
        }
    }
}
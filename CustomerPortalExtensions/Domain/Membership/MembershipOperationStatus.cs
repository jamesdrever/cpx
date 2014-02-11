using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CustomerPortalExtensions.Domain.Operations;


namespace CustomerPortalExtensions.Domain
{
    public class MembershipOperationStatus : OperationStatus
    {
        public Membership.Membership UpdatedMembership { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ex"></param>
        /// <returns></returns>
        /// TODO: doesn't confirm to DRY princple - must be a better way to do this
        public static new MembershipOperationStatus CreateFromException(string message, Exception ex)
        {
            MembershipOperationStatus opStatus = new MembershipOperationStatus
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
using System;
using System.Diagnostics;
using System.Reflection;
using log4net;

namespace CustomerPortalExtensions.Domain.Operations
{
    [DebuggerDisplay("Status: {Status}")]
    public class OperationStatus
    {

        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
                );


        public bool Status { get; set; }
        public int RecordsAffected { get; set; }
        public string Message { get; set; }
        public string MessageCode { get; set; }

        public Object OperationId { get; set; }
        public string ExceptionMessage { get; set; }
        public string ExceptionStackTrace { get; set; }
        public string ExceptionInnerMessage { get; set; }
        public string ExceptionInnerStackTrace { get; set; }

        public string FullErrorDetails
        {
            get
            {
                return Message + "\n" + ExceptionMessage + "\n" + ExceptionStackTrace + "\n" +
                       ExceptionInnerMessage + "\n" + ExceptionInnerStackTrace;
            }
        }

        public void LogFailedOperation(Exception exc)
        {
            LogFailedOperation(exc, "");
        }


        public void LogFailedOperation(Exception exc, string message)
        {
            Message = message;
            if (exc != null)
            {
                ExceptionMessage = exc.Message;
                ExceptionStackTrace = exc.StackTrace;
                ExceptionInnerMessage = (exc.InnerException == null) ? null : exc.InnerException.Message;
                ExceptionInnerStackTrace = (exc.InnerException == null) ? null : exc.InnerException.StackTrace;

                Log.Error(exc.Message+" "+exc.StackTrace, exc);
            }

        }


    }

    
    public class OperationStatusExceptionHelper<T> where T: OperationStatus, new()
    {
        private static readonly ILog Log =
            LogManager.GetLogger(
                MethodBase.GetCurrentMethod().DeclaringType
                );
        public static new T CreateFromException(string message, Exception ex)
        {
            T opStatus = new T()
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
                Log.Error(ex.Message + " " + ex.StackTrace, ex);
            }
            return opStatus;
        }
    }
}
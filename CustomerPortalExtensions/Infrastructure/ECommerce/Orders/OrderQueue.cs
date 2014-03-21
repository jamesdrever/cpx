using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using CustomerPortalExtensions.Domain.Contacts;
using CustomerPortalExtensions.Domain.ECommerce;
using CustomerPortalExtensions.Domain.Operations;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using Examine;

namespace CustomerPortalExtensions.Infrastructure.ECommerce.Orders
{
    public class OrderQueue : IOrderQueue
    {
        private readonly IOrderRepository _orderRepository;
        private readonly Database _database;
        private readonly IAdditionalQueueProcessingHandlerFactory _additionalQueueProcessingHandlerFactory;

        public OrderQueue(IOrderRepository orderRepository,Database database,IAdditionalQueueProcessingHandlerFactory additionalQueueProcessingHandlerFactory)
        {
            
            if (orderRepository == null) throw new ArgumentNullException("orderRepository");
            _orderRepository = orderRepository;
            
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }
            _database = database;
            if (additionalQueueProcessingHandlerFactory == null) throw new ArgumentNullException("additionalQueueProcessingHandlerFactory");
            _additionalQueueProcessingHandlerFactory = additionalQueueProcessingHandlerFactory;
        }

        public OrderQueueOperationStatus GetOrderQueue(string orderStatus, DateTime rangeStart,DateTime rangeFinish, string location)
        {
            var operationStatus = new OrderQueueOperationStatus();
            try
            {
                var sql = Sql.Builder
                    .Select("o.OrderId,OrderLineId,o.OrderCreated,ProductTitle,ProductType,ProductExternalId,ProductOptionTitle,ProductOptionExternalId,PaymentType,StartDate,FinishDate,Location,Quantity,o.GiftAidAgreement,c.Title,c.FirstName,c.LastName,c.Address1,c.Address2,c.Address3,c.Town,c.County,c.Postcode,c.Country,c.ExternalContactNumber,c.ContactId,c.Email")
                    .From("cpxOrderLines ol LEFT JOIN cpxOrder o ON ol.OrderId=o.OrderId INNER JOIN cpxContact c ON o.UserName=c.UserName");
               
                sql.Append("WHERE OrderStatus=@0", orderStatus);
                if (!String.IsNullOrEmpty(location))
                    sql.Append(" AND location=@0", location);
                if (orderStatus=="PROV")
                    sql.OrderBy("OrderCreated");
                else
                    sql.OrderBy("PaymentType,OrderCreated");
                operationStatus.OrderQueueItems = _database.Fetch<OrderQueueItem>(sql);
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderQueueOperationStatus>
                    .CreateFromException("An error has occurred retrieving the order from the queue", e);

            }
            return operationStatus;
        }

        public OrderOperationStatus QueueOrder(Order order, Contact contact)
        {
            var operationStatus = new OrderOperationStatus();
            try
            {
                SendEmails(order, contact);
                _additionalQueueProcessingHandlerFactory.GetHandler(order.AdditionalQueueProcessingHandler).PerformAdditionalProcessing(order,contact);
                order.Status = "QUE";
                order.UpdateOrderLines("QUE");
                _orderRepository.SaveOrder(order);
                operationStatus.Order = order;
                operationStatus.Status = true;
            }
            catch (Exception e)
            {
                operationStatus = OperationStatusExceptionHelper<OrderOperationStatus>
                    .CreateFromException("An error has occurred processing the order", e);
            }
            return operationStatus;
        }

        private void SendEmails(Order order, Contact contact)
        {
            //TODO: email content needs to be editable

            var mailBodyNotification = new StringBuilder();
            var mailBodyCustomer = new StringBuilder();
            var mailBodyCourses = new StringBuilder();
            var mailBodyMemberships = new StringBuilder();
            var mailBodyDonations = new StringBuilder();


            bool sendMail = false;

            mailBodyNotification.Append("<p>Contact Details</p>");
            mailBodyNotification.AppendFormat("<p>Name: {0} {1} {2}</p>", contact.Title, contact.FirstName,
                                              contact.LastName);
            mailBodyNotification.AppendFormat("<p>Address: {0}<br />{1}<br />{2}<br />{3}<br />{4}<br />{5}</p>",
                                              contact.Address1, contact.Address2, contact.Town, contact.County,
                                              contact.Postcode,
                                              contact.CountryDesc);
            mailBodyNotification.AppendFormat("<p>Telephone : {0}\nMobile: {1}</p>", contact.Telephone, contact.Mobile);
            mailBodyNotification.AppendFormat("<p>Email: {0}</p>", contact.Email);


            if (order.ContainsCourses())
            {
                sendMail = true;

                mailBodyNotification.Append("<p>This person has booked the following courses:</p>");

                mailBodyCustomer.Append(
                    "Thank you for requesting the course(s) below. The centre(s) running them will confirm availability and contact you shortly.");

                //create the main email first
                //loop through each item in the cart and add it to the email body
                foreach (OrderLine orderLine in order.GetCourseOrderLines())
                {

                    mailBodyCourses.AppendFormat("<p><b>Course: {0} {1}</b></p>", orderLine.ProductTitle,
                                                 orderLine.ProductDescription);
                    mailBodyCourses.AppendFormat("<p>Booking Option: {0} &#163;{1}</p>", orderLine.ProductOptionTitle,
                                                 orderLine.ProductPrice);
                    mailBodyCourses.AppendFormat("<p>Number of attendees: {0}", orderLine.Quantity);
                    mailBodyCourses.AppendFormat("<p>Payment Made Per attendee: &#163;{0}</p>", orderLine.PaymentAmount);
                    mailBodyCourses.AppendFormat("<p>Total Payment: &#163;{0}</p>", orderLine.PaymentLineTotal);
                }
                mailBodyCourses.AppendFormat("<p>Special Requirements: {0}</p>", order.SpecialRequirements);

                //create the centre emails next
                //loop through each centre and check whether they have any bookings

                var locationEmails = order.GetLocationEmails();

                foreach (string email in locationEmails)
                {
                    var mailBodyLocation = new StringBuilder();
                    string locationEmail = email;
                    bool foundCourses = false;
                    foreach (OrderLine orderLine in order.CurrentOrderLines.Where(x => x.LocationEmail == locationEmail))
                    {
                        //if there is a booking for this centre, add it to the centre email

                        foundCourses = true;
                        mailBodyLocation.AppendFormat("<p><b>Course: {0} {1}</b></p>", orderLine.ProductTitle,
                                                      orderLine.ProductDescription);
                        mailBodyLocation.AppendFormat("<p>Booking Type: {0}</p>",
                                                      orderLine.PaymentTypeDescription);

                        mailBodyLocation.AppendFormat("<p>Booking Option: {0} &#163;{1}</p>",
                                                      orderLine.ProductOptionTitle,
                                                      orderLine.ProductPrice);
                        mailBodyLocation.AppendFormat("<p>Number of attendees: {0}", orderLine.Quantity);
                        mailBodyLocation.AppendFormat("<p>Total Payment: &#163;{0}</p>", orderLine.PaymentLineTotal);

                    }
                    //send the centre email
                    if (foundCourses)
                    {
                        mailBodyLocation.AppendFormat("<p>Special Requirements: {0}</p>", order.SpecialRequirements);
                        if (order.HasValidVoucher())
                        {
                            mailBodyLocation.AppendFormat("<p><b>The following voucher has been applied to this order: {0} {1}.</b></p>", order.VoucherInfo, order.GetVoucherDetail());
                        }
                        
                        SendEmail("reception@field-studies-council.org", locationEmail,
                                  "WEBSITE Individuals and Families Booking",
                                  mailBodyNotification.ToString() + mailBodyLocation.ToString());
                    }
                }


                mailBodyNotification.Append(mailBodyCourses);
                mailBodyCustomer.Append(mailBodyCourses);

            }
            if (order.ContainsProductType("M"))
            {
                sendMail = true;

                mailBodyNotification.Append("<p>This person has requested the following membership:</p>");

                mailBodyCustomer.Append(
                    "<p>Thank you for joining FSC.  Your membership will support our work to inspire young people to understand and experience the natural world including the wider benefits that a greater appreciation of nature can bring.</p>");
                mailBodyCustomer.Append(
                    "<p>A welcome pack will be sent to you by post within 2-3 weeks. You will receive a membership card that is renewed annually.  We produce a members’ magazine FSC Magazine twice a year and you will also receive access to an exclusive members’ preview of the annual leisure learning and professional development programme.</p>");

                foreach (OrderLine orderLine in order.GetMembershipOrderLines())
                {

                    mailBodyMemberships.AppendFormat("<p><b>{0} {1}</b></p>", orderLine.ProductTitle,
                                                     orderLine.ProductDescription);
                    mailBodyMemberships.AppendFormat("<p>Total Payment: &#163;{0}</p>", orderLine.PaymentLineTotal);
                }
                mailBodyNotification.Append(mailBodyMemberships);
                mailBodyCustomer.Append(mailBodyMemberships);
            }

            if (order.ContainsProductType("D"))
            {

                sendMail = true;

                mailBodyNotification.Append("<p>This person has made the following donation(s):</p>");

                mailBodyCustomer.Append(
                    "<p>Thank you for your donation which will support our work to inspire young people to understand and experience the natural world including the wider benefits that a greater appreciation of nature can bring.</p>");

                foreach (OrderLine orderLine in order.GetDonationOrderLines())
                {

                    mailBodyDonations.AppendFormat("<p><b>{0} {1}</b></p>", orderLine.ProductTitle,
                                                   orderLine.ProductDescription);
                    mailBodyDonations.AppendFormat("<p>Total Payment: &#163;{0}</p>", orderLine.PaymentLineTotal);
                }
                mailBodyNotification.Append(mailBodyDonations);
                mailBodyCustomer.Append(mailBodyDonations);

            }

            if (order.GiftAidAgreement)
            {
                mailBodyNotification.Append("<p><b>This person has made a Gift Aid Agreement.</b></p>");

                mailBodyCustomer.Append(
                    "Thank you for making a Gift Aid declaration.");
            }

            if (order.HasValidVoucher())
            {
                mailBodyNotification.AppendFormat("<p><b>The following voucher has been applied to this order: {0} {1}.</b></p>", order.VoucherInfo, order.GetVoucherDetail());
                mailBodyCustomer.AppendFormat("<p><b>The following voucher has been applied to your order: {0} {1}.</b></p>", order.VoucherInfo, order.GetVoucherDetail());
            }


            if (sendMail)
            {

                //send the notification email
                SendEmail("webmaster@field-studies-council.org", "enquiries@field-studies-council.org",
                          "WEBSITE Individuals and Families Booking",
                          mailBodyNotification.ToString());
                //send the customer email
                SendEmail("reception@field-studies-council.org", contact.Email, "FSC Individuals and Families Booking",
                          mailBodyCustomer.ToString());
            }

        

        }

        private void SendEmail(string emailfrom, string to, string subject, string body)
        {
            MailMessage mail = new MailMessage(emailfrom, to, subject, body);
            mail.IsBodyHtml = true;
            MailAddress webmaster = new MailAddress("webmaster@field-studies-council.org");
            mail.CC.Add(webmaster);
            mail.BodyEncoding = Encoding.ASCII;
            SmtpClient client = new SmtpClient("localhost");
            //client.DeliveryMethod = SmtpDeliveryMethod.PickupDirectoryFromIis;
            client.Send(mail);
        }

    }
}
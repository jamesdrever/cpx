using System;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mef;
using Autofac.Integration.Mvc;
using CustomerPortalExtensions.Application.Contacts;
using CustomerPortalExtensions.Application.Ecommerce;
using CustomerPortalExtensions.Application.Ecommerce.Locations;
using CustomerPortalExtensions.Application.Ecommerce.OrderQueue;
using CustomerPortalExtensions.Application.Email;
using CustomerPortalExtensions.Infrastructure.Contacts;
using CustomerPortalExtensions.Infrastructure.ECommerce.Discounts;
using CustomerPortalExtensions.Infrastructure.ECommerce.Location;
using CustomerPortalExtensions.Infrastructure.ECommerce.Orders;
using CustomerPortalExtensions.Infrastructure.ECommerce.Products;
using CustomerPortalExtensions.Infrastructure.ECommerce.Shipping;
using CustomerPortalExtensions.Infrastructure.ECommerce.Vouchers;
using CustomerPortalExtensions.Infrastructure.Email;
using CustomerPortalExtensions.Infrastructure.Services.Database;
using CustomerPortalExtensions.Infrastructure.Synchronisation;
using CustomerPortalExtensions.Interfaces.Config;
using CustomerPortalExtensions.Interfaces.Contacts;
using CustomerPortalExtensions.Interfaces.ECommerce;
using CustomerPortalExtensions.Interfaces.Ecommerce;
using CustomerPortalExtensions.Interfaces.Email;
using CustomerPortalExtensions.Interfaces.Synchronisation;
using Umbraco.Core;
using Configuration = CustomerPortalExtensions.Helper.Config.Configuration;

namespace CustomerPortalExtensions.MVC
{
    public class CustomerPortalExtensionsApplication : IApplicationEventHandler
    {
        void IApplicationEventHandler.OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }

        void IApplicationEventHandler.OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

            //set up MEF to look for plugins in the /bin/plugins folder
            string codeBase = Assembly.GetExecutingAssembly().CodeBase;
            UriBuilder uri = new UriBuilder(codeBase);
            string path = Uri.UnescapeDataString(uri.Path);
            string exeLocation=Path.GetDirectoryName(path);
            string pluginPath = Path.Combine(exeLocation, "plugins");
            var pluginCatalog = new DirectoryCatalog(pluginPath);

            var builder = new ContainerBuilder();

            //register all controllers found in this assembly
            builder.RegisterControllers(typeof(CustomerPortalExtensionsApplication).Assembly);

            //add "internal" dependencies via Autofac
            builder.RegisterType<ContactRepository>().As<IContactRepository>();
            builder.RegisterType<ContactSecondaryRepository>().As<IContactSecondaryRepository>();
            builder.RegisterType<ContactAuthenticationHandler>().As<IContactAuthenticationHandler>();
            builder.RegisterType<DefaultContactSynchroniser>().As<IContactSynchroniser>();
            builder.RegisterType<Configuration>().As<IConfigurationService>();
            builder.RegisterType<ContactService>().As<IContactService>();
            builder.RegisterType<MailChimpEmailSubscriptionConnector>().As<IEmailSubscriptionConnector>();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>();
            builder.RegisterType<DiscountHandlerFactory>().As<IDiscountHandlerFactory>();
            builder.RegisterType<EcommerceService>().As<IEcommerceService>();
            builder.RegisterType<OrderCoordinator>().As<IOrderCoordinator>();
            builder.RegisterType<ShippingHandlerFactory>().As<IShippingHandlerFactory>();
            builder.RegisterType<ProductRepository>().As<IProductRepository>();
            builder.RegisterType<VoucherRepository>().As<IVoucherRepository>();
            builder.RegisterType<LocationHandlerFactory>().As<ILocationHandlerFactory>();
            builder.RegisterType<LocationHandler>().As<ILocationHandler>();
            builder.RegisterType<OrderQueueService>().As<IOrderQueueService>();
            builder.RegisterType<EmailSubscriptionsService>().As<IEmailSubscriptionsService>();
            builder.RegisterType<OrderQueue>().As<IOrderQueue>();
            builder.RegisterType<BespokePricingHandlerFactory>().As<IBespokePricingHandlerFactory>();
            builder.RegisterType<AdditionalQueueProcessingHandlerFactory>()
                   .As<IAdditionalQueueProcessingHandlerFactory>();
            builder.RegisterType<Database>()
                   .InstancePerHttpRequest()
                   .WithParameter("connectionString", ConfigurationManager.ConnectionStrings["umbracoDbDSN"].ConnectionString)
                   .WithParameter("providerName", "System.Data.SqlClient");

            //add in "exeternal" dependencies via MEF
            builder.RegisterComposablePartCatalog(pluginCatalog);

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        void IApplicationEventHandler.OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
        }
    }
}
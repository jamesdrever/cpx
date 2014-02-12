using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System.Threading;

namespace CustomerPortalExtensions.Tests
{
    [TestClass]
    public class SeleniumTests
    {
        private string TestDomain = "http://fscho-it-dev.local/";
        /// <summary>
        /// Add two publications to cart
        /// Checks totals at each stage
        /// Updates special requirements
        /// Checks in
        /// </summary>
        [TestMethod]
        public void ShouldAddTwoBookingsToOrderInFirefox()
        {
            //open Firefox
            var driver = new FirefoxDriver();


            //first delete any existing orders
            driver.Navigate().GoToUrl(TestDomain + "individuals-and-families/your-courses.aspx");

            var deleteButtons=driver.FindElementsByClassName("deleteOrderLineButton");

            foreach (var deleteButton in deleteButtons)
            {
                deleteButton.Click();
            }


            //go to course information page
            driver.Navigate().GoToUrl(TestDomain+"individuals-and-families/courses/2013/fm/identifying-clovers,-medicks-and-vetches-40531.aspx");
            
            //select submit button for booking option
            IWebElement button1 = driver.FindElement(By.Name("24789"));
            //click button (and execute jQuery to add booking option to basket)
            driver.ClickButton(button1);

            //wait for JQuery to complete
            waitForJQuery(driver);

            //go to basket page
            driver.Navigate().GoToUrl(TestDomain+"individuals-and-families/your-courses.aspx");
            IWebElement orderTotal = driver.FindElement(By.ClassName("paymentTotal"));

            //check that cart total is £248
            Assert.IsTrue(orderTotal.Text == "£248.00");

            driver.Navigate().GoToUrl(TestDomain+"individuals-and-families/courses/2013/rc/walking-hidden-wales-40889.aspx");

            IWebElement button2 = driver.FindElement(By.Id("25039"));
            //click button (and execute jQuery to add booking option to basket)
            driver.ClickButton(button2);


            //wait for JQuery to complete
            waitForJQuery(driver);

            //go to basket page
            driver.Navigate().GoToUrl(TestDomain+"individuals-and-families/your-courses.aspx");
            orderTotal = driver.FindElement(By.ClassName("paymentTotal"));

            //check that cart total is £282
            Assert.IsTrue(orderTotal.Text == "£658.00");

            //find Special Requirements textbox
            IWebElement specialRequirementsTextBox = driver.FindElement(By.Id("specialRequirements"));
            specialRequirementsTextBox.Clear();
            specialRequirementsTextBox.SendKeys("These are my special requirements");

            //submit special requirements
            IWebElement specialRequirementsButton = driver.FindElement(By.ClassName("updateSpecialRequirements"));
            driver.ClickButton(specialRequirementsButton);

            //IWebElement infoBox = driver.FindElement(By.ClassName("info-box"), 10);
            //Assert.IsTrue(infoBox.Text == "You have successfully updated your special requirements!");

            //specialRequirementsTextBox = driver.FindElement(By.Id("specialRequirements"));
            //Assert.IsTrue(specialRequirementsTextBox.Text == "These are my special requirements");

            //go to checkout
            IWebElement checkoutButton = driver.FindElement(By.Id("CheckoutButton"));
            driver.ClickButton(checkoutButton);

            //find username field
            IWebElement userNameTextBox = driver.FindElement(By.Id("Login_Email"), 10);
            userNameTextBox.SendKeys("james@jamesdrever.co.uk");

            //find password field
            IWebElement passwordTextBox = driver.FindElement(By.Id("Login_Password"));
            passwordTextBox.SendKeys("123");

            //click login button
            IWebElement loginButton = driver.FindElement(By.Id("LoginButton"));
            driver.ClickButton(loginButton);

            orderTotal = driver.FindElement(By.ClassName("paymentTotal"), 10);
            //check that cart total is £658
            Assert.IsTrue(orderTotal.Text == "£658.00");

            specialRequirementsTextBox = driver.FindElement(By.Id("specialRequirements"));

            Assert.IsTrue(specialRequirementsTextBox.Text == "These are my special requirements");

            driver.Quit();

        }

        /// <summary>
        /// Add two publications to cart
        /// Checks totals at each stage
        /// </summary>
        [TestMethod]
        public void AddTwoPublicationsToCartInFirefox()
        {
            //open Firefox
            var driver = new FirefoxDriver();

            //go to course information page
            driver.Navigate().GoToUrl(TestDomain+"/publications/pubs/plants-common-on-sand-dunes.aspx");

            //select submit button for booking option
            IWebElement button1 = driver.FindElement(By.Id("addToOrderButton"));
            //click button (and execute jQuery to add booking option to basket)
            driver.ClickButton(button1);

            //wait for JQuery to complete
            waitForJQuery(driver);

            //go to basket page
            driver.Navigate().GoToUrl(TestDomain + "publications/your-publications.aspx");
            IWebElement cartTotal = driver.FindElement(By.ClassName("paymentSubTotal"));

            //check that cart total is £3.50
            Assert.IsTrue(cartTotal.Text == "£2.75");

            //check that discount is £0
            IWebElement cartDiscount = driver.FindElement(By.ClassName("orderDiscountTotal"));
            Assert.IsTrue(cartDiscount.Text == "-£0.00");

            //check that delivery is £1
            IWebElement cartPostage = driver.FindElement(By.ClassName("orderShippingTotal"));
            Assert.IsTrue(cartPostage.Text == "£1.00");

            //check that overall total is £1
            IWebElement cartOverallTotal = driver.FindElement(By.ClassName("paymentTotal"));
            Assert.IsTrue(cartOverallTotal.Text == "£3.75");

            driver.Navigate().GoToUrl(TestDomain + "publications/pubs/using-digital-maps-and-gps-in-fieldwork-a-practical-guide-for-teachers.aspx");

            IWebElement button2 = driver.FindElement(By.Id("addToOrderButton"));
            //click button (and execute jQuery to add booking option to basket)
            driver.ClickButton(button2);

            //wait for JQuery to complete
            waitForJQuery(driver);

            //go to basket page
            driver.Navigate().GoToUrl(TestDomain + "publications/your-publications.aspx");
            cartTotal = driver.FindElement(By.ClassName("paymentSubTotal"));

            //check that cart total is £7.75
            Assert.IsTrue(cartTotal.Text == "£7.75");

            //check that discount is £0
            cartDiscount = driver.FindElement(By.ClassName("orderDiscountTotal"));
            Assert.IsTrue(cartDiscount.Text == "-£0.00");

            //check that delivery is £1
            cartPostage = driver.FindElement(By.ClassName("orderShippingTotal"));
            Assert.IsTrue(cartPostage.Text == "£2.00");

            //check that overall total is £1
            cartOverallTotal = driver.FindElement(By.ClassName("paymentTotal"));
            Assert.IsTrue(cartOverallTotal.Text == "£9.75");

            //go to checkout
            IWebElement checkoutButton = driver.FindElement(By.Id("CheckoutButton"));
            driver.ClickButton(checkoutButton);

            //find username field
            IWebElement userNameTextBox = driver.FindElement(By.Id("Login_Email"), 10);
            userNameTextBox.SendKeys("james@jamesdrever.co.uk");

            //find password field
            IWebElement passwordTextBox = driver.FindElement(By.Id("Login_Password"));
            passwordTextBox.SendKeys("123");

            //click login button
            IWebElement loginButton = driver.FindElement(By.Id("LoginButton"));
            driver.ClickButton(loginButton);

            cartTotal = driver.FindElement(By.ClassName("paymentTotal"), 10);
            //check that cart total is £282
            Assert.IsTrue(cartTotal.Text == "£11.75");

            driver.Quit();



        }




        private void waitForJQuery(FirefoxDriver driver)
        {
            while (true) // Handle timeout somewhere
            {
                var ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete)
                    break;
                Thread.Sleep(100);
            }
        }

    }
    public static class WebDriverExtensions
    {
        public static IWebElement FindElement(this IWebDriver driver, By by, int timeoutInSeconds)
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }
            return driver.FindElement(by);
        }

        public static void ClickButton(this IWebDriver driver, IWebElement button)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].click();", button);
        }
    }
}


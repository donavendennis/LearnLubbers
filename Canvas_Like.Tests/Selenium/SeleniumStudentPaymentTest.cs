using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Canvas_Like.Tests.Selenium
{
    [TestFixture]
    internal class SeleniumStudentPaymentTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        //private string appUrl = "http://localhost:5290/"; // Ensure this URL works on the machine it's running on
        private string appUrl = "https://canvaslike20241008102221.azurewebsites.net/";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors"); // Ignore insecure certificates

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void LoginWithValidCredentials_ShouldSucceed()
        {
            // Open the application base URL to ensure it's loaded
            driver.Navigate().GoToUrl(appUrl);

            // Wait until the email input is present on the login page
            wait.Until(d => d.FindElement(By.Id("Input_Email")));

            // Locate and enter the email
            var emailField = driver.FindElement(By.CssSelector("input[id='Input_Email']"));
            emailField.Clear();
            emailField.SendKeys("student@weber.edu");

            // Locate and enter the password
            var passwordField = driver.FindElement(By.CssSelector("input[id='Input_Password']"));
            passwordField.Clear();
            passwordField.SendKeys("Password123*");

            // Check the 'Remember me' checkbox if it pops up
            var rememberMeCheckbox = driver.FindElement(By.Id("Input_RememberMe"));
            if (!rememberMeCheckbox.Selected)
            {
                rememberMeCheckbox.Click();
            }

            // Find and click the submit button
            var loginButton = driver.FindElement(By.Id("login-submit"));
            loginButton.Click();

            // Find and click the Account link
            var accountLink = driver.FindElement(By.LinkText("Account"));
            accountLink.Click();

            // Find and enter payment amount
            var paymentField = driver.FindElement(By.Id("amount"));
            paymentField.Clear();
            paymentField.SendKeys("1.00");

            // Find and enter Name on Card
            var nameField = driver.FindElement(By.Id("name-on-card"));
            nameField.Clear();
            nameField.SendKeys("Cardholder Name");

            // Enter card number
            SwitchToIframeByTitle("Secure card number input frame");
            var cardNumberField = wait.Until(d => d.FindElement(By.Name("cardnumber")));
            cardNumberField.SendKeys("4242424242424242");
            driver.SwitchTo().DefaultContent();

            // Enter expiration date
            SwitchToIframeByTitle("Secure expiration date input frame");
            var expiryDateField = wait.Until(d => d.FindElement(By.Name("exp-date")));
            expiryDateField.SendKeys("1234");
            driver.SwitchTo().DefaultContent();

            // Enter CVC
            SwitchToIframeByTitle("Secure CVC input frame");
            var cvcField = wait.Until(d => d.FindElement(By.Name("cvc")));
            cvcField.SendKeys("123");
            driver.SwitchTo().DefaultContent();

            // Find and click the submit payment button
            var submitPaymentButton = driver.FindElement(By.Id("pay"));
            submitPaymentButton.Click();

            // Wait for the success message to appear
            IWebElement successMessageElement = null;
            wait.Until(driver =>
            {
                try
                {
                    successMessageElement = driver.FindElement(By.Id("success"));
                    return successMessageElement.Displayed;
                }
                catch (NoSuchElementException)
                {
                    // If element is not found, return false and keep waiting
                    return false;
                }
            });

            // Get the success message text
            var successMessage = successMessageElement.Text;

            // Assert that the success message contains the expected text
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(successMessage.Contains("successful"), "Payment was not successful or success message not found.");
        }

        private void SwitchToIframeByTitle(string title)
        {
            try
            {
                var iframe = wait.Until(d => d.FindElement(By.CssSelector($"iframe[title='{title}']")));
                driver.SwitchTo().Frame(iframe);
            }
            catch (WebDriverTimeoutException e)
            {
                Microsoft.VisualStudio.TestTools.UnitTesting.Assert.Fail($"Could not find the Stripe iframe with title '{title}': {e.Message}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Close the browser and dispose of resources
            driver.Quit();
            driver.Dispose();
        }
    }
}

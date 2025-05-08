using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OpenQA.Selenium.Chrome;

namespace Canvas_Like.Tests.Selenium
{
    [TestFixture]
    public class SeleniumClassDelete
    {
        private IWebDriver driver;

        private WebDriverWait wait;

        private string appUrl = "https://canvaslike20241008102221.azurewebsites.net/"; // Make sure this URL works on the machine it's running on
        //private string appUrl = "http://localhost:5290/"; // Make sure this URL works on the machine it's running on

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors"); // Ignore insecure certificates
            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [Test]
        public void ClassDelete_ShouldSucceed()
        {
            // clear cookies
            driver.Manage().Cookies.DeleteAllCookies();

            // Open the application base URL to ensure it's loaded
            driver.Navigate().GoToUrl(appUrl);
            
            // Wait until the email input is present on the login page
            wait.Until(d => d.FindElement(By.Id("Input_Email")));

            // Locate and enter the email
            var emailField = driver.FindElement(By.Id("Input_Email"));
            emailField.Clear();
            emailField.SendKeys("arpitchristi@weber.edu");

            // Locate and enter the password
            var passwordField = driver.FindElement(By.Id("Input_Password"));
            passwordField.Clear();
            passwordField.SendKeys("ChrisiIsTheBest2024!");

            // Check the 'Remember me' checkbox if it pops up
            var rememberMeCheckbox = driver.FindElement(By.Id("Input_RememberMe"));
            if (!rememberMeCheckbox.Selected)
            {
                rememberMeCheckbox.Click();
            }

            // Find and click the submit button
            var loginButton = driver.FindElement(By.Id("login-submit"));
            loginButton.Click();

            // Wait for the Classes link to appear and click it
            wait.Until(d => d.FindElement(By.LinkText("Classes"))).Click();

            // Locate the row containing the class title
            var classRow = wait.Until(d =>
                d.FindElement(
                    By.XPath("//tr[td[2][contains(text(), '3280')]]")));

            // Find the "Delete" link within that row and click it
            var deleteLink = classRow.FindElement(By.LinkText("Delete"));
            deleteLink.Click();

            // Wait for the confirmation modal to appear
            wait.Until(d => d.FindElement(By.Id("delete")));
            var confirmButton = driver.FindElement(By.Id("delete"));
            confirmButton.Click();

            // wait until the Add a class link is available to confirm successful return to classes page and validate
            wait.Until(d => d.FindElement(By.LinkText("Add a class")));

            var currentURL = driver.Url;
            NUnit.Framework.Assert.That(currentURL.Contains("Classes"), Is.True, "Success message not displayed after class");
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}

using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;

namespace Canvas_Like.Tests.Selenium
{
  [TestFixture]
  public class SeleniumUserCanSubmitTest
  {
    private IWebDriver driver;
    private WebDriverWait wait;
    private string appUrl = "https://canvaslike20241008102221.azurewebsites.net/"; // Make sure this URL works on the machine it's running on
    //private string appUrl = "http://localhost:5290/"; // Make sure this URL works on the machine it's running on

    [SetUp]
    public void Setup()
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
        public void LoginWithValidCredentials_ShouldSucceed()
        {
            // Open the application base URL to ensure it's loaded
            driver.Navigate().GoToUrl(appUrl);

            // Wait for the Login link to appear and click it
            //wait.Until(d => d.FindElement(By.LinkText("Login"))).Click();

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

            var courseCards = driver.FindElements(By.ClassName("course-card"));
            var courseCard = courseCards.FirstOrDefault();

            courseCard.Click();

            var submitButtons = driver.FindElements(By.TagName("button"));
            var submitButton = submitButtons.Where((b) => b.Text == "Submit").FirstOrDefault();

            submitButton.Click();
            Thread.Sleep(5000);

            var textEntry = driver.FindElement(By.Id("objSubmission_Submission"));

            var submitAssignmentButtons = driver.FindElements(By.TagName("button"));
            var submitAssignmentButton = submitAssignmentButtons.Where((b) => b.Text == "Submit Assignment").FirstOrDefault();
            try
            {
                textEntry.Clear();
                textEntry.SendKeys("This is an automation");
                submitAssignmentButton.Click();
            }
            catch
            {
                var returnToAssignmentButtons = driver.FindElements(By.TagName("button"));
                var returnToAssignment = returnToAssignmentButtons.Where((b) => b.Text == "Return to Assignments").FirstOrDefault();
                returnToAssignment.Click();
            }
            Thread.Sleep(5000);
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
}

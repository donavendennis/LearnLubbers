using System;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace Canvas_Like.Tests.Selenium
{
    [TestFixture]
    public class SeleniumClassCreationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private string appUrl = "https://canvaslike20241008102221.azurewebsites.net/";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddArgument("--ignore-certificate-errors");

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
        }

        [Test]
        public void ClassCreation_ShouldSucceed()
        {
            // Perform login
            PerformLogin("arpitchristi@weber.edu", "ChrisiIsTheBest2024!");

            // Wait for the Classes link to appear and click it
            wait.Until(d => d.FindElement(By.LinkText("Classes"))).Click();

            // Wait for the Add a class link to appear and click it
            wait.Until(d => d.FindElement(By.LinkText("Add a class"))).Click();

            // Fill in class forms
            wait.Until(d => d.FindElement(By.Id("ddlDepartment"))).Click();
            var departmentDropdown = new SelectElement(driver.FindElement(By.Id("ddlDepartment")));
            departmentDropdown.SelectByIndex(1);

            driver.FindElement(By.Id("objClass_CourseNumber")).SendKeys("3280");
            driver.FindElement(By.Id("objClass_Title")).SendKeys("Object Oriented Windows Application Development");

            driver.FindElement(By.Id("objDateStart")).SendKeys("11-30-2024");
            driver.FindElement(By.Id("objDateEnd")).SendKeys("03-31-2025");

            driver.FindElement(By.Id("objClass_Building")).SendKeys("EH");
            driver.FindElement(By.Id("objClass_RoomNumber")).SendKeys("368");

            driver.FindElement(By.Id("Monday")).Click();
            driver.FindElement(By.Id("Wednesday")).Click();
            driver.FindElement(By.Id("Friday")).Click();

            driver.FindElement(By.Id("objTimeStart")).SendKeys("09:00 AM");
            driver.FindElement(By.Id("objTimeEnd")).SendKeys("10:30 AM");

            driver.FindElement(By.Id("objClass_Capacity")).SendKeys("20");
            driver.FindElement(By.Id("objClass_CreditHours")).SendKeys("4");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
            Thread.Sleep(1000);

            driver.FindElement(By.Id("class-submit")).Click();

            wait.Until(d => d.FindElement(By.LinkText("Add a class")));

            var currentURL = driver.Url;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(currentURL.Contains("Classes"), "Success message not displayed after class creation.");
        }


        private void PerformLogin(string email, string password)
        {
            // Open the application base URL to ensure it's loaded
            driver.Navigate().GoToUrl(appUrl);

            // Wait until the email input is present on the login page
            wait.Until(d => d.FindElement(By.Id("Input_Email")));

            // Locate and enter the email
            var emailField = driver.FindElement(By.CssSelector("input[id='Input_Email']"));
            emailField.Clear();
            emailField.SendKeys(email);

            // Locate and enter the password
            var passwordField = driver.FindElement(By.CssSelector("input[id='Input_Password']"));
            passwordField.Clear();
            passwordField.SendKeys(password);

            // Check the 'Remember me' checkbox if it pops up
            var rememberMeCheckbox = driver.FindElement(By.Id("Input_RememberMe"));
            if (!rememberMeCheckbox.Selected)
            {
                rememberMeCheckbox.Click();
            }

            // Find and click the submit button
            var loginButton = driver.FindElement(By.Id("login-submit"));
            loginButton.Click();
        }


        private void NavigateToClasses()
        {
            // Wait for the "Classes" link and click it
            wait.Until(d => d.FindElement(By.LinkText("Classes"))).Click();

            // Wait for the "Add a class" link and click it
            wait.Until(d => d.FindElement(By.LinkText("Add a class"))).Click();
        }

        private void AddClass()
        {
            // Fill out the form for class creation
            wait.Until(d => d.FindElement(By.Id("ddlDepartment"))).Click();
            new SelectElement(driver.FindElement(By.Id("ddlDepartment"))).SelectByIndex(1);

            driver.FindElement(By.Id("objClass_CourseNumber")).SendKeys("3280");
            driver.FindElement(By.Id("objClass_Title")).SendKeys("Object Oriented Windows Application Development");
            driver.FindElement(By.Id("objDateStart")).SendKeys("11-30-2024");
            driver.FindElement(By.Id("objDateEnd")).SendKeys("03-31-2025");
            driver.FindElement(By.Id("objClass_Building")).SendKeys("EH");
            driver.FindElement(By.Id("objClass_RoomNumber")).SendKeys("368");

            driver.FindElement(By.Id("Monday")).Click();
            driver.FindElement(By.Id("Wednesday")).Click();
            driver.FindElement(By.Id("Friday")).Click();

            driver.FindElement(By.Id("objTimeStart")).SendKeys("09:00 AM");
            driver.FindElement(By.Id("objTimeEnd")).SendKeys("10:30 AM");

            driver.FindElement(By.Id("objClass_Capacity")).SendKeys("20");
            driver.FindElement(By.Id("objClass_CreditHours")).SendKeys("4");

            // Submit the form
            driver.FindElement(By.Id("class-submit")).Click();
        }

        private void VerifyClassCreation()
        {
            // Wait for the "Add a class" link to reappear, indicating return to the Classes page
            wait.Until(d => d.FindElement(By.LinkText("Add a class")));

            // Verify the current URL
            var currentURL = driver.Url;
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(
                currentURL.Contains("Classes"),
                "Class creation failed. Classes page not displayed after submission."
            );
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
            driver.Dispose();
        }
    }
}

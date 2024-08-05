using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UchaSe
{
    public class UserFunctionalitiesLogin
    {
        private IWebDriver _driver;
        protected readonly string _url = "https://ucha.se/";
        private UserFunctionalitiesRegister _userRegister;

        private HelperMethods _helper;

        [OneTimeSetUp]
        public void Setup()
        {
            _helper = new HelperMethods();
            _userRegister = new UserFunctionalitiesRegister();

            var options = new ChromeOptions();
            options.AddArguments("--disable-search-engine-choice-screen");
            options.AddArguments("--no-first-run");
            options.AddArguments("--disable-notifications");
            options.AddArguments("--disable-infobars");

            _driver = new ChromeDriver(options);
            _driver.Navigate().GoToUrl(this._url);
            _driver.Manage().Window.Maximize();
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Test, Order(1)]
        public void UserNavigateTo_LoginPage()
        {
            _driver.FindElement(By.XPath("//li[@id='menu-item-login']//a")).Click();

            string expectedUrl = "https://ucha.se/login/";

            Assert.True(_driver.Url == expectedUrl);
        }

        [Test, Order(2)]
        public void UserLogin_WithValidCredentials()
        {
            string? email = _userRegister._userEmail;
            string? password = _userRegister._password;
            string? registeredUserName = _userRegister._parentName;

            _driver.FindElement(By.XPath("//div[@id='field-email']//input")).SendKeys(email);
            _driver.FindElement(By.XPath("//div[@id='field-password']//input")).SendKeys(password);
            _driver.FindElement(By.XPath("//input[@id='send_data']")).Click();            

            var userLoginName = _driver.FindElement(By.XPath("//span[@id='username-sidebar']"));

            Assert.True(userLoginName.Displayed);
            Assert.That(userLoginName.Text, Is.EqualTo(registeredUserName));
        }
    }
}

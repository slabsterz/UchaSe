using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Net;

namespace UchaSe
{
    public class UserFunctionalitiesRegister
    {
        private IWebDriver _driver;
        protected readonly string _url = "https://ucha.se/";

        private HelperMethods _helper;

        [SetUp]
        public void Setup()
        {
            _helper = new HelperMethods();

            var options = new ChromeOptions();
            options.AddArguments("--disable-search-engine-choice-screen");
            options.AddArguments("--no-first-run");
            options.AddArguments("--disable-notifications");
            options.AddArguments("--disable-infobars");

            _driver = new ChromeDriver(options);
            _driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [TearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Test]
        public void UserNavigateTo_HomePage()
        {
            _driver.Navigate().GoToUrl(_url);

            Assert.That(_driver.Url, Is.EqualTo(_url));
        }

        [Test]
        public void UserNavigateTo_RegiserPage()
        {
            _driver.Navigate ().GoToUrl(_url);
            _driver.FindElement(By.XPath("//button[@class='btn btn-primary cookie-user-choice order-1 order-md-2 mt-2 mt-md-0']")).Click();  

            string registerPageUrl = "https://ucha.se/registration/";

            var registerButton = _driver.FindElement(By.Id("register-top"));
            registerButton.Click();
            string welcomeMessage = _driver.FindElement(By.XPath("//h1[@id='page-title']")).Text;

            Assert.True(_driver.Url == registerPageUrl);
            Assert.That(welcomeMessage, Is.EqualTo("Стани част от Уча.се!"));
        }

        [Test]
        public void UserPickMascot_RegiserPage()
        {
            _driver.Navigate().GoToUrl(_url);
            _driver.FindElement(By.XPath("//button[@class='btn btn-primary cookie-user-choice order-1 order-md-2 mt-2 mt-md-0']")).Click();

            var registerButton = _driver.FindElement(By.Id("register-top"));
            registerButton.Click();
            _driver.FindElement(By.XPath("//div[@data-kind='student']")).Click();

            Assert.True(_driver.Url != _url);

            var registerWithEmailButton = _driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg btn-md-xl reg-email']"));
            var registerWithFacebookButton = _driver.FindElement(By.Id("fb-auth"));

            Assert.Multiple(() => {
                Assert.True(registerWithEmailButton.Displayed);
                Assert.True(registerWithFacebookButton.Displayed);
            }); 
        }

        [Test]
        public void UserRegistration_WithEmail()
        {
            _driver.Navigate().GoToUrl(_url);
            _driver.FindElement(By.XPath("//button[@class='btn btn-primary cookie-user-choice order-1 order-md-2 mt-2 mt-md-0']")).Click();

            var registerButton = _driver.FindElement(By.Id("register-top"));
            registerButton.Click();
            _driver.FindElement(By.XPath("//div[@data-kind='student']")).Click();

            _driver.FindElement(By.XPath("//button[@data-kind='student']")).Click();
            

            var form = _driver.FindElement(By.Id("form-section"));

            Assert.True(form.Displayed);

            string userEmail = _helper.GenerateRandomEmail();
            string userPhoneNumber = _helper.GenerateRandomMobilePhone();
            string randomUser = _helper.GenerateRandomUser();

            _driver.FindElement(By.XPath("//input[@name='username']")).SendKeys(randomUser);
            _driver.FindElement(By.Id("phone-number-value")).SendKeys(userPhoneNumber);
            _driver.FindElement(By.Id("next")).Click();

            _driver.FindElement(By.XPath("//input[@name='email']")).SendKeys(userEmail);
            _driver.FindElement(By.Id("new-password")).SendKeys("password123");
            _driver.FindElement(By.Id("new-password2")).SendKeys("password123");
            _driver.FindElement(By.Id("next")).Click();

            //select grade
            _driver.FindElement(By.XPath("//button[@data-id='grade-filter']")).Click();
            _driver.FindElement(By.XPath("//*[@id=\"field-grade\"]/div/div/div[2]/a[1]/span/span[1]")).Click();

            //select city
            _driver.FindElement(By.XPath("//button[@data-id='city']")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));           

            var citySelect = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"town\"]/div/div/div[2]/a[2]")));
            citySelect.Click();

            //select school
            _driver.FindElement(By.XPath("//button[@data-id='school-filter']")).Click();
            _driver.FindElement(By.XPath("//*[@id=\"school\"]/div/div/div[2]/a[1]/span/span[1]")).Click();

            _driver.FindElement(By.XPath("//label[@class='custom-control-label' and @for='customCheck2']")).Click();
            _driver.FindElement(By.XPath("//label[@class='custom-control-label' and @for='customCheck1']")).Click();

            _driver.FindElement(By.Id("send_data")).Click();

            var registrationConfirmation = _driver.FindElement(By.XPath("//section[@id='registration-success']//div[text()='Трябва да потвърдиш регистрацията си!']"));

            Assert.True(registrationConfirmation.Displayed);

            var confirmationText = _driver.FindElement(By.XPath("//div[@class='form-body']//div")).Text;

            Assert.That(confirmationText, Does.Contain(userEmail));
        }
    }
}
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
        public readonly string? _userEmail;
        public readonly string? _password;

        private HelperMethods _helper;

        [OneTimeSetUp]
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

        [OneTimeTearDown]
        public void TearDown()
        {
            _driver.Quit();
            _driver.Dispose();
        }

        [Test, Order(1)]
        public void UserNavigateTo_HomePage()
        {
            _driver.Navigate().GoToUrl(_url);

            Assert.That(_driver.Url, Is.EqualTo(_url));
        }

        [Test, Order(2)]
        public void UserNavigateTo_RegiserPage()
        {
            _driver.FindElement(By.XPath("//button[@class='btn btn-primary cookie-user-choice order-1 order-md-2 mt-2 mt-md-0']")).Click();  

            string registerPageUrl = "https://ucha.se/registration/";

            var registerButton = _driver.FindElement(By.Id("register-top"));
            registerButton.Click();
            string welcomeMessage = _driver.FindElement(By.XPath("//h1[@id='page-title']")).Text;

            Assert.True(_driver.Url == registerPageUrl);
            Assert.That(welcomeMessage, Is.EqualTo("Стани част от Уча.се!"));
        }

        [Test, Order(3)]
        public void UserPickMascot_RegiserPage()
        {            
            _driver.FindElement(By.XPath("//div[@data-kind='student']")).Click();

            Assert.True(_driver.Url != _url);

            var registerWithEmailButton = _driver.FindElement(By.XPath("//button[@class='btn btn-primary btn-lg btn-md-xl reg-email']"));
            var registerWithFacebookButton = _driver.FindElement(By.Id("fb-auth"));

            Assert.Multiple(() => {
                Assert.True(registerWithEmailButton.Displayed);
                Assert.True(registerWithFacebookButton.Displayed);
            }); 
        }

        [Test, Order(4)]
        public void UserRegistration_WithEmail()
        {           
            _driver.FindElement(By.XPath("//button[@data-kind='student']")).Click();

            var form = _driver.FindElement(By.Id("form-section"));

            Assert.True(form.Displayed);

        }

        [Test, Order(5)]
        public void UserRegistration_WithEmail_ParentContactInput()
        {            
            string parentPhoneNumber = _helper.GenerateRandomMobilePhone();
            string parentName = _helper.GenerateRandomUser();

            _driver.FindElement(By.XPath("//input[@name='username']")).SendKeys(parentName);
            _driver.FindElement(By.Id("phone-number-value")).SendKeys(parentPhoneNumber);
            _driver.FindElement(By.Id("next")).Click();

            var inputUserEmailField = _driver.FindElement(By.XPath("//input[@name='email']"));
            var inputUserPasswordField = _driver.FindElement(By.Id("new-password"));
            var continueButton = _driver.FindElement(By.Id("next"));

            Assert.Multiple(() =>
            {
                Assert.True(inputUserEmailField.Displayed);
                Assert.True(inputUserPasswordField.Displayed);
                Assert.True(continueButton.Displayed);
            });
        }

        [Test, Order(6)]
        public void UserRegistration_WithEmail_StudentContactInput()
        {
            string userEmail = _helper.GenerateRandomEmail();
            string userPassword = _helper.GenerateRandomPassword();

            _driver.FindElement(By.XPath("//input[@name='email']")).SendKeys(userEmail);
            _driver.FindElement(By.Id("new-password")).SendKeys(userPassword);
            _driver.FindElement(By.Id("new-password2")).SendKeys(userPassword);
            _driver.FindElement(By.Id("next")).Click();

            var gradeSelector = _driver.FindElement(By.XPath("//button[@data-id='grade-filter']"));
            var citySelector = _driver.FindElement(By.XPath("//button[@data-id='city']"));
            var schoolSelector = _driver.FindElement(By.XPath("//button[@data-id='school-filter']"));
            var continueButton = _driver.FindElement(By.Id("send_data"));

            Assert.Multiple(() =>
            {
                Assert.True(gradeSelector.Displayed);
                Assert.True(citySelector.Displayed);
                Assert.True(schoolSelector.Displayed);
                Assert.True(continueButton.Displayed);
            });

            this._userEmail = userEmail;
            this._password = userPassword;
        }

        [Test, Order(7)]
        public void UserRegistration_WithEmail_StudentSchool()
        {        
            _driver.FindElement(By.XPath("//button[@data-id='grade-filter']")).Click();
            _driver.FindElement(By.XPath("//*[@id=\"field-grade\"]/div/div/div[2]/a[1]/span/span[1]")).Click();
            _driver.FindElement(By.XPath("//button[@data-id='city']")).Click();

            var wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(5));           

            var citySelect = wait.Until(ExpectedConditions.ElementToBeClickable(By.XPath("//*[@id=\"town\"]/div/div/div[2]/a[2]")));
            citySelect.Click();

            _driver.FindElement(By.XPath("//button[@data-id='school-filter']")).Click();
            _driver.FindElement(By.XPath("//*[@id=\"school\"]/div/div/div[2]/a[1]/span/span[1]")).Click();

            _driver.FindElement(By.XPath("//label[@class='custom-control-label' and @for='customCheck2']")).Click();
            _driver.FindElement(By.XPath("//label[@class='custom-control-label' and @for='customCheck1']")).Click();

            _driver.FindElement(By.Id("send_data")).Click();

            var registrationConfirmation = _driver.FindElement(By.XPath("//section[@id='registration-success']//div[text()='Трябва да потвърдиш регистрацията си!']"));

            Assert.True(registrationConfirmation.Displayed);

            var confirmationText = _driver.FindElement(By.XPath("//div[@class='form-body']//div")).Text;

            Assert.That(confirmationText, Does.Contain(_userEmail));
        }
    }
}
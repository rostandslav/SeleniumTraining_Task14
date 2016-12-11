using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;


namespace LinksOpenInNewWindowTest
{
    [TestClass]
    public class UnitTest1
    {
        private IWebDriver driver;
        private WebDriverWait wait;


        [TestInitialize]
        public void Init()
        {
            driver = new ChromeDriver();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(60));
        }


        private static Func<IWebDriver, string> AnyWindowOtherThan(ICollection<string> oldWindows)
        {
            return (driver) =>
            {
                List<string> newWindows = new List<string>(driver.WindowHandles);
                newWindows.RemoveAll(h => oldWindows.Contains(h));
                return newWindows.Count > 0 ? newWindows[0] : null;
            };
        }


        [TestMethod]
        public void LinksOpenInNewWindowTest()
        {
            driver.Url = "http://litecart/admin/";

            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Click();

            driver.FindElement(By.CssSelector("a[href='http://litecart/admin/?app=countries&doc=countries']")).Click();


            driver.FindElement(By.CssSelector("a[href='http://litecart/admin/?app=countries&doc=edit_country']")).Click();

            var externalLinks = driver.FindElements(By.XPath("//i[contains(@class, 'fa-external-link')]/parent::a"));
            int externalLinksCount = externalLinks.Count;

            ICollection<string> openedWindows = driver.WindowHandles;
            var countryFormWindow = driver.CurrentWindowHandle;

            for (int i = 0; i < externalLinksCount; i++)
            {
                externalLinks[i].Click();
                string newWindow = wait.Until(AnyWindowOtherThan(openedWindows));
                driver.SwitchTo().Window(newWindow);
                driver.Close();
                driver.SwitchTo().Window(countryFormWindow);
            }
        }


        [TestCleanup]
        public void Finish()
        {
            driver.Quit();
            //driver = null;
        }
    }
}

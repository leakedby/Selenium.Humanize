using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Selenium.Humanize.Models
{
    /// <summary>
    /// Implementations used by POM
    /// </summary>
    public abstract class BasePage : BaseModel
    {
        public Uri Url;

        public KeyboardMovement Keyboard;
        public MouseMovement Mouse;

        public BasePage(ref IWebDriver driver, ref Random random) : base(ref driver, ref random)
        {
            Keyboard = new KeyboardMovement(ref driver, ref random);
            Mouse = new MouseMovement(ref driver, ref random);
        }

        public void WaitForChanges()
        {
            Thread.Sleep(3000);
            //var wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(10));
            //wait.Until(drv => ExpectedConditions.StalenessOf(Driver.FindElement(By.TagName("body"))));
        }

        public void GoToPage()
        {
            if (string.IsNullOrEmpty(Url.AbsoluteUri))
                throw new Exception("Url is empty");

            Driver.Navigate().GoToUrl(Url);
        }

        /// <summary>
        /// Click <see cref="IWebElement"/> in humanized manner
        /// </summary>
        /// <param name="element"></param>
        public void Click(IWebElement element)
        {
            if (element.Selected)
                return;

            Mouse.Navigate(element);
            Mouse.Click();
        }

        /// <summary>
        /// Click & type <see cref="IWebElement"/> in humanized manner
        /// </summary>
        /// <param name="element"></param>
        public void SendKeys(IWebElement element, string text)
        {
            Click(element);
            Keyboard.Type(text);
        }

        /// <summary>
        /// Click & clear <see cref="IWebElement"/> in humanized manner
        /// </summary>
        /// <param name="element"></param>
        public void Clear(IWebElement element)
        {
            Click(element);
            element.Clear();
        }

        /// <summary>
        /// Get source code of page
        /// </summary>
        /// <returns></returns>
        public string GetSourceCode()
        {
            return Driver.ExecuteJavaScript<string>("return document.getElementsByTagName('html')[0].innerHTML");
        }

    }
}

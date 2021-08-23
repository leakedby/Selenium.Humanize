using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace Selenium.Humanize.Models
{
    public abstract class BaseModel
    {
        public IWebDriver Driver;
        public Random Rnd;

        public BaseModel(ref IWebDriver driver, ref Random random)
        {
            this.Driver = driver;
            this.Rnd = random;
        }

    }
}

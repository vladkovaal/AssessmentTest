using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace AssesmentTest
{
    public class UITests
    {
        private IWebDriver driver;
        private string baseUrl;
        private string username;
        private string password;

        [SetUp]
        public void Setup()
        {
            var configuration = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("AppConfig.json")
               .Build();

            baseUrl = configuration["UITestSettings:baseUrl"];
            username = configuration["UITestSettings:username"];
            password = configuration["UITestSettings:password"];

            driver = new ChromeDriver();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
        }

        [Test]
        public void UITest()
        {
            // Go to the website
            driver.Navigate().GoToUrl(baseUrl);

            // Authorization
            driver.FindElement(By.Id("user-name")).SendKeys(username);
            driver.FindElement(By.Id("password")).SendKeys(password);
            driver.FindElement(By.Id("login-button")).Click();

            // Filter products
            var filterDropdown = new SelectElement(driver.FindElement(By.ClassName("product_sort_container")));
            filterDropdown.SelectByText("Price (high to low)");

            // Search then parce name and price of the product
            var item = driver.FindElement(By.XPath("//div[text()='Sauce Labs Bike Light']/ancestor::div[@class='inventory_item']"));
            var itemName = item.FindElement(By.ClassName("inventory_item_name")).Text;
            var itemPrice = item.FindElement(By.ClassName("inventory_item_price")).Text;

            // Go to details section
            item.FindElement(By.ClassName("inventory_item_name")).Click();

            // Compare info from details to main page
            var detailName = driver.FindElement(By.ClassName("inventory_details_name")).Text;
            var detailPrice = driver.FindElement(By.ClassName("inventory_details_price")).Text;

            Assert.That(detailName, Is.EqualTo(itemName));
            Assert.That(detailPrice, Is.EqualTo(itemPrice));

            // Add product to the cart
            driver.FindElement(By.ClassName("btn_inventory")).Click();

            // Check cart counter
            var cartCount = driver.FindElement(By.ClassName("shopping_cart_badge")).Text;
            Assert.That(cartCount, Is.EqualTo("1"));

            // Go to cart and check name and price
            driver.FindElement(By.ClassName("shopping_cart_link")).Click();
            var cartItemName = driver.FindElement(By.ClassName("inventory_item_name")).Text;
            var cartItemPrice = driver.FindElement(By.ClassName("inventory_item_price")).Text;

            Assert.That(cartItemName, Is.EqualTo(itemName));
            Assert.That(cartItemPrice, Is.EqualTo(itemPrice));
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
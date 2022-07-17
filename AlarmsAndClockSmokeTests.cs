using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;

namespace MSTestOverview
{
    [TestClass]
    public class AlarmsAndClockSmokeTests
    {

        static WindowsDriver<WindowsElement> sessionAlarms;

        [ClassInitialize]
        public static void PrepareForTestingAlarms(TestContext testContext)
        {
            Debug.WriteLine("Hello ClassInitialize");

            AppiumOptions capCalc = new AppiumOptions();

            capCalc.AddAdditionalCapability("app", "Microsoft.WindowsAlarms_8wekyb3d8bbwe!App");

            sessionAlarms = new WindowsDriver<WindowsElement>(new Uri("http://127.0.0.1:4723"), capCalc);

        }

        [ClassCleanup]
        public static void CleanupAfterAllAlarmsTests()
        {
            Debug.WriteLine("Hello ClassCleanup");

            if (sessionAlarms != null)
            {
                sessionAlarms.Quit();
            }
        }

        [TestInitialize]
        public void BeforeATest()
        {
            Debug.WriteLine("Before a test, calling TestInitialize");
        }

        [TestCleanup]
        public void AfterATest()
        {
            Debug.WriteLine("After a test, calling TestCleanup");
        }

        [TestMethod]
        public void JustAnotherTest()
        {
            Debug.WriteLine("Hello another test.");
        }

        [TestMethod]
        public void TestAlarmsAndCLockIsLaunchingSuccessfully()
        {

            Debug.WriteLine("Hello TestAlarmsIsLaunchingSuccessfully!");

            Assert.AreEqual("Clock", sessionAlarms.Title, false,
                $"Actual title doesn't match expected title: {sessionAlarms.Title}");
        }

        [TestMethod]
        public void VerifyNewClockCanBeAdded()
        {
            // Click the clock button in top section of the app
            sessionAlarms.FindElementByAccessibilityId("ClockButton").Click();

            sessionAlarms.FindElementByName("Add a new city").Click();

            WebDriverWait waitForMe = new WebDriverWait(sessionAlarms, TimeSpan.FromSeconds(10));

            var txtLocation = sessionAlarms.FindElementByName("Enter a location");

            waitForMe.Until(pred => txtLocation.Displayed);

            txtLocation.SendKeys("Lahore, Pakistan");
            txtLocation.SendKeys(Keys.Enter);

            System.Threading.Thread.Sleep(3000); 

            var listClock  = sessionAlarms.FindElementByAccessibilityId("ClockDetailListView");

            var clockItems = listClock.FindElementsByTagName("ListItem");
            
            Debug.WriteLine($"Total tiles found: {clockItems.Count}");

            bool wasClockTileFound = false;
            WindowsElement tileFound = null;

            foreach (WindowsElement clockTile in clockItems)
            {
                if (clockTile.Text.StartsWith("Lahore, Pakistan"))
                {
                    wasClockTileFound = true;
                    tileFound = clockTile;
                    Debug.WriteLine("Clock found.");
                    break;
                }
            }

            Assert.IsTrue(wasClockTileFound, "No clock tile found.");

            waitForMe.Until(pred => tileFound.Displayed);

            Actions actionForRightClick = new Actions(sessionAlarms);
            actionForRightClick.MoveToElement(tileFound);
            actionForRightClick.Click();
            actionForRightClick.ContextClick(tileFound);
            actionForRightClick.Perform();

            AppiumOptions capDesktop = new AppiumOptions();
            capDesktop.AddAdditionalCapability("app", "Root");
            WindowsDriver<WindowsElement> sessionDesktop = new WindowsDriver<WindowsElement>(
                new Uri("http://127.0.0.1:4723"), capDesktop
                );

            var contextItemDelete = sessionDesktop.FindElementByAccessibilityId("ContextMenuDelete");

            WebDriverWait desktopWaitForMe = new WebDriverWait(sessionDesktop, TimeSpan.FromSeconds(10));

            desktopWaitForMe.Until(pred => contextItemDelete.Displayed);

            contextItemDelete.Click();

        }
    }
}

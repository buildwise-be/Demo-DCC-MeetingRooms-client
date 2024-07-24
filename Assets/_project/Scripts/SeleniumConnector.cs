using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System.IO;
using System;

public class SeleniumConnector : MonoBehaviour
{
    IWebDriver driver;
    [SerializeField]
    private string url = "https://t1b.gobright.cloud/portal/#/loginDisplay/117525992268228746302400058579894172338389585";

    private string PATH = @"C:\Unity\[TEST]PiXYZ\Assets\Packages\Selenium.WebDriver.MSEdgeDriver.125.0.2535.79\driver\win64\msedgedriver.exe";

    void Start()
    {
        // Set up Edge options
        Console.WriteLine("Setting Options");
        EdgeOptions edgeOptions = new EdgeOptions();
        edgeOptions.AddArgument("--headless");

        // Initialize the Edge WebDriver
        Console.WriteLine("Initialize the Edge WebDriver");
        EdgeDriverService service = EdgeDriverService.CreateDefaultService(PATH);
        driver = new EdgeDriver(service, edgeOptions);

    }

    public void GetRoomsUsage()
    {
        StartCoroutine(GetRoomsUsageCoroutine());
    }

    private IEnumerator GetRoomsUsageCoroutine()
    {
        driver.Navigate().GoToUrl(url);

        yield return new WaitForSeconds(5);

        IReadOnlyList<IWebElement> search = driver.FindElements(By.ClassName("ng-binding"));

        // Collect meeting room names
        List<string> meetingRooms = new List<string>();
        foreach (IWebElement element in search)
        {
            meetingRooms.Add(element.Text);
        }

        // Print the meeting rooms
        foreach (string room in meetingRooms)
        {
            Console.WriteLine(room);
        }
    }

    void OnDisable()
    {
        Console.WriteLine("Quit");
        driver.Quit();
    }
}
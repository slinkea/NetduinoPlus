using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TODO: Add PCMonitorCloud.dll as a reference to your project
using MM.Monitor.Cloud;
using Netduino.Communication;

namespace MM.Monitor.Plugins
{
    public class Cloud
    {
        private int numberOfNotificationsSent;
        private int detailsInvocationCount;

        private const int COMMAND_SEND_TEST_NOTIFICATION = 1;
        private const int COMMAND_CLEAR_NOTIFICATIONS_COUNT = 2;
        private const int COMMAND_CLEAR_INVOCATIONS_COUNT = 3;
        private const int COMMAND_STOP_INSTANCE_MONITORING = 4;
        private const int COMMAND_SEND_OPEN_WINDOW_SHADE = 5;
        private const int COMMAND_SEND_CLOSE_WINDOW_SHADE = 6;
        private const int COMMAND_SEND_STOP_WINDOW_SHADE = 7;
        private const int COMMAND_SEND_SPRINKLER_STOP = 8;
        private const int COMMAND_SEND_SPRINKLER_START_1 = 9;
        private const int COMMAND_SEND_SPRINKLER_START_2 = 10;
        private const int COMMAND_SEND_SPRINKLER_START_3 = 11;
        private const int COMMAND_SEND_SPRINKLER_START_4 = 12;
        private const int COMMAND_SEND_SPRINKLER_START_5 = 13;
        private const int COMMAND_SEND_SPRINKLER_START_6 = 14;
        private const int COMMAND_SEND_SPRINKLER_START_7 = 15;

        private const int PAGE_FIRST = 1;
        private const int PAGE_SECOND = 2;

        private CommunicateWithNetduino _ethernetCommunication;

        public string Temperature { get; set; }
        public string Humidity { get; set; }
        public string Shade { get; set; }

        public Cloud(CommunicateWithNetduino ethernetCommunication)
        {
            _ethernetCommunication = ethernetCommunication;

            // Subscribing to events
            MM.Monitor.Cloud.Service.Instance.OnDetailsRequest += new Service.DetailsRequestDelegate(OnDetailsRequest);
            MM.Monitor.Cloud.Service.Instance.OnPageRequest += new Service.PageRequestDelegate(OnPageRequest);
            MM.Monitor.Cloud.Service.Instance.OnCommandReceived += new Service.CommandReceivedDelegate(OnCommandReceived);            
            MM.Monitor.Cloud.Service.Instance.OnPageCommandReceived += new Service.PageCommandReceivedDelegate(OnPageCommandReceived);
            MM.Monitor.Cloud.Service.Instance.OnExceptionOccurred += new Service.ExceptionOccurredDelegate(OnExceptionOccurred);
            
            // Configuring the cloud instance
            MM.Monitor.Cloud.Service.Instance.Configure("250 Laure-Conan", "Cloud", "Running on " + Environment.MachineName, true /*set to true if you want to be notified when instance is offline*/);

            string username = "rocca1976";
            string password = "0123456789";
            string apiKey = "D7-C6-B1-F2-3A-7D-69-6C-36-B4-EE-1D-CE-D3-42-23-68-C0-66-FF-AA-E0-C6-51-05-5D-96-92-19-03-7D-2A-A4-B8-0D-D0-2F-CB-31-1E-C0-8F-9C-31-E2-0C-31-E8-76-FB-EB-46-5D-62-87-3B-81-7A-09-8D-41-8D-94-96"; // Email us at support@mobilepcmonitor.com with your PC Monitor username to receive your API Key

            // Starting the instance monitoring
            MM.Monitor.Cloud.Service.Instance.Start(username, password, apiKey);
            Console.ReadLine();
        }

        // Called when the instance details are requested by the mobile application
        private void OnDetailsRequest()
        {
            detailsInvocationCount++;

            Groups details = new Groups();

            Group group = new Group();
            string item = "Temperature: " + Temperature + "\nHumidity: " + Humidity;
            group.Items.Add(new SimpleItem("Basement", item));

            group.Items.Add(new PageItem(PAGE_FIRST, "Window Shade"));
            group.Items.Add(new PageItem(PAGE_SECOND, "Sprinklers"));

            details.Add(group);

            MM.Monitor.Cloud.Service.Instance.SetDetails(details);
        }

        // Called when the a page is requested by the mobile application
        private void OnPageRequest(int pageId, string mobileDeviceIdentifier)
        {
            Groups details = new Groups();

            if (pageId == PAGE_FIRST)
            {
                Group group = new Group("Window Shade");
                group.Items.Add(new SimpleItem("State: ", Shade));
                group.Items.Add(new CommandItem(COMMAND_SEND_OPEN_WINDOW_SHADE, "Open"));
                group.Items.Add(new CommandItem(COMMAND_SEND_STOP_WINDOW_SHADE, "Stop"));
                group.Items.Add(new CommandItem(COMMAND_SEND_CLOSE_WINDOW_SHADE, "Close"));
                details.Add(group);
            }
            else if (pageId == PAGE_SECOND)
            {
                Group group = new Group("Sprinklers");
                group.Items.Add(new SimpleItem("State: "));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_STOP, "Stop"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_1, "Start Zone 1"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_2, "Start Zone 2"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_3, "Start Zone 3"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_4, "Start Zone 4"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_5, "Start Zone 5"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_6, "Start Zone 6"));
                group.Items.Add(new CommandItem(COMMAND_SEND_SPRINKLER_START_7, "Start Zone 7"));
                details.Add(group);
            }

            MM.Monitor.Cloud.Service.Instance.SetPageDetails(pageId, details);
        }

        // Called when a command is executed on the mobile application
        private void OnCommandReceived(int commandId, string mobileDeviceIdentifier)
        {
            if(commandId == COMMAND_SEND_TEST_NOTIFICATION)
            {
                //numberOfNotificationsSent++;
                // Send a notification to all registered devices on the account
                MM.Monitor.Cloud.Service.Instance.SendNotificationToAllDevices("This is a test notification from the Cloud API", NotificationPriority.NORMAL);
            }
            else if (commandId == COMMAND_CLEAR_NOTIFICATIONS_COUNT)
            {
                numberOfNotificationsSent = 0;                
            }
            else if (commandId == COMMAND_CLEAR_INVOCATIONS_COUNT)
            {
                detailsInvocationCount = 0;
            }
            else if (commandId == COMMAND_STOP_INSTANCE_MONITORING)
            {
                // Stop the instance monitoring
                MM.Monitor.Cloud.Service.Instance.Stop();
            }
            else if (commandId == COMMAND_SEND_OPEN_WINDOW_SHADE)
            {
                _ethernetCommunication.SendMessage("OPEN_WINDOW_SHADE");
            }
            else if (commandId == COMMAND_SEND_CLOSE_WINDOW_SHADE)
            {
                _ethernetCommunication.SendMessage("CLOSE_WINDOW_SHADE");
            }
            else if (commandId == COMMAND_SEND_STOP_WINDOW_SHADE)
            {
                _ethernetCommunication.SendMessage("STOP_WINDOW_SHADE");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_STOP)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_STOP");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_1)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_1");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_2)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_2");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_3)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_3");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_4)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_4");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_5)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_5");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_6)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_6");
            }
            else if (commandId == COMMAND_SEND_SPRINKLER_START_7)
            {
                _ethernetCommunication.SendMessage("SPRINKLER_START_7");
            }
            
        }

        // Called when a command is executed from a page on the mobile application
        private void OnPageCommandReceived(int pageId, int commandId, string mobileDeviceIdentifier)
        {
            //if (commandId == COMMAND_SEND_TEST_NOTIFICATION)
            //{
                //numberOfNotificationsSent++;
                // Send a notification to all registered devices on the account

                //if (pageId == PAGE_FIRST)
                //{
                    //MM.Monitor.Cloud.Service.Instance.SendNotificationToAllDevices("This is a test notification from the 1st Page Cloud API", NotificationPriority.CRITICAL);
                //}
                //else if (pageId == PAGE_SECOND)
                //{
                    //MM.Monitor.Cloud.Service.Instance.SendNotificationToAllDevices("This is another notification from the 2nd Page Cloud API", NotificationPriority.CRITICAL);
                //}
            //}
        }

        // Called when an exception occurred in the Cloud API
        private void OnExceptionOccurred(Exception ex)
        {            
            // Get more info about the error
        }        
    }
}

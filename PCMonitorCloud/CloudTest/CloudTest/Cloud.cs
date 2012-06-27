using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TODO: Add PCMonitorCloud.dll as a reference to your project
using MM.Monitor.Cloud;

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

        private const int PAGE_FIRST = 1;
        private const int PAGE_SECOND = 2;

        public Cloud()
        {
            // Subscribing to events
            MM.Monitor.Cloud.Service.Instance.OnDetailsRequest += new Service.DetailsRequestDelegate(OnDetailsRequest);
            MM.Monitor.Cloud.Service.Instance.OnPageRequest += new Service.PageRequestDelegate(OnPageRequest);
            MM.Monitor.Cloud.Service.Instance.OnCommandReceived += new Service.CommandReceivedDelegate(OnCommandReceived);            
            MM.Monitor.Cloud.Service.Instance.OnPageCommandReceived += new Service.PageCommandReceivedDelegate(OnPageCommandReceived);
            MM.Monitor.Cloud.Service.Instance.OnExceptionOccurred += new Service.ExceptionOccurredDelegate(OnExceptionOccurred);
            
            // Configuring the cloud instance
            MM.Monitor.Cloud.Service.Instance.Configure("Test Cloud", "Cloud", "Running on " + Environment.MachineName, false /*set to true if you want to be notified when instance is offline*/);

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

            Group firstGroup = new Group("Values");            
            firstGroup.Items.Add(new SimpleItem(numberOfNotificationsSent.ToString(), "Notifications Sent"));
            firstGroup.Items.Add(new SimpleItem(detailsInvocationCount.ToString(), "Details Request Invocation Count"));
            details.Add(firstGroup);

            Group secondGroup = new Group("Commands");
            secondGroup.Items.Add(new CommandItem(COMMAND_SEND_TEST_NOTIFICATION, "Send a test notification"));
            secondGroup.Items.Add(new CommandItem(COMMAND_CLEAR_NOTIFICATIONS_COUNT, "Clear Notifications Count"));
            secondGroup.Items.Add(new CommandItem(COMMAND_CLEAR_INVOCATIONS_COUNT, "Clear Details Request Count"));
            secondGroup.Items.Add(new CommandItem(COMMAND_STOP_INSTANCE_MONITORING, "Stop", "Stop Monitoring this Instance"));
            details.Add(secondGroup);

            Group thirdGroup = new Group("Pages");
            thirdGroup.Items.Add(new PageItem(PAGE_FIRST, "Page 1", "This is a page"));            
            details.Add(thirdGroup);

            MM.Monitor.Cloud.Service.Instance.SetDetails(details);
        }

        // Called when the a page is requested by the mobile application
        private void OnPageRequest(int pageId, string mobileDeviceIdentifier)
        {
            Groups details = new Groups();

            if (pageId == PAGE_FIRST)
            {
                Group firstGroup = new Group("First Page Items");
                firstGroup.Items.Add(new SimpleItem("1", "Simple Value"));
                firstGroup.Items.Add(new CommandItem(COMMAND_SEND_TEST_NOTIFICATION, "Send a test notification"));
                firstGroup.Items.Add(new PageItem(PAGE_SECOND, "Page 2", "This is another page"));
                details.Add(firstGroup);
            }
            else if (pageId == PAGE_SECOND)
            {
                Group firstGroup = new Group("Second Page Items");
                firstGroup.Items.Add(new SimpleItem("2", "Simple Value"));
                firstGroup.Items.Add(new CommandItem(COMMAND_SEND_TEST_NOTIFICATION, "Send a test notification"));
                details.Add(firstGroup);
            }

            MM.Monitor.Cloud.Service.Instance.SetPageDetails(pageId, details);
        }

        // Called when a command is executed on the mobile application
        private void OnCommandReceived(int commandId, string mobileDeviceIdentifier)
        {
            if(commandId == COMMAND_SEND_TEST_NOTIFICATION)
            {
                numberOfNotificationsSent++;
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
        }

        // Called when a command is executed from a page on the mobile application
        private void OnPageCommandReceived(int pageId, int commandId, string mobileDeviceIdentifier)
        {
            if (commandId == COMMAND_SEND_TEST_NOTIFICATION)
            {
                numberOfNotificationsSent++;
                // Send a notification to all registered devices on the account

                if (pageId == PAGE_FIRST)
                {
                    MM.Monitor.Cloud.Service.Instance.SendNotificationToAllDevices("This is a test notification from the 1st Page Cloud API", NotificationPriority.CRITICAL);
                }
                else if (pageId == PAGE_SECOND)
                {
                    MM.Monitor.Cloud.Service.Instance.SendNotificationToAllDevices("This is another notification from the 2nd Page Cloud API", NotificationPriority.CRITICAL);
                }
            }
        }

        // Called when an exception occurred in the Cloud API
        private void OnExceptionOccurred(Exception ex)
        {            
            // Get more info about the error
        }        
    }
}

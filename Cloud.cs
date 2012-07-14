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

        private const string TEXT_INPUT_ID = "StringInput";
        private const string NUMERIC_INPUT_ID = "NumericInput";
        private const string DATE_INPUT_ID = "DateInput";
        private const string TIME_INPUT_ID = "TimeInput";
        private const string DATETIME_INPUT_ID = "DateTimeInput";
        private const string PICKLIST_INPUT_ID = "PickListInput";

        private string stringInputValue = "string value";
        private int numericInputValue = 24;
        private Date dateInputValue = new Date(2010, 10, 4);
        private Time timeInputValue = new Time(10, 21);
        private MM.Monitor.Cloud.DateTime dateTimeInputValue = new MM.Monitor.Cloud.DateTime(2011, 10, 11, 12, 13);
        private PickListItem pickListInputValue = pickListItem2;

        private static PickListItem pickListItem1 = new PickListItem(1, "Item 1", "First item");
        private static PickListItem pickListItem2 = new PickListItem(2, "Item 2", "Second item");
        private static PickListItem pickListItem3 = new PickListItem(3, "Item 3", "Third item");

        private static PickListItem[] AllPickListItems = new PickListItem[] { pickListItem1, pickListItem2, pickListItem3 };

        public Cloud()
        {
            // Subscribing to events
            MM.Monitor.Cloud.Service.Instance.OnDetailsRequest += new Service.DetailsRequestDelegate(OnDetailsRequest);
            MM.Monitor.Cloud.Service.Instance.OnPageRequest += new Service.PageRequestDelegate(OnPageRequest);
            MM.Monitor.Cloud.Service.Instance.OnCommandReceived += new Service.CommandReceivedDelegate(OnCommandReceived);            
            MM.Monitor.Cloud.Service.Instance.OnPageCommandReceived += new Service.PageCommandReceivedDelegate(OnPageCommandReceived);
            MM.Monitor.Cloud.Service.Instance.OnExceptionOccurred += new Service.ExceptionOccurredDelegate(OnExceptionOccurred);

            MM.Monitor.Cloud.Service.Instance.OnTextInputValueChanged += new Service.TextInputValueChangedDelegate(Instance_OnTextInputValueChanged);
            MM.Monitor.Cloud.Service.Instance.OnNumericInputValueChanged += new Service.NumericInputValueChangedDelegate(Instance_OnNumericInputValueChanged);
            MM.Monitor.Cloud.Service.Instance.OnDateInputValueChanged += new Service.DateInputValueChangedDelegate(Instance_OnDateInputValueChanged);
            MM.Monitor.Cloud.Service.Instance.OnTimeInputValueChanged += new Service.TimeInputValueChangedDelegate(Instance_OnTimeInputValueChanged);
            MM.Monitor.Cloud.Service.Instance.OnDateTimeInputValueChanged += new Service.DateTimeInputValueChangedDelegate(Instance_OnDateTimeInputValueChanged);
            MM.Monitor.Cloud.Service.Instance.OnPickListInputValueChanged += new Service.PickListInputValueChangedDelegate(Instance_OnPickListInputValueChanged);
            
            // Configuring the cloud instance
            MM.Monitor.Cloud.Service.Instance.Configure("Test Cloud", "Cloud", "Running on " + Environment.MachineName, false /*set to true if you want to be notified when instance is offline*/);

            string username = "Your PC Monitor username";
            string password = "Your PC Monitor password";
            string apiKey   = "Your PC Monitor API Key"; // Email us at support@mobilepcmonitor.com with your PC Monitor username to receive your API Key

            // Starting the instance monitoring
            MM.Monitor.Cloud.Service.Instance.Start(username, password, apiKey);                        
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

            Group forthGroup = new Group("Input Controls");
            forthGroup.Items.Add(new TextInputItem(TEXT_INPUT_ID, "String", "Current value: " + stringInputValue, stringInputValue));
            forthGroup.Items.Add(new NumberInputItem(NUMERIC_INPUT_ID, "Number", "Current value: " + numericInputValue.ToString(), numericInputValue));
            forthGroup.Items.Add(new DateInputItem(DATE_INPUT_ID, "Date", "Current value: " + GetDateString(dateInputValue), dateInputValue));
            forthGroup.Items.Add(new TimeInputItem(TIME_INPUT_ID, "Time", "Current value: " + GetTimeString(timeInputValue), timeInputValue));
            forthGroup.Items.Add(new DateTimeInputItem(DATETIME_INPUT_ID, "Date and Time", "Current value: " + GetDateTimeString(dateTimeInputValue), dateTimeInputValue));

            PickListItems items = new PickListItems();
            items.Add(pickListItem1);
            items.Add(pickListItem2);
            items.Add(pickListItem3);
            forthGroup.Items.Add(new PickListInputItem(PICKLIST_INPUT_ID, "Pick", "Current value: " + pickListInputValue.Name, items, pickListInputValue));

            details.Add(forthGroup);

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

        // Called when a string input control has changed
        void Instance_OnTextInputValueChanged(string inputId, string inputValue)
        {
            if (inputId == TEXT_INPUT_ID)
            {
                stringInputValue = inputValue;
            }
        }
        // Called when a numeric input control has changed
        void Instance_OnNumericInputValueChanged(string inputId, int inputValue)
        {
            if (inputId == NUMERIC_INPUT_ID)
            {
                numericInputValue = inputValue;
            }
        }
        // Called when a date input control has changed        
        void Instance_OnDateInputValueChanged(string inputId, Date inputValue)
        {
            if (inputId == DATE_INPUT_ID)
            {
                dateInputValue = inputValue;
            }
        }
        // Called when a time input control has changed        
        void Instance_OnTimeInputValueChanged(string inputId, Time inputValue)
        {
            if (inputId == TIME_INPUT_ID)
            {
                timeInputValue = inputValue;
            }
        }
        // Called when a date and time input control has changed        
        void Instance_OnDateTimeInputValueChanged(string inputId, Monitor.Cloud.DateTime inputValue)
        {
            if (inputId == DATETIME_INPUT_ID)
            {
                dateTimeInputValue = inputValue;
            }
        }                
        // Called when a pick list input control has changed        
        void Instance_OnPickListInputValueChanged(string inputId, int pickListItemId)
        {
            if (inputId == PICKLIST_INPUT_ID)
            {
                foreach (PickListItem item in AllPickListItems)
                {
                    if (pickListItemId == item.Id)
                    {
                        pickListInputValue = item;
                        break;
                    }
                }
            }
        }        

        private static string GetDateString(Date dateInputValue)
        {
            return dateInputValue.Day.ToString() + "-" + dateInputValue.Month.ToString() + "-" + dateInputValue.Year.ToString();
        }
        private static string GetTimeString(Time timeInputValue)
        {
            return timeInputValue.Hour.ToString() + ":" + timeInputValue.Minute.ToString("00");
        }
        private static string GetDateTimeString(MM.Monitor.Cloud.DateTime dateTimeInputValue)
        {
            return dateTimeInputValue.Day.ToString() + "-" + dateTimeInputValue.Month.ToString() + "-" + dateTimeInputValue.Year.ToString() + " " + dateTimeInputValue.Hour.ToString() + ":" + dateTimeInputValue.Minute.ToString("00");
        }
    }
}

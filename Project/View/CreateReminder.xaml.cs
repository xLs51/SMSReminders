using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Scheduler;
using Microsoft.Phone.Tasks;
using Project.Model;

namespace Project.View
{
    public partial class CreateReminder : PhoneApplicationPage
    {
        public CreateReminder()
        {
            InitializeComponent();
            PhoneNumberChooserTask = new PhoneNumberChooserTask();
            PhoneNumberChooserTask.Completed += phoneNumberChooserTask_Completed;
        }

        private PhoneNumberChooserTask PhoneNumberChooserTask { get; set; }
        private string NameBox { get; set; }
        private string NumberBox { get; set; }

        private void Create(object sender, EventArgs e)
        {
            var date = (DateTime) DatePicker.Value;
            var time = (DateTime) TimePicker.Value;
            DateTime beginTime = date + time.TimeOfDay;

            if (NumberTextBox.Text == "")
            {
                MessageBox.Show("Please enter a number phone or add a contact");
                return;
            }
            if (BodyTextBox.Text == "")
            {
                MessageBox.Show("Please enter a body message");
                return;
            }
            if (beginTime < DateTime.Now)
            {
                MessageBox.Show("The date must be in the future.");
                return;
            }

            var random = new Random(DateTime.Now.Millisecond);
            int randomNumber = random.Next(1, 500000);
            string alarmName = NumberTextBox.Text + randomNumber.ToString(CultureInfo.InvariantCulture);

            DateTime MyDateTime = ((DateTime) DatePicker.Value).Date.Add(((DateTime) TimePicker.Value).TimeOfDay);

            var sms = new Sms(BodyTextBox.Text, NumberTextBox.Text, NameTextBox.Text, MyDateTime, alarmName);
            SmsDb.SaveData(sms);

            var alarm = new Alarm(alarmName)
            {
                Content =
                    "Hey, you planned to send a sms \r\nto " + NumberTextBox.Text + " " + NameTextBox.Text + "\r\nat " +
                    MyDateTime,
                BeginTime = beginTime,
                ExpirationTime = beginTime,
                RecurrenceType = RecurrenceInterval.None
            };

            ScheduledActionService.Add(alarm);

            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void Cancel(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }

        private void Add_Contact(object sender, RoutedEventArgs e)
        {
            PhoneNumberChooserTask.Show();
        }

        private void phoneNumberChooserTask_Completed(object sender, PhoneNumberResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                NameTextBox.Text = e.DisplayName;
                NumberTextBox.Text = e.PhoneNumber;
                NameBox = e.DisplayName;
                NumberBox = e.PhoneNumber;

                AddButton.IsEnabled = false;
            }
        }

        private void NumberTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            AddButton.IsEnabled = NumberTextBox.Text == "";
            NameTextBox.Text = "";

            if (NameBox != null && NumberBox == NumberTextBox.Text)
                NameTextBox.Text = NameBox;
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);
            NavigationService.Navigate(new Uri("/View/MainPage.xaml", UriKind.Relative));
        }
    }
}
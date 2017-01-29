using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Project.Model;

namespace Project.View
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            Messenger.Default.Register<Sms>(this, ShowSms);

            List<Sms> load = SmsDb.LoadData();

            ListReminders.ItemsSource = load;
            int count = load.Count;
            Sms sms = load.FirstOrDefault();

            if (sms == null) return;

            ShellTile tuileParDefaut = ShellTile.ActiveTiles.First();

            if (tuileParDefaut != null)
            {
                var flipTileData = new FlipTileData
                {
                    Title = "SmsReminders",
                    Count = count,
                    BackTitle = "Next reminder",
                    BackContent = sms.Number + "\r\nat " + sms.Date,
                };

                tuileParDefaut.Update(flipTileData);
            }
        }

        private void ShowSms(Sms sms)
        {
            if (sms == null) return;
            NavigationService.Navigate(new Uri("/View/ShowReminder.xaml?id=" + sms.Id, UriKind.Relative));
        }

        private void AddSecondaryTile(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement) sender;
            var sms = element.DataContext as Sms;
            if (sms == null) return;

            int id = sms.Id;
            ShellTile mySms = ShellTile.ActiveTiles.FirstOrDefault(s => s.NavigationUri.ToString().Contains("id=" + id));

            if (mySms == null)
            {
                var tile = new FlipTileData
                {
                    Title = "SmsReminders",
                    BackTitle = sms.Number + " - " + sms.Date,
                    BackContent = sms.Body,
                    SmallBackgroundImage = new Uri("/Assets/AppIcon.png", UriKind.Relative),
                    BackgroundImage = new Uri("/Assets/AppIcon.png", UriKind.Relative),
                    WideBackgroundImage = new Uri("/Assets/LargeTileAppIcon.png", UriKind.Relative),
                };

                ShellTile.Create(new Uri("/View/ShowReminder.xaml?id=" + id, UriKind.Relative), tile, false);
            }
        }

        protected override void OnBackKeyPress(CancelEventArgs e)
        {
            base.OnBackKeyPress(e);

            while (NavigationService.BackStack.Any())
                NavigationService.RemoveBackEntry();
        }
    }
}
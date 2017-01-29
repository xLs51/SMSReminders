using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Phone.Controls;
using Project.Model;

namespace Project.ViewModel
{
    /// <summary>
    ///     This class contains properties that a View can data bind to.
    ///     <para>
    ///         See http://www.galasoft.ch/mvvm
    ///     </para>
    /// </summary>
    public class ListSmsViewModel : ViewModelBase
    {
        /// <summary>
        ///     The <see cref="ListSms" /> property's name.
        /// </summary>
        public const string ListSmsPropertyName = "ListSms";

        /// <summary>
        ///     The <see cref="Selection" /> property's name.
        /// </summary>
        public const string SelectionPropertyName = "Selection";

        private ISmsService _smsService;
        private ObservableCollection<SmsBinding> _listSms;
        private Sms _selection;

        public ICommand SelectionElementCommand { get; set; }
        public ICommand AddReminder { get; set; }
        public ICommand DeleteAllReminders { get; set; }


        /// <summary>
        ///     Initializes a new instance of the ListSmsViewModel class.
        /// </summary>
        public ListSmsViewModel(ISmsService smsService)
        {
            _smsService = smsService;
            _smsService.GetAllData(
                (item, error) =>
                {
                    if (error != null)
                        return;

                    if (item == null) return;

                    var myList = item.Select(s => new SmsBinding { Id = s.Id, Body = s.Body, Number = s.Number, Name = s.Name, Date = s.Date, AlarmName = s.AlarmName, ViewModel = this }).ToList();
                    var mySms = new ObservableCollection<SmsBinding>(myList);
                    ListSms = mySms;
                });

            SelectionElementCommand = new RelayCommand<SelectionChangedEventArgs>(OnSelectionElement);
            AddReminder = new RelayCommand(Add);
            DeleteAllReminders = new RelayCommand(DeleteAll);
        }

        /// <summary>
        ///     Gets the ListSms property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public ObservableCollection<SmsBinding> ListSms
        {
            get { return _listSms; }
            set
            {
                if (_listSms == value)
                {
                    return;
                }

                _listSms = value;
                RaisePropertyChanged(ListSmsPropertyName);
            }
        }

        /// <summary>
        ///     Gets the Selection property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public Sms Selection
        {
            get { return _selection; }
            set
            {
                if (_selection == value)
                {
                    return;
                }

                _selection = value;
                RaisePropertyChanged(SelectionPropertyName);
            }
        }

        private void OnSelectionElement(SelectionChangedEventArgs args)
        {
	        Messenger.Default.Send(Selection);
	        Selection = null;
        }

        private static void Add()
        {
            (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/View/CreateReminder.xaml", UriKind.Relative));;
        }

        private static void DeleteAll()
        {
            if (MessageBox.Show("Do you want to delete to all reminders ?", "Delete", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
                SmsDb.DeleteAll();
                (Application.Current.RootVisual as PhoneApplicationFrame).Navigate(new Uri("/View/MainPage.xaml?" + DateTime.Now.Ticks, UriKind.Relative));
            }
        }
    }
}
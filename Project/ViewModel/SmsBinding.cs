using System;

namespace Project.ViewModel
{
    public class SmsBinding
    {
        public int Id { get; set; }
        public string Body { get; set; }
        public string Number { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string AlarmName { get; set; }
        public ListSmsViewModel ViewModel { get; set; }
    }
}
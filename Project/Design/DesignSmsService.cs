using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Project.Model;

namespace Project.Design
{
    public class DesignSmsService : ISmsService
    {
        public void GetAllData(Action<ObservableCollection<Sms>, Exception> callback)
        {
            var body = new List<String>
            {
                "Happy Birthday",
                "Happy Mother's Day",
                "Happy Father's Day",
            };

            var number = new List<String>
            {
                "0606060606",
                "0606060606",
                "0606060606",
            };

            var name = new List<String>
            {
                "Friend",
                "Dad",
                "Mom",
            };

            var date = new List<DateTime>
            {
                new DateTime(2014, 9, 12, 12, 24, 0),
                new DateTime(2014, 9, 12, 12, 24, 0),
                new DateTime(2014, 9, 12, 12, 24, 0),
            };

            List<string> alarmName = (from n in number
                let random = new Random(DateTime.Now.Millisecond)
                let randomNumber = random.Next(1, 500000)
                select n + randomNumber.ToString(CultureInfo.InvariantCulture)).ToList();

            var item = new ObservableCollection<Sms>();

            for (int i = 0; i < body.Count; i++)
            {
                item.Add(new Sms(body[i], number[i], name[i], date[i], alarmName[i]));
            }

            callback(item, null);
        }
    }
}
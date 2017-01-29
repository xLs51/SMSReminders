using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq;
using Microsoft.Phone.Scheduler;

namespace Project.Model
{
    [Table]
    public class Sms
    {
        public Sms()
        {
        }

        public Sms(string b, string n, string name, DateTime d, string an)
        {
            Body = b;
            Number = n;
            Name = name;
            Date = d;
            AlarmName = an;
        }

        public Sms(int i, string b, string n, string name, DateTime d, string an)
        {
            Id = i;
            Body = b;
            Number = n;
            Name = name;
            Date = d;
            AlarmName = an;
        }

        [Column(IsPrimaryKey = true, IsDbGenerated = true)]
        public int Id { get; set; }

        [Column]
        public string Body { get; set; }

        [Column]
        public string Number { get; set; }

        [Column]
        public string Name { get; set; }

        [Column]
        public DateTime Date { get; set; }

        [Column]
        public string AlarmName { get; set; }
    }

    public interface ISmsService
    {
        void GetAllData(Action<ObservableCollection<Sms>, Exception> callback);
    }

    public class SmsService : ISmsService
    {
        public void GetAllData(Action<ObservableCollection<Sms>, Exception> callback)
        {
            var item = new ObservableCollection<Sms>(SmsDb.LoadData());
            callback(item, null);
        }
    }

    public class HrContext : DataContext
    {
        public Table<Sms> Sms;

        public HrContext(string strConnection) : base(strConnection)
        {
            if (false == DatabaseExists())
                CreateDatabase();
        }
    }

    public class SmsDb
    {
        public static void SaveData(Sms sms)
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                db.Sms.InsertOnSubmit(sms);
                db.SubmitChanges();
            }
        }

        public static List<Sms> LoadData()
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                if (db.DatabaseExists() == false)
                {
                    db.CreateDatabase();
                    db.SubmitChanges();
                }

                Table<Sms> sms = db.GetTable<Sms>();

                try
                {
                    IEnumerator<Sms> enumEntity = sms.GetEnumerator();
                }
                catch (Exception)
                {
                    Debug.WriteLine("Got exception while asking for table enumerator. Table probably doesn't exist...");

                    db.DeleteDatabase();
                    db.CreateDatabase();
                    db.SubmitChanges();

                    Debug.WriteLine("Recreated database.");
                }

                IOrderedQueryable<Sms> listSms = (from s in db.Sms select s).OrderBy(s => s.Date);

                return listSms.ToList();
            }
        }

        public static Sms LoadSms(int id)
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                Sms sms = db.Sms.FirstOrDefault(s => s.Id == id);
                return sms;
            }
        }

        public static void DeleteAll()
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                IQueryable<Sms> sms = from s in db.Sms select s;

                foreach (Sms alarm in sms)
                    ScheduledActionService.Remove(alarm.AlarmName);

                db.Sms.DeleteAllOnSubmit(sms);
                db.SubmitChanges();
            }
        }

        public static void Delete(int id)
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                Sms sms = db.Sms.FirstOrDefault(s => s.Id == id);
                ScheduledActionService.Remove(sms.AlarmName);

                db.Sms.DeleteOnSubmit(sms);
                db.SubmitChanges();
            }
        }

        public static void Update(Sms sms)
        {
            const string sConn = "Data Source=isostore:/HRDatabase.sdf";

            using (var db = new HrContext(sConn))
            {
                Sms updateSms = db.Sms.FirstOrDefault(s => s.Id == sms.Id);
                ScheduledActionService.Remove(updateSms.AlarmName);

                updateSms.AlarmName = sms.AlarmName;
                updateSms.Body = sms.Body;
                updateSms.Date = sms.Date;
                updateSms.Name = sms.Name;
                updateSms.Number = sms.Number;

                db.SubmitChanges();
            }
        }
    }
}
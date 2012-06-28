using System;
using System.Threading;
using System.Collections;
using Microsoft.SPOT;

namespace Netduino.Controller
{
    public delegate void AlarmCallback();

    class AlarmData
    {
        public int Key { get; set; }
        public ExtendedTimer ExtendedTimer { get; set; }
        public bool RemoveAfterRun { get; set; }
        public AlarmCallback Callback { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    class Time
    {
        #region Private Variables
        private static Hashtable _alarmHashtable;
        private static int _key;
        #endregion

        #region Constructors
        //This keeps other classes from creating an instance
        private Time()
        {
        }
        #endregion


        #region Public Static Methods

        public static void SetTime(int year, int month, int day, int hour,int minute, int second, int millisecond )
        {
            DateTime presentTime = new DateTime( year, month, day, hour, minute, second, millisecond);
            Microsoft.SPOT.Hardware.Utility.SetLocalTime(presentTime);
        }

        public static void RunDaily(AlarmCallback alarmCallback, int hour, int minute, int second)
        {
            DateTime alarmTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second, 0);
            
            //If we already missed today then tomorrow is the first day to run
            if(alarmTime<DateTime.Now)
            {
                alarmTime = alarmTime.AddDays(1);
            }

            TimeSpan dailyTimeSpan = new TimeSpan(24, 0, 0);
            CreateAlarm(alarmCallback, alarmTime, dailyTimeSpan, false, new ArrayList());
        }

        public static int RunOnDelay(AlarmCallback alarmCallback, int runInMilliseconds)
        {
            DateTime alarmTime = DateTime.Now.AddMilliseconds(runInMilliseconds);
            return CreateAlarm(alarmCallback, alarmTime, TimeSpan.Zero, true, new ArrayList());
        }

        public static void RunRepetitively(AlarmCallback alarmCallback, int repeatMilliseconds)
        {
            DateTime alarmTime = DateTime.Now.AddMilliseconds(repeatMilliseconds);
            TimeSpan repeatTimeSpan = new TimeSpan(0, 0, 0, 0, repeatMilliseconds);
            CreateAlarm(alarmCallback, alarmTime, repeatTimeSpan, false, new ArrayList());
        }

        public static bool GetInfo(int key, int elapsedMilliseconds, int remainingMilliseconds)
        {
            bool result = false;

            AlarmData alarmData = GetAlarmData(key);

            if (alarmData != null)
            {
                elapsedMilliseconds = DateTime.Now.Millisecond - alarmData.StartTime.Millisecond;

                remainingMilliseconds = alarmData.EndTime.Millisecond - DateTime.Now.Millisecond;

                result = true;
            }

            return result;
        }

        public static bool Remove(int key)
        {
            bool result = false;

            AlarmData alarmData = GetAlarmData(key);

            if (alarmData != null)
            {
                alarmData.ExtendedTimer.Dispose();

                _alarmHashtable.Remove(key);

                result = true;
            }

            return result;
        }

        private static AlarmData GetAlarmData(int key)
        {
            AlarmData alarmData = null;

            if (_alarmHashtable != null && _alarmHashtable.Contains(key))
            {
                alarmData = (AlarmData)_alarmHashtable[key];
            }

            return alarmData;
        }


        #endregion

        #region Private Methods
        private static int CreateAlarm(AlarmCallback alarmCallback, DateTime alarmTime, TimeSpan timeSpan, bool removeAfterRun, ArrayList skipDay)
        {
            if (_alarmHashtable == null)
            {
                _alarmHashtable = new Hashtable();
            }

            _key=_key+1;

            AlarmData alarmData = new AlarmData();
            alarmData.Key = _key;
            alarmData.Callback = alarmCallback;
            alarmData.ExtendedTimer = new ExtendedTimer(OnExecuteAlarm, alarmData, alarmTime, timeSpan);
            alarmData.RemoveAfterRun = removeAfterRun;
            alarmData.StartTime = DateTime.Now;
            alarmData.EndTime = alarmTime;

            _alarmHashtable.Add(_key, alarmData);

            return _key;
        }

        private static void OnExecuteAlarm(object target)
        {
            AlarmData alarmData = (AlarmData)target;

            if (alarmData.RemoveAfterRun)
            {
                _alarmHashtable.Remove(alarmData.Key);
            }

            alarmData.Callback.Invoke();
        }
        #endregion
    }
}

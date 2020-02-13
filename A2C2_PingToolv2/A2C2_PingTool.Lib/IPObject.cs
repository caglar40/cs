using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A2C2_PingTool.Lib
{
    public class IPObject : INotifyPropertyChanged
    {
        #region fields;
        int _id;
        IPAddress _ip;
        int _success;
        int _failed;
        string _time = string.Empty;

        Ping _ping;
        PingReply _reply;
        bool _exitPing;
        bool _startOneThreadForTimer = true;
        bool _stoptimer = false;

        DateTime _starttime;
        TimeSpan _span = TimeSpan.Zero;
        string _informFailedPing = string.Empty;
        #endregion


        public IPObject()
        {
            _ping = new Ping();
        }

        #region Properties
        public int Id { get => _id; set => _id = value; }
        public IPAddress Ip { get => _ip; set { _ip = value; } }
        public int Success { get => _success; set { _success = value; OnPropertyChanged("succeess"); } }
        public int Failed { get => _failed; set { _failed = value; OnPropertyChanged("failed"); } }
        public string Time { get => _time; set { _time = value; OnPropertyChanged("time"); } }
        public string ResultTime { get => _informFailedPing; set => _informFailedPing = value; }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public void SendPing()
        {
            int _countcase = 0;
            int _countFailedOfCase = 0;
            while (_exitPing == false)
            {
                _reply = _ping.Send(_ip);
                if (_reply.Status.ToString() == "Success")
                {
                    Success++;
                    if (_span != TimeSpan.Zero)
                    {
                        _stoptimer = true;
                        _startOneThreadForTimer = true;
                        _countcase++;
                        TimetoLog(_countcase,_countFailedOfCase);
                    }
                    _span = TimeSpan.Zero;
                    Time = string.Empty;
                    _countFailedOfCase = 0;
                }
                else
                {
                    _countFailedOfCase++;
                    Failed++;
                    _stoptimer = false;
                    if (_startOneThreadForTimer == true)
                    {
                        _starttime = DateTime.Now;
                        Thread th = new Thread(Timerstart);
                        th.IsBackground = true;
                        th.Name = "Start Timer";
                        th.Start();
                        _startOneThreadForTimer=false;
                    }
                }
                Thread.Sleep(1000);
            }
        }

        private void TimetoLog(int countcase,int countfailedofcase)
        {
            ResultTime = "<" + DateTime.Now.ToString() + ">  |  " + "IP:" + _ip.ToString() + "  |  " + "Case#" + countcase + "   |   " + "Duration: " + Time.ToString() + "   |   " + "total failed: " + countfailedofcase.ToString();
        }

        public void Timerstart()
        {
            while (_stoptimer == false)
            {
                _span = DateTime.Now.Subtract(_starttime);
                 Time = _span.ToString("mm\\:ss\\:fff");
            }
        }

        public void PingAction(string value)
        {
            if (value == "stop")
            {
                _exitPing = true;
                _stoptimer = true;
            }
            else
            {
                _exitPing = false;
                _stoptimer = false;
                _span = TimeSpan.Zero;
                Time = string.Empty;
                _startOneThreadForTimer = true;
                Success = 0;
                Failed = 0;
            }
        }
    }
}

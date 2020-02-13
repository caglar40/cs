using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace A2C2_PingTool.Lib
{
    public class Manager
    {
        private static Manager _instance = null;
        BindingList<IPObject> _ipList;
        int id = 1;
        List<string> _listofFailedPings;
        bool _exitloop = false;
        ExportService _exs;

        public Manager()
        {
            _listofFailedPings = new List<string>();
        }

        public static Manager GetInstance()
        {
            if (_instance == null)
            {
                return _instance = new Manager();
            }
            return _instance;
        }
        public void GetListOfIps(BindingList<IPObject> ipList)
        {
            _ipList = ipList;
        }

        public void Start()
        {
            foreach (IPObject item in _ipList)
            {
                item.PingAction("start");
                item.Id = id;
                Thread th = new Thread(item.SendPing);
                th.Name = "Start Ping";
                th.IsBackground = true;
                th.Start();
                id++;
            }
            id = 1;
        }

        public List<string> GetFailedPings()
        {
            while (_exitloop == false)
            {
                _listofFailedPings.Clear();
                foreach (IPObject item in _ipList)
                {
                    if (item.ResultTime != "")
                    {
                        _listofFailedPings.Add(item.ResultTime);
                    }
                    item.ResultTime = "";
                }
                return _listofFailedPings;
            }
            return null;
        }

        public void Stop()
        {
            _exitloop = true;

            foreach (IPObject item in _ipList)
            {
                item.PingAction("stop");
            }
        }

        public BindingList<IPObject> Load()
        {
            BindingList<IPObject> _defaultlist = new BindingList<IPObject>();
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("127.0.0.1") });
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("172.217.23.67") });
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("10.51.16.1") });
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("10.51.16.2") });
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("10.51.16.254") });
            _defaultlist.Add(new IPObject() { Ip = IPAddress.Parse("10.51.10.13") });
            return _defaultlist;
        }

        public void Flush()
        {
            _ipList.Clear();
        }

        public void SaveLog(string filename,List<string> logoutput)
        {
            _exs = new ExportService(filename);
            _exs.Save(logoutput);
        }
    }
}

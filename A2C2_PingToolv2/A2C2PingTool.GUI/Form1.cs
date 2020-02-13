using A2C2_PingTool.Lib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace A2C2PingTool.GUI
{
    public partial class Form1 : Form
    {
        #region fields
        string btnstart = "Off";
        BindingList<IPObject> _ipList = new BindingList<IPObject>();
        Manager _manager;

        private delegate void updatelbx(string value);
        private updatelbx _updater;
        bool _getout;

        List<string> _listofFailedPings = new List<string>();
        bool _loadlimitreached = false;
        List<string> _itemsofLogOutput;
        string _filename = @"C:\Users\caglar\Desktop\ac2c.txt";
        #endregion

        public Form1()
        {
            InitializeComponent();
            this.Text = "PingTool";
            _updater = new updatelbx(_UpdateLog);
            tbx_Ip.Select();
            iPObjectDataGridView.EnableHeadersVisualStyles = false;
            iPObjectDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            lbx_failedPings.Items.Add("Log:");
        }

        private void btn_Start_Click(object sender, EventArgs e)
        {
            StartProcess();
        }

        private void StartProcess()
        {
            if (_ipList.Count > 0)
            {
                ExecuteManager();
                changebuttonname();
            }
            else
            {
                MessageBox.Show("No IP Address given.");
            }
        }

        private void ExecuteManager()
        {
            _manager = Manager.GetInstance();
            _manager.GetListOfIps(_ipList);
        }

        public void changebuttonname()
        {
            if (btnstart == "Off")
            {
                btn_Add.Enabled = false;
                btn_Clear.Enabled = false;
                btn_Load.Enabled = false;
                btnstart = "On";
                btn_Start.Text = "Stop";
                _manager.Start();
                _getout = false;
                Thread th = new Thread(GetUpdatesfromManager);
                th.IsBackground = true;
                th.Name = "Get Updates";
                th.Start();
            }
            else
            {
                btn_Add.Enabled = true;
                btn_Clear.Enabled = true;
                btn_Load.Enabled = true;
                btnstart = "Off";
                btn_Start.Text = "Start";
                _getout = true;
                _manager.Stop();
            }
        }

        public void _UpdateLog(string message)
        {
            if (lbx_failedPings.InvokeRequired)
            {
                lbx_failedPings.Invoke(_updater, message);
            }
            else
            {
                lbx_failedPings.Items.Add(message);
            }
        }

        public void GetUpdatesfromManager()
        {
            while (_getout == false)
            {
                _listofFailedPings = _manager.GetFailedPings();
                for (int i = 0; i < _listofFailedPings.Count; i++)
                {
                    _UpdateLog(_listofFailedPings[i].ToString());
                }
            }
        }

        private void btn_Add_Click(object sender, EventArgs e)
        {
            try
            {
                IPAddress _ip = IPAddress.Parse(tbx_Ip.Text);
                _ipList.Add(new IPObject() { Ip = _ip });
                iPObjectDataGridView.DataSource = _ipList; //aktualiesert datagridview an dieser Stelle
                tbx_Ip.Text = string.Empty;
            }
            catch (Exception)
            {
                MessageBox.Show("ungültige IP Adresse.");
            }
            tbx_Ip.Select();
        }

        private void btn_Clear_Click(object sender, EventArgs e)
        {
            _manager.Flush();
            _loadlimitreached = false;
        }

        private void btn_ClearLog_Click(object sender, EventArgs e)
        {
            lbx_failedPings.Items.Clear();
            lbx_failedPings.Items.Add("Log:");
        }

        private void btn_Load_Click(object sender, EventArgs e)
        {
            if (_loadlimitreached != true)
            {
                _manager = Manager.GetInstance();
                foreach (var item in _manager.Load())
                {
                    _ipList.Add(item);
                }
                iPObjectDataGridView.DataSource = _ipList;
            }
            _loadlimitreached = true;
        }

        private void btn_ChoosePath_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                _filename = dialog.FileName;
            }
        }

        private void btn_Save_Click(object sender, EventArgs e)
        {
            try
            {
                _itemsofLogOutput = new List<string>();
                for (int i = 0; i < lbx_failedPings.Items.Count; i++)
                {
                    _itemsofLogOutput.Add(lbx_failedPings.Items[i].ToString());
                }
                if (_itemsofLogOutput.Count!=0)
                {
                    _manager.SaveLog(_filename, _itemsofLogOutput);
                    MessageBox.Show("Log saved successfully");
                }
                else
                {
                    MessageBox.Show("emmpty log");
                }
            }

            catch (Exception)
            {
                MessageBox.Show("select a valid path!");
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Automation.FrameworkExtension.deviceDriver;
using Automation.FrameworkExtension.elementsInterfaces;
using DeviceMeasureClassLibrary;

namespace Automation.Base.DeviceMeasureClassLibrary
{
    public partial class LaserBaseControl : UserControl, IDeviceControl<ILaser>
    {
        public LaserBaseControl()
        {
            InitializeComponent();
        }


        private ILaser _device;

        public void LoadDevice(ILaser device)
        {
            groupBoxDev.Text = device.ToString();

            _device = device;
        }

        public void UserActivate()
        {
            throw new NotImplementedException();
        }

        public void UserDeactivate()
        {
            throw new NotImplementedException();
        }

        private void buttonOpen_Click(object sender, EventArgs e)
        {
            try
            {
                _device?.Initialize();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                _device?.Terminate();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonTrigger_Click(object sender, EventArgs e)
        {
            try
            {
                var ret = _device.Measure();
                UpdateLog(string.Join(",", ret.Select(d => d.ToString("F6"))));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void UpdateLog(string log)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(UpdateLog), log);
            }
            else
            {
                richTextBoxLog.AppendText($"{DateTime.Now.ToString("yyyyMMdd-HHmmss.fff")}: {log}\r\n");
                richTextBoxLog.ScrollToCaret();
            }
        }

        private void richTextBoxLog_DoubleClick(object sender, EventArgs e)
        {
            if (MessageBox.Show("清除记录？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                richTextBoxLog.Clear();
                richTextBoxLog.ScrollToCaret();
            }
        }
    }
}

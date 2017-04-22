using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;

namespace TempAlert
{
    public partial class MainForm : Form
    {
        private Computer computer = new Computer();
        private float maxTempThreshold = 50;
        private float currentMaxTemp = 0;
        private DateTime nextAlert = DateTime.UtcNow;
        private bool alerting = true;
        public MainForm()
        {
            InitializeComponent();
            this.FormClosing += MainForm_FormClosing;

            computer.CPUEnabled = true;
            computer.GPUEnabled = true;
            computer.MainboardEnabled = true;
            computer.HDDEnabled = true;
            computer.RAMEnabled = true;
            computer.Open();

            UpdateTemps();

            updateTimer.Tick += UpdateTemps_Tick;
            updateTimer.Enabled = true;
        }

        private void UpdateTemps_Tick(object sender, EventArgs e)
        {
            UpdateTemps();
            HighTemperatureAlertCheck();
        }

        private void UpdateTemps()
        {
            var readingsText = new StringBuilder();
            var tempList = new List<ISensor>();
            foreach (var hardware in computer.Hardware)
            {
                hardware.Update();
                var tempsensors = hardware.Sensors.Where(s => s.SensorType == SensorType.Temperature);
                if (tempsensors.Count() == 0)
                {
                    // nothing to show for this device
                    continue;
                }
                
                readingsText.AppendLine(hardware.Name + ":");
                foreach (var sensor in tempsensors)
                {
                    if (sensor.Value.HasValue)
                    {
                        tempList.Add(sensor);
                        readingsText.AppendLine(sensor.Name + " is @ " + sensor.Value + "°C");
                    }
                }
                readingsText.AppendLine("-----");
            }
            sensorReadingsTextBox.Text = readingsText.ToString();

            if (tempList.Count > 0)
            {
                currentMaxTemp = tempList.Max(t => t.Value).Value;
                maxTempLabel.Text = currentMaxTemp.ToString() + "°C";
            } else
            {
                maxTempLabel.ResetText();
            }
        }

        private void HighTemperatureAlertCheck()
        {
            if (nextAlert > DateTime.UtcNow || !alerting)
            {
                return;
            }

            if (currentMaxTemp > maxTempThreshold)
            {
                HighTemperatureAlert(currentMaxTemp);
            }
        }

        private void HighTemperatureAlert(float max)
        {
            alerting = false;
            MessageBox.Show("A device is reporting abnormally high temperatures. It is reporting a temperature of " + max.ToString() + "°C", "TempAlert Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            nextAlert = DateTime.UtcNow.AddMinutes(5);
            alerting = true;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                this.Hide();
                e.Cancel = true;
                notifyIcon.ShowBalloonTip(10000, "TempAlert is still running", "TempAlert is continuing to monitor your computer in the background.", ToolTipIcon.Info);
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to quit? This will stop monitoring.", "Quit?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void notifyIcon_DoubleClick(object sender, EventArgs e)
        {
            this.Show();
        }

        private void MainForm_FormClosing(object sender, EventArgs e)
        {
            notifyIcon.Dispose();
        }
    }
}

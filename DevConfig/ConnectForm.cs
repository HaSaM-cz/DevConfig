﻿using CanDiagSupport;
using DevConfig.Properties;
using DevConfig.Service;
using System.IO.Ports;

namespace DevConfig
{
    public partial class ConnectForm : Form
    {
        public IInputPeriph? InputPeriph = null;
        ///////////////////////////////////////////////////////////////////////////////////////////
        public ConnectForm()
        {
            InitializeComponent();
            btnRefreshPorts_Click(btnRefreshPorts, new EventArgs());
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void OpenPort_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                String? connect_str = null;
                if (rbUsbCom.Checked)
                {
                    connect_str = $"USB Serial/{cbbPortName.Text}:{cbbPortSpeed.Text}";
                }
                else if (rbToolstick.Checked)
                {
                    connect_str = $"USB ToolStick/{cbbPortName.Text}:{cbbPortSpeed.Text}"; 
                }
                else if (rbTcpSocket.Checked)
                {
                    connect_str = $"TCP Tunel/{txtHost.Text}:{txtPort.Text}";
                }
                else if (rbSshKaro.Checked)
                {
                    connect_str = $"SSH Karo CAN/{txtHost.Text}:{txtPort.Text}";
                }

                if (!string.IsNullOrWhiteSpace(connect_str))
                {
                    if(DevConfigService.Instance.Open(connect_str))
                        DialogResult = DialogResult.Continue;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally 
            {
                Cursor = Cursors.Default; 
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void ConnectForm_Load(object sender, EventArgs e)
        {
            cbbPortName.Text = Settings.Default.ComPort;
            cbbPortSpeed.Text = Settings.Default.BaudRate;
            txtHost.Text = Settings.Default.TcpHost;
            txtPort.Text = Settings.Default.TcpPort;

            rbUsbCom.Checked = false;
            rbToolstick.Checked = false;
            rbTcpSocket.Checked = false;
            rbSshKaro.Checked = false;

            switch (Settings.Default.Communication)
            {
                case 0: rbUsbCom.Checked = true; break;
                case 1: rbToolstick.Checked = true; break;
                case 2: default: rbTcpSocket.Checked = true; break;
                case 3: rbSshKaro.Checked = true; break;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void ConnectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            int Communication = -1;
            if (rbUsbCom.Checked)
                Communication = 0;
            else if (rbToolstick.Checked)
                Communication = 1;
            else if (rbTcpSocket.Checked)
                Communication = 2;
            else if (rbSshKaro.Checked)
                Communication = 3;

            if (Settings.Default.ComPort != cbbPortName.Text ||
                Settings.Default.BaudRate != cbbPortSpeed.Text ||
                Settings.Default.TcpHost != txtHost.Text ||
                Settings.Default.TcpPort != txtPort.Text ||
                Settings.Default.Communication != Communication)

            {
                Settings.Default.ComPort = cbbPortName.Text;
                Settings.Default.BaudRate = cbbPortSpeed.Text;
                Settings.Default.TcpHost = txtHost.Text;
                Settings.Default.TcpPort = txtPort.Text;
                Settings.Default.Communication = Communication;
                Settings.Default.Save();
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void rbConnection_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSshKaro.Checked)
            {
                label_com_port.Enabled = false;
                label_com_bd.Enabled = false;
                label_tcp_host.Enabled = true;
                label_tcp_port.Enabled = true;
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                cbbPortSpeed.Enabled = false;
                btnRefreshPorts.Enabled = false;
                cbbPortName.Enabled = false;

                txtPort.Text = "22";
            }
            else if (rbToolstick.Checked)
            {
                label_com_port.Enabled = true;
                label_com_bd.Enabled = true;
                label_tcp_host.Enabled = false;
                label_tcp_port.Enabled = false;
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                cbbPortSpeed.Enabled = true;
                btnRefreshPorts.Enabled = true;
                cbbPortName.Enabled = true;

                cbbPortSpeed.Text = "125000";
            }
            else if (rbUsbCom.Checked)
            {
                label_com_port.Enabled = true;
                label_com_bd.Enabled = true;
                label_tcp_host.Enabled = false;
                label_tcp_port.Enabled = false;
                txtHost.Enabled = false;
                txtPort.Enabled = false;
                cbbPortSpeed.Enabled = true;
                btnRefreshPorts.Enabled = true;
                cbbPortName.Enabled = true;

                cbbPortSpeed.Text = "115200";
            }
            else if (rbTcpSocket.Checked)
            {
                label_com_port.Enabled = false;
                label_com_bd.Enabled = false;
                label_tcp_host.Enabled = true;
                label_tcp_port.Enabled = true;
                txtHost.Enabled = true;
                txtPort.Enabled = true;
                cbbPortSpeed.Enabled = false;
                btnRefreshPorts.Enabled = false;
                cbbPortName.Enabled = false;

                txtPort.Text = Settings.Default.TcpPort;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnRefreshPorts_Click(object sender, EventArgs e)
        {
            cbbPortName.Items.Clear();
            cbbPortName.Items.AddRange(SerialPort.GetPortNames());

            if (cbbPortName.Items.Count > 0)
                cbbPortName.SelectedIndex = 0;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

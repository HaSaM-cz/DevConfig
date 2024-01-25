namespace DevConfig
{
    partial class ConnectForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            rbUsbCom = new RadioButton();
            rbToolstick = new RadioButton();
            rbTcpSocket = new RadioButton();
            cbbPortName = new ComboBox();
            label_com_port = new Label();
            label_tcp_host = new Label();
            txtHost = new TextBox();
            label_tcp_port = new Label();
            txtPort = new TextBox();
            label_com_bd = new Label();
            cbbPortSpeed = new ComboBox();
            btnOpen = new Button();
            buttonCancel = new Button();
            groupBox1 = new GroupBox();
            rbSshKaro = new RadioButton();
            btnRefreshPorts = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // rbUsbCom
            // 
            rbUsbCom.AutoSize = true;
            rbUsbCom.Location = new Point(6, 26);
            rbUsbCom.Name = "rbUsbCom";
            rbUsbCom.Size = new Size(94, 24);
            rbUsbCom.TabIndex = 0;
            rbUsbCom.TabStop = true;
            rbUsbCom.Text = "USB COM";
            rbUsbCom.UseVisualStyleBackColor = true;
            rbUsbCom.CheckedChanged += rbConnection_CheckedChanged;
            // 
            // rbToolstick
            // 
            rbToolstick.AutoSize = true;
            rbToolstick.Location = new Point(6, 56);
            rbToolstick.Name = "rbToolstick";
            rbToolstick.Size = new Size(88, 24);
            rbToolstick.TabIndex = 1;
            rbToolstick.TabStop = true;
            rbToolstick.Text = "Toolstick";
            rbToolstick.UseVisualStyleBackColor = true;
            rbToolstick.CheckedChanged += rbConnection_CheckedChanged;
            // 
            // rbTcpSocket
            // 
            rbTcpSocket.AutoSize = true;
            rbTcpSocket.Location = new Point(6, 86);
            rbTcpSocket.Name = "rbTcpSocket";
            rbTcpSocket.Size = new Size(91, 24);
            rbTcpSocket.TabIndex = 2;
            rbTcpSocket.TabStop = true;
            rbTcpSocket.Text = "TCP tunel";
            rbTcpSocket.UseVisualStyleBackColor = true;
            rbTcpSocket.CheckedChanged += rbConnection_CheckedChanged;
            // 
            // cbbPortName
            // 
            cbbPortName.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbPortName.FormattingEnabled = true;
            cbbPortName.Location = new Point(164, 48);
            cbbPortName.Margin = new Padding(4, 5, 4, 5);
            cbbPortName.Name = "cbbPortName";
            cbbPortName.Size = new Size(160, 28);
            cbbPortName.TabIndex = 92;
            // 
            // label_com_port
            // 
            label_com_port.AutoSize = true;
            label_com_port.Location = new Point(164, 23);
            label_com_port.Margin = new Padding(4, 0, 4, 0);
            label_com_port.Name = "label_com_port";
            label_com_port.Size = new Size(35, 20);
            label_com_port.TabIndex = 96;
            label_com_port.Text = "Port";
            // 
            // label_tcp_host
            // 
            label_tcp_host.AutoSize = true;
            label_tcp_host.Location = new Point(164, 81);
            label_tcp_host.Margin = new Padding(4, 0, 4, 0);
            label_tcp_host.Name = "label_tcp_host";
            label_tcp_host.Size = new Size(65, 20);
            label_tcp_host.TabIndex = 122;
            label_tcp_host.Text = "TCP host";
            // 
            // txtHost
            // 
            txtHost.Location = new Point(164, 106);
            txtHost.Margin = new Padding(4, 5, 4, 5);
            txtHost.Name = "txtHost";
            txtHost.Size = new Size(160, 27);
            txtHost.TabIndex = 121;
            txtHost.Text = "10.0.0.149";
            // 
            // label_tcp_port
            // 
            label_tcp_port.AutoSize = true;
            label_tcp_port.Location = new Point(332, 81);
            label_tcp_port.Margin = new Padding(4, 0, 4, 0);
            label_tcp_port.Name = "label_tcp_port";
            label_tcp_port.Size = new Size(65, 20);
            label_tcp_port.TabIndex = 120;
            label_tcp_port.Text = "TCP port";
            // 
            // txtPort
            // 
            txtPort.Location = new Point(332, 106);
            txtPort.Margin = new Padding(4, 5, 4, 5);
            txtPort.Name = "txtPort";
            txtPort.Size = new Size(86, 27);
            txtPort.TabIndex = 119;
            txtPort.Text = "10001";
            // 
            // label_com_bd
            // 
            label_com_bd.AutoSize = true;
            label_com_bd.Location = new Point(332, 23);
            label_com_bd.Margin = new Padding(4, 0, 4, 0);
            label_com_bd.Name = "label_com_bd";
            label_com_bd.Size = new Size(73, 20);
            label_com_bd.TabIndex = 116;
            label_com_bd.Text = "BaudRate";
            // 
            // cbbPortSpeed
            // 
            cbbPortSpeed.DropDownStyle = ComboBoxStyle.DropDownList;
            cbbPortSpeed.FormattingEnabled = true;
            cbbPortSpeed.Items.AddRange(new object[] { "1200", "2400", "4800", "9600", "14400", "19200", "38400", "57600", "115200" });
            cbbPortSpeed.Location = new Point(332, 48);
            cbbPortSpeed.Margin = new Padding(4, 5, 4, 5);
            cbbPortSpeed.Name = "cbbPortSpeed";
            cbbPortSpeed.Size = new Size(84, 28);
            cbbPortSpeed.TabIndex = 115;
            // 
            // btnOpen
            // 
            btnOpen.Location = new Point(223, 150);
            btnOpen.Margin = new Padding(4, 5, 4, 5);
            btnOpen.Name = "btnOpen";
            btnOpen.Size = new Size(94, 29);
            btnOpen.TabIndex = 123;
            btnOpen.Text = "Open";
            btnOpen.UseVisualStyleBackColor = true;
            btnOpen.Click += OpenPort_Click;
            // 
            // buttonCancel
            // 
            buttonCancel.DialogResult = DialogResult.Cancel;
            buttonCancel.Location = new Point(324, 150);
            buttonCancel.Name = "buttonCancel";
            buttonCancel.Size = new Size(94, 29);
            buttonCancel.TabIndex = 125;
            buttonCancel.Text = "Cancel";
            buttonCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rbSshKaro);
            groupBox1.Controls.Add(rbTcpSocket);
            groupBox1.Controls.Add(rbUsbCom);
            groupBox1.Controls.Add(rbToolstick);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(145, 149);
            groupBox1.TabIndex = 126;
            groupBox1.TabStop = false;
            groupBox1.Text = "Connect Type";
            // 
            // rbSshKaro
            // 
            rbSshKaro.AutoSize = true;
            rbSshKaro.Location = new Point(6, 116);
            rbSshKaro.Name = "rbSshKaro";
            rbSshKaro.Size = new Size(65, 24);
            rbSshKaro.TabIndex = 3;
            rbSshKaro.TabStop = true;
            rbSshKaro.Text = "KaRo";
            rbSshKaro.UseVisualStyleBackColor = true;
            rbSshKaro.CheckedChanged += rbConnection_CheckedChanged;
            // 
            // btnRefreshPorts
            // 
            btnRefreshPorts.Font = new Font("Adobe Heiti Std R", 6F, FontStyle.Regular, GraphicsUnit.Point);
            btnRefreshPorts.Location = new Point(253, 22);
            btnRefreshPorts.Margin = new Padding(4, 5, 4, 5);
            btnRefreshPorts.Name = "btnRefreshPorts";
            btnRefreshPorts.Size = new Size(71, 24);
            btnRefreshPorts.TabIndex = 127;
            btnRefreshPorts.Text = "refresh";
            btnRefreshPorts.UseVisualStyleBackColor = true;
            btnRefreshPorts.Click += btnRefreshPorts_Click;
            // 
            // ConnectForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(433, 194);
            Controls.Add(cbbPortSpeed);
            Controls.Add(btnRefreshPorts);
            Controls.Add(groupBox1);
            Controls.Add(buttonCancel);
            Controls.Add(btnOpen);
            Controls.Add(label_tcp_host);
            Controls.Add(txtHost);
            Controls.Add(label_tcp_port);
            Controls.Add(txtPort);
            Controls.Add(label_com_bd);
            Controls.Add(label_com_port);
            Controls.Add(cbbPortName);
            Name = "ConnectForm";
            Text = "ConnectForm";
            FormClosing += ConnectForm_FormClosing;
            Load += ConnectForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private RadioButton rbUsbCom;
        private RadioButton rbToolstick;
        private RadioButton rbTcpSocket;
        private ComboBox cbbPortName;
        private Label label_com_port;
        private Label label_tcp_host;
        private TextBox txtHost;
        private Label label_tcp_port;
        private TextBox txtPort;
        private Label label_com_bd;
        private ComboBox cbbPortSpeed;
        private Button btnOpen;
        private Button buttonCancel;
        private GroupBox groupBox1;
        private Button btnRefreshPorts;
        private RadioButton rbSshKaro;
    }
}
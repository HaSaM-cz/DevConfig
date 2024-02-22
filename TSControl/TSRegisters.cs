using DevConfigSupp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Message = CanDiagSupport.Message;

namespace TSControl
{
    public partial class TSRegisters : DockContentEx
    {
        public TSRegisters()
        {
            InitializeComponent();
        }

        protected override void InputPeriph_MessageReceived(Message msg)
        {

        }
    }
}

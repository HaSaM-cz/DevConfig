using CanDiagSupport;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConfigSupp
{
    public interface IMainApp
    {
        IInputPeriph? inputPeriph { get; }
        public object? GetProperty(string PropName);
        public void SetProperty(string PropName, object PropValue);
        public void AppendToDebug(string text, bool bNewLine = true, bool bBolt = false, Color? color = null);
    }
}

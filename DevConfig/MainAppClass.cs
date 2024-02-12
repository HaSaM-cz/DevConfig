using CanDiagSupport;
using DevConfig.Service;
using DevConfigSupp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConfig
{
    internal class MainAppClass : IMainApp
    {
        private MainForm MainForm;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public MainAppClass(MainForm main_form)
        {
            MainForm = main_form;
        }

        public IInputPeriph? inputPeriph => DevConfigService.Instance.InputPeriph;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void AppendToDebug(string text, bool bNewLine = true, bool bBolt = false, Color? color = null)
        {
            MainForm.AppendToDebug(text, bNewLine, bBolt, color);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public object? GetProperty(string PropName)
        {
            switch (PropName)
            {
                case "SelectedDeviceCanID":
                    return DevConfigService.Instance.selectedDevice?.Address;
            }
            return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetProperty(string PropName, object PropValue)
        {
            switch(PropName)
            {
                case "AppendToDebug":
                    MainForm.AppendToDebug((string)PropValue);
                    Debug.WriteLine($"{PropValue}");
                    break;
                case "ProgressMin":
                    MainForm.ProgressBar_Minimum = (int)PropValue;
                    break;
                case "ProgressMax":
                    MainForm.ProgressBar_Maximum = (int)PropValue;
                    break;
                case "ProgressValue":
                    MainForm.ProgressBar_Value = (int)PropValue;
                    break;
            }
        }
    }
}

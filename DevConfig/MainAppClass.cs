using CanDiagSupport;
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

        public IInputPeriph? inputPeriph => MainForm.InputPeriph;

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
                    return MainForm.selectedDevice?.Address;
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
                    MainForm.progressBar.Minimum = (int)PropValue;
                    break;
                case "ProgressMax":
                    MainForm.progressBar.Maximum = (int)PropValue;
                    break;
                case "ProgressValue":
                    if (MainForm.progressBar.Maximum < (int)PropValue)
                    {
                        Debug.WriteLine($"PropName = {PropName}, PropValue = {PropValue}");
                        PropValue = MainForm.progressBar.Maximum;
                    }
                    MainForm.progressBar.Value = (int)PropValue;
                    break;
            }
        }
    }
}

using CanDiagSupport;
using Message = CanDiagSupport.Message;

namespace DevConfigSupp
{
    public class DockContentEx : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        protected IMainApp? MainApp;
        protected IInputPeriph? InputPeriph;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public DockContentEx()
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetMainApp(IMainApp main_app)
        {
            MainApp = main_app;
            if (MainApp.inputPeriph != null)
            {
                InputPeriph = MainApp.inputPeriph;
                InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void InputPeriph_MessageReceived(Message msg)
        {
            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void SendMessage(Message message)
        {
            if (MainApp != null && MainApp.inputPeriph != null && !MainApp.inputPeriph.Equals(InputPeriph))
            {
                InputPeriph = MainApp.inputPeriph;
                InputPeriph.MessageReceived += InputPeriph_MessageReceived;
            }
            MainApp?.inputPeriph?.SendMsg(message);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

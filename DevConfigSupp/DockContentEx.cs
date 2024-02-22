using CanDiagSupport;
using Message = CanDiagSupport.Message;

namespace DevConfigSupp
{
    public class DockContentEx : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        protected IMainApp? MainApp = null;
        protected IInputPeriph? InputPeriph = null;

        ///////////////////////////////////////////////////////////////////////////////////////////
        public DockContentEx()
        {
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public void SetMainApp(IMainApp main_app)
        {
            MainApp = main_app;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void InputPeriph_MessageReceived(Message msg)
        {
            throw new NotImplementedException();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        protected void SendMessage(Message message)
        {
            if(MainApp != null && MainApp.inputPeriph != null)
            {
                if(InputPeriph != MainApp.inputPeriph)
                {
                    InputPeriph = MainApp.inputPeriph;
                    InputPeriph.MessageReceived += InputPeriph_MessageReceived;
                }
                MainApp.inputPeriph.SendMsg(message);
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

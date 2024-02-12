

namespace DevConfig
{
    public partial class DebugForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        ///////////////////////////////////////////////////////////////////////////////////////////
        public DebugForm(MainForm mainForm)
        {
            InitializeComponent();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        Font? font_bold = null;
        Font? font_normal = null;
        public delegate void AppendLogDelegate(string s, bool bNewLine, bool bBolt, Color? c);
        public void AppendText(string text, bool bNewLine = true, bool bBolt = false, Color? color = null)
        {
            try
            {
                if(color == null)
                    color = SystemColors.WindowText;

                if (rtbLog.InvokeRequired)
                {
                    object[] args = { text, bNewLine, bBolt, color };
                    rtbLog.Invoke(new AppendLogDelegate(AppendText), args);
                }
                else
                {
                    if (font_normal == null || font_bold == null)
                    {
                        font_normal = new Font(rtbLog.Font, FontStyle.Regular);
                        font_bold = new Font(rtbLog.Font, FontStyle.Bold);
                    }
                    rtbLog.SelectionFont = (bBolt ? font_bold : font_normal);
                    rtbLog.SelectionColor = (Color)color;
                    rtbLog.AppendText(text);
                    if (bNewLine)
                    {
                        rtbLog.AppendText(Environment.NewLine);
                        //TrimLog();
                        rtbLog.SelectionStart = rtbLog.TextLength;
                        rtbLog.ScrollToCaret();
                    }
                }
            }
            catch (ObjectDisposedException)
            {
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

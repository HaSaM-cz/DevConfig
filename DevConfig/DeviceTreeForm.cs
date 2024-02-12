using DevConfig.Service;


namespace DevConfig
{
    public partial class DeviceTreeForm : WeifenLuo.WinFormsUI.Docking.DockContent
    {
        MainForm MainForm;
        public DeviceTreeForm(MainForm mainForm)
        {
            InitializeComponent();
            MainForm = mainForm;
        }

        private void listViewDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var items = listViewDevices.SelectedItems;
            if (items != null && items.Count == 1)
            {
                var item = items[0];
                if (item != null)
                    MainForm.SelectItem((Device)item.Tag);
            }
        }
    }
}

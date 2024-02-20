using DevConfig.Service;
using DevConfig.Utils;
using System.Diagnostics;

namespace DevConfig
{
    public partial class BackupForm : Form
    {
        public Backup_t BackupObj;
        ///////////////////////////////////////////////////////////////////////////////////////////
        public BackupForm(Backup_t backupObj)
        {
            InitializeComponent();
            BackupObj = backupObj;
            if (backupObj.restore)
            {
                Text = "DevConfig - Restore";
                label_Location.Text = "Restore from";
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void BackupForm_Load(object sender, EventArgs e)
        {
            label_Files.Text = BackupObj.file_cnt.ToString();
            label_Directorys.Text = BackupObj.dir_cnt.ToString();
            label_Bytes.Text = BackupObj.size.ToHumanSting();
            label_Time.Text = new TimeSpan(0, 0, (int)(BackupObj.size / 5000)).ToString();

            BackupDestination.Text = BackupObj.backup_destination;
            tbExclude.Text = string.Join(";", BackupObj.exclude_ext);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void Browse_Folder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog() { SelectedPath = BackupDestination.Text };
            if (dialog.ShowDialog() != DialogResult.OK)
                return;
            BackupDestination.Text = dialog.SelectedPath;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void BackupForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            BackupObj.backup_destination = BackupDestination.Text;
            BackupObj.exclude_ext = tbExclude.Text.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            Debug.Assert(this.Tag.GetType() == typeof(SDCardCtrl));

            BackupObj.size = 0;
            BackupObj.dir_cnt = 0;
            BackupObj.file_cnt = 0;
            BackupObj.files.Clear();
            BackupObj.backup_destination = BackupDestination.Text;
            BackupObj.exclude_ext = tbExclude.Text.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            try
            {
                SDCardCtrl sDCardCtrl = (SDCardCtrl)this.Tag;
                if (BackupObj.restore)
                    sDCardCtrl.RestorePrepare(BackupObj.backup_destination);
                else
                    sDCardCtrl.BackupPrepare(sDCardCtrl.treeView1.TopNode);
            }
            catch (Exception ex)
            {
                DevConfigService.Instance.mainForm.AppendToDebug($"{ex.Message}", default, default, Color.Red);
            }

            label_Files.Text = BackupObj.file_cnt.ToString();
            label_Directorys.Text = BackupObj.dir_cnt.ToString();
            label_Bytes.Text = BackupObj.size.ToHumanSting();
            label_Time.Text = new TimeSpan(0, 0, (int)(BackupObj.size / 5000)).ToString();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        private void button_Start_Click(object sender, EventArgs e)
        {
            if (BackupObj.restore)
            {
                DialogResult dr = MessageBox.Show($"The SD card in the device will be formatted and overwritten with new content.{Environment.NewLine}Do you want to perform this operation?",
                    "DevConfig - restore", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            }
            else
            {
                DirectoryInfo di = new DirectoryInfo(BackupDestination.Text);
                if (di.Exists && (di.EnumerateFiles().Count() > 0 || di.EnumerateDirectories().Count() > 0))
                {
                    DialogResult dr = MessageBox.Show($"The directory is not empty.{Environment.NewLine}Do you want to delete the contents before the operation?",
                        "DevConfig - backup", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (dr == DialogResult.Yes)
                    {
                        // vymazat obsah
                        foreach (var f in di.GetFiles())
                            f.Delete();

                        foreach (var f in di.GetDirectories())
                            f.Delete(true);
                    }
                    else if (dr == DialogResult.Cancel)
                    {
                        // prerusit operaci
                        DialogResult = DialogResult.None;
                    }
                }
            }
        }
        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}


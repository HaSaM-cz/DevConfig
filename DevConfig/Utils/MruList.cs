using CanDiag;

namespace DevConfig.Utils
{
    public class MruList<T>
    {
        // The application's name.
        private string ApplicationName;

        // A list of the files.
        private int NumFiles;
        private List<object> FileInfos;

        // The File menu.
        private ToolStripMenuItem MyMenu;

        // The menu items we use to display files.
        private ToolStripSeparator Separator;
        private ToolStripMenuItem[] MenuItems;

        // Raised when the user selects a file from the MRU list.
        public delegate void FileSelectedEventHandler(string file_name);
        public event FileSelectedEventHandler? FileSelected;

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Constructor.
        public MruList(string application_name, ToolStripMenuItem menu, ToolStripMenuItem sub_menu, int num_files)
        {
            ApplicationName = application_name;
            MyMenu = menu;
            NumFiles = num_files;
            FileInfos = new();

            // Make a separator.
            Separator = new ToolStripSeparator();
            Separator.Visible = false;

#if SUB_ITEM
            MyMenu.DropDownItems.Add(Separator);
#else
            int ind = MyMenu.DropDownItems.IndexOf(sub_menu);
            MyMenu.DropDownItems.Insert(++ind, Separator);
#endif

            // Make the menu items we may later need.
            MenuItems = new ToolStripMenuItem[NumFiles + 1];
            for (int i = 0; i < NumFiles; i++)
            {
                MenuItems[i] = new ToolStripMenuItem();
                MenuItems[i].Visible = false;
#if SUB_ITEM
                MyMenu.DropDownItems.Add(MenuItems[i]);
#else
                MyMenu.DropDownItems.Insert(++ind, MenuItems[i]);
#endif
            }

            // Reload items from the registry.
            LoadFiles();

            // Display the items.
            ShowFiles();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public string? GetFirstFile()
        {
            if (FileInfos != null && FileInfos.Count > 0)
            {
                if (FileInfos[0].GetType() == typeof(System.IO.FileInfo))
                    return ((System.IO.FileInfo)FileInfos[0]).FullName;
                else
                    return $"{FileInfos[0]}";
            }
            else
                return null;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Load saved items from the Registry.
        private void LoadFiles()
        {
            // Reload items from the registry.
            for (int i = 0; i < NumFiles; i++)
            {
                string file_name = (string)RegistryTools.GetSetting(
                    ApplicationName, "FilePath" + i.ToString(), "");
                if (file_name != "")
                {
                    if (typeof(T) == typeof(System.IO.FileInfo))
                        FileInfos.Add(new System.IO.FileInfo(file_name));
                    else
                        FileInfos.Add($"{file_name}");
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Save the current items in the Registry.
        private void SaveFiles()
        {
            // Delete the saved entries.
            for (int i = 0; i < NumFiles; i++)
            {
                RegistryTools.DeleteSetting(ApplicationName, "FilePath" + i.ToString());
            }

            // Save the current entries.
            int index = 0;
            foreach (var file_info in FileInfos)
            {
                if (file_info.GetType() == typeof(System.IO.FileInfo))
                    RegistryTools.SaveSetting(ApplicationName, "FilePath" + index.ToString(), ((System.IO.FileInfo)file_info).FullName);
                else
                    RegistryTools.SaveSetting(ApplicationName, "FilePath" + index.ToString(), $"{file_info}");
                index++;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Remove a file's info from the list.
        private void RemoveFileInfo(string file_name)
        {
            // Remove occurrences of the file's information from the list.
            for (int i = FileInfos.Count - 1; i >= 0; i--)
            {
                if (FileInfos[i].GetType() == typeof(System.IO.FileInfo))
                {
                    if (((System.IO.FileInfo)FileInfos[i]).FullName == file_name)
                        FileInfos.RemoveAt(i);
                }
                else
                {
                    if ($"{FileInfos[i]}" == file_name)
                        FileInfos.RemoveAt(i);
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Add a file to the list, rearranging if necessary.
        public void AddFile(string file_name)
        {
            // Remove the file from the list.
            RemoveFileInfo(file_name);

            // Add the file to the beginning of the list.
            if (typeof(T) == typeof(System.IO.FileInfo))
                FileInfos.Insert(0, new System.IO.FileInfo(file_name));
            else
                FileInfos.Insert(0, file_name);

            // If we have too many items, remove the last one.
            if (FileInfos.Count > NumFiles) FileInfos.RemoveAt(NumFiles);

            // Display the files.
            ShowFiles();

            // Update the Registry.
            SaveFiles();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Remove a file from the list, rearranging if necessary.
        public void RemoveFile(string file_name)
        {
            // Remove the file from the list.
            RemoveFileInfo(file_name);

            // Display the files.
            ShowFiles();

            // Update the Registry.
            SaveFiles();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Display the files in the menu items.
        private void ShowFiles()
        {
            Separator.Visible = FileInfos.Count > 0;
            for (int i = 0; i < FileInfos.Count; i++)
            {
                if (FileInfos[i].GetType() == typeof(System.IO.FileInfo))
                    MenuItems[i].Text = string.Format("&{0} {1}", i + 1, ((System.IO.FileInfo)FileInfos[i]).Name);
                else
                    MenuItems[i].Text = $"{FileInfos[i]}";
                MenuItems[i].Visible = true;
                MenuItems[i].Tag = FileInfos[i];
                MenuItems[i].Click -= File_Click;
                MenuItems[i].Click += File_Click;
            }
            for (int i = FileInfos.Count; i < NumFiles; i++)
            {
                MenuItems[i].Visible = false;
                MenuItems[i].Click -= File_Click;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        // The user selected a file from the menu.
        private void File_Click(object sender, EventArgs e)
        {
            // Don't bother if no one wants to catch the event.
            if (FileSelected != null)
            {
                // Get the corresponding FileInfo object.
                ToolStripMenuItem menu_item = (ToolStripMenuItem)sender;

                if (menu_item.Tag.GetType() == typeof(System.IO.FileInfo))
                {
                    System.IO.FileInfo file_info = (System.IO.FileInfo)menu_item.Tag;
                    // Raise the event.
                    FileSelected(file_info.FullName);
                }
                else
                {
                    FileSelected($"{menu_item.Tag}");
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
    }
}

using System.Data;

namespace DevConfig
{
    class DirInfo
    {
        public List<FileInfo>? FileInfoList = null;

        internal void RemoveFileInfo(string name)
        {
            FileInfo? fi = (from xxx in FileInfoList where xxx.Name == name select xxx).FirstOrDefault();
            if (fi != null)
                FileInfoList?.Remove(fi);
        }

        internal void RenameFileInfo(string old_name, string new_name)
        {
            FileInfo? fi = (from xxx in FileInfoList where xxx.Name == old_name select xxx).FirstOrDefault();
            if (fi != null)
                fi.Name = new_name;
        }
    }
}

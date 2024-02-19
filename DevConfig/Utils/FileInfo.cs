namespace DevConfig
{
    public class FileInfo
    {
        public string Name;
        public DateTime ModifyTime;
        public bool IsDirectory;
        public uint Size;

        public FileInfo(string name)
        {
            Name = name;
        }

        public FileInfo(FileInfo fi)
        {
            Name = fi.Name;
            ModifyTime = fi.ModifyTime;
            IsDirectory = fi.IsDirectory;
            Size = fi.Size;
        }
    }
}

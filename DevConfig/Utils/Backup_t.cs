using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DevConfig
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    public class Backup_t
    {
        [XmlIgnore] public uint dir_cnt = 0;
        [XmlIgnore] public uint file_cnt = 0;
        [XmlIgnore] public ulong size = 0;
        [XmlIgnore] public List<FileInfo> files = new();

        public string[] exclude_ext = new string[] { ".bin" };
        public string backup_destination = string.Empty;


        public void Save()
        {
            var ser = new XmlSerializer(typeof(Backup_t));
            using var fs = new FileStream(Path.Combine(Path.GetTempPath(), "Backup_t.xml"), FileMode.OpenOrCreate | FileMode.Truncate);
            using var tw = new StreamWriter(fs, new UTF8Encoding());
            var ns = new XmlSerializerNamespaces(); ns.Add("", "");
            ser.Serialize(tw, this, ns);
            tw.Close(); tw.Dispose();
            fs.Close(); fs.Dispose();
        }

        public static Backup_t Load()
        {
            XmlSerializer ser = new (typeof(Backup_t));
            Backup_t? settings;

            try
            {
                using FileStream s = File.OpenRead(Path.Combine(Path.GetTempPath(), "Backup_t.xml"));
                settings = (Backup_t?)ser.Deserialize(s);
                s.Close(); s.Dispose();
            }
            catch// (Exception ex1)
            {
                //MessageBox.Show("Cannot read settings. " + ex1.Message, "error");
                settings = new Backup_t();
            }
            return settings!;
        }
    };
}

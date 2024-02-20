using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace DevConfig
{
    ///////////////////////////////////////////////////////////////////////////////////////////
    public class Backup_t
    {
        [XmlIgnore] internal uint dir_cnt = 0;
        [XmlIgnore] internal uint file_cnt = 0;
        [XmlIgnore] internal ulong size = 0;
        [XmlIgnore] internal List<FileInfo> files = new();
        [XmlIgnore] internal bool restore;

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

        public static Backup_t Load(bool b_restore)
        {
            XmlSerializer ser = new (typeof(Backup_t));
            Backup_t? settings;

            try
            {
                using FileStream s = File.OpenRead(Path.Combine(Path.GetTempPath(), "Backup_t.xml"));
                settings = (Backup_t?)ser.Deserialize(s);
                s.Close(); s.Dispose();
            }
            catch
            {
                settings = new Backup_t();
            }
            settings!.restore = b_restore;
            return settings!;
        }
    };
}

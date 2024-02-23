using static System.Windows.Forms.DataFormats;

namespace DevConfig.Service
{
    public class Device
    {
        public byte Address { get; set; }
        public string AddressStr { get { return $"{Address:X2}"; } }
        public uint DevId { get; set; }
        public string DevIdStr { get { return $"{DevId:X}"; } }

        public string? Name;// { get; set; }
        public string? FwVer;// { get; set; }
        public string? CpuId;// { get; set; }

        internal List<Parameter>? Parameters = null;

        public ListViewItem? listViewItem = null;
    }
}

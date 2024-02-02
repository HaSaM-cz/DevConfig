using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace RGB_config
{
    public class cGlobals
    {
        public static List<cDeviceConfig> listDevices = new List<cDeviceConfig>();
        public static List<cModesConfig> listModes = new List<cModesConfig>();
        public static List<cStatesConfig> listStates = new List<cStatesConfig>();

        public static TreeView MainTreeView;
//        public static Form1 mainForm = null;

        public enum te_segm_modes
        {
            mode_led_off,
            mode_led_on,
            mode_led_on_plus_white,
            mode_led_toogle,
            mode_snake,
            mode_snake_reverse,
            mode_snake_magic_eye_add,
            mode_snake_magic_eye_one_led,
            mode_duty_min_to_max,
            mode_duty_max_to_min,
            mode_snake_inverse,             // black snake in light
            mode_snake_inverse_reverse,	// black snake in light move reverse
        }

        public class cDeviceConfig
        {
            public byte address;
            public UInt16 line1_Offset;
            public UInt16 line2_Offset;
            public UInt16[] map0 = new UInt16[500];
            public UInt16[] map1 = new UInt16[500];

            public void InitFromXml(XmlNode xn)
            {
                foreach (XmlAttribute xnn in xn.Attributes)
                {
                    switch (xnn.Name)
                    {
                        case "deviceAddress":
                            this.address = StringToByte(xnn.Value);
                            break;
                        case "line1_Offset":
                            this.line1_Offset = (UInt16)StringToInt(xnn.Value);
                            break;
                        case "line2_Offset":
                            this.line2_Offset = (UInt16)StringToInt(xnn.Value);
                            break;
                        case "led_pixel_map0":
                            string pixels_map = xnn.Value;
                            string[] pixel_seqm = pixels_map.Split(';');

                            UInt16 map_ptr = 0;

                            foreach (string str in pixel_seqm)
                            {
                                string[] from_to = str.Split('-');

                                UInt16 from = (UInt16)StringToInt(from_to[0]);
                                UInt16 to = (UInt16)StringToInt(from_to[1]);

                                if (from < to)
                                {
                                    for (UInt16 i = from; i <= to; i++)
                                    {
                                        this.map0[map_ptr++] = i;
                                    }
                                }
                                else
                                {
                                    for (UInt16 i = from; i >= to; i--)
                                    {
                                        this.map0[map_ptr++] = i;
                                    }
                                }
                            }

                            break;


                    }
                }
            }
        }

        public class cModesConfig
        {
            public byte id;
            public string name;
            public UInt64 auxiliary;
            public te_segm_modes mode;
            public UInt16 step_time1ms;
            public byte max_RED;
            public byte min_RED;
            public byte max_GREEN;
            public byte min_GREEN;
            public byte max_BLUE;
            public byte min_BLUE;

            public void InitFromXml(XmlNode xn)
            {
                foreach (XmlAttribute xnn in xn.Attributes)
                {
                    switch (xnn.Name)
                    {
                        case "id":
                            this.id = StringToByte(xnn.Value);
                            break;
                        case "name":
                            this.name = xnn.Value;
                            break;
                        case "auxiliary":
                            this.auxiliary = (UInt64)StringToUint64(xnn.Value);
                            break;
                        case "stepTime":
                            this.step_time1ms = (UInt16)StringToInt(xnn.Value);
                            break;
                        case "redMax":
                            this.max_RED = StringToByte(xnn.Value);
                            break;
                        case "redMin":
                            this.min_RED = StringToByte(xnn.Value);
                            break;
                        case "greenMax":
                            this.max_GREEN = StringToByte(xnn.Value);
                            break;
                        case "greenMin":
                            this.min_GREEN = StringToByte(xnn.Value);
                            break;
                        case "blueMax":
                            this.max_BLUE = StringToByte(xnn.Value);
                            break;
                        case "blueMin":
                            this.min_BLUE = StringToByte(xnn.Value);
                            break;
                        case "mode":
                            switch (xnn.Value)
                            {
                                case "LED_OFF":
                                    this.mode = te_segm_modes.mode_led_off;
                                    break;
                                case "LED_ON":
                                    this.mode = te_segm_modes.mode_led_on;
                                    break;
                                case "LED_ON_W":
                                    this.mode = te_segm_modes.mode_led_on_plus_white;
                                    break;
                                case "LED_TGL":
                                    this.mode = te_segm_modes.mode_led_toogle;
                                    break;
                                case "DUTY_MAX_MIN":
                                    this.mode = te_segm_modes.mode_duty_max_to_min;
                                    break;
                                case "DUTY_MIN_MAX":
                                    this.mode = te_segm_modes.mode_duty_min_to_max;
                                    break;
                                case "SNAKE":
                                    this.mode = te_segm_modes.mode_snake;
                                    break;
                                case "SNAKE_REV":
                                    this.mode = te_segm_modes.mode_snake_reverse;
                                    break;
                                case "EYE_ADD":
                                    this.mode = te_segm_modes.mode_snake_magic_eye_add;
                                    break;
                                case "EYE_ONE_LED":
                                    this.mode = te_segm_modes.mode_snake_magic_eye_one_led;
                                    break;
                                case "SNAKE_INV":
                                    this.mode = te_segm_modes.mode_snake_inverse;
                                    break;
                                case "SNAKE_INV_REV":
                                    this.mode = te_segm_modes.mode_snake_inverse_reverse;
                                    break;
                            }
                            break;

                    }
                }
            }
        }

        public struct ts_order_data
        {
            public UInt16 order_time;
            public byte order_repeat;
            public byte order_mode;
        }

        public struct ts_segments
        {
            public string segm_name;
            public UInt16 pixels;
            public List<ts_order_data> order_data;
        }

        public class cStatesConfig
        {
            public byte stateID;
            public string stateName;

            public List<cSubStatesConfig> list_SubStates = new List<cSubStatesConfig>();

            public void InitFromXml(XmlNode xn)
            {
                list_SubStates.Clear();

                foreach (XmlAttribute xsa in xn.Attributes)
                {
                    switch (xsa.Name)
                    {
                        case "id":
                            this.stateID = StringToByte(xsa.Value);
                            break;
                        case "name":
                            this.stateName = xsa.Value;
                            break;
                    }
                }

                foreach (XmlNode xns in xn.ChildNodes)
                {
                    cSubStatesConfig newSubState = new cSubStatesConfig();
                    newSubState.InitFromXml(xns);
                    list_SubStates.Add(newSubState);
                }
            }
        }

        public class cSubStatesConfig
        {
            public byte substateID;
            public string substateName;
            public byte SyncStateEnable = 0;
            public byte SyncStateTimer = 0;

            public List<ts_segments> list_segment = new List<ts_segments>();

            private ts_segments InitSequenceFromXML(XmlNode xns)
            {
                ts_segments sList = new ts_segments();
                sList.order_data = new List<ts_order_data>();

                foreach (XmlNode xnseq in xns.ChildNodes)
                {
                    if (xnseq.Name == "sequence")
                    {
                        ts_order_data order_data = new ts_order_data();
                        foreach (XmlAttribute xaseq in xnseq.Attributes)
                        {
                            switch (xaseq.Name)
                            {
                                case "time":
                                    order_data.order_time = (UInt16)StringToInt(xaseq.Value);
                                    break;
                                case "repetion":
                                    order_data.order_repeat = StringToByte(xaseq.Value);
                                    break;
                                case "mode":
                                    order_data.order_mode = StringToByte(xaseq.Value);
                                    break;
                            }
                        }
                        sList.order_data.Add(order_data);
                    }
                }
                return sList;
            }

            private void IniSegmentFromXML(XmlNode xsegm)
            {
                foreach (XmlNode xns in xsegm.ChildNodes)
                {
                    if (xns.Name == "Segment")
                    {
                        ts_segments sList = new ts_segments();
                        sList.order_data = new List<ts_order_data>();

                        sList = InitSequenceFromXML(xns);

                        foreach (XmlAttribute xasegm in xns.Attributes)
                        {
                            switch (xasegm.Name)
                            {
                                case "name":
                                    sList.segm_name = xasegm.Value;
                                    break;
                                case "pixels":
                                    sList.pixels = (UInt16)StringToInt(xasegm.Value);
                                    break;
                            }
                        }
                        list_segment.Add(sList);
                    }
                }
            }

            public void InitFromXml(XmlNode xn)
            {
                list_segment.Clear();

                foreach (XmlAttribute xsa in xn.Attributes)
                {
                    switch (xsa.Name)
                    {
                        case "id":
                            this.substateID = StringToByte(xsa.Value);
                            break;
                        case "name":
                            this.substateName = xsa.Value;
                            break;
                        case "SyncStateEnable":
                            this.SyncStateEnable = StringToByte(xsa.Value);
                            break;
                        case "SyncStateTimer":
                            this.SyncStateTimer = StringToByte(xsa.Value);
                            break;
                    }
                }

                IniSegmentFromXML(xn);
            }
        }

        public static byte StringToByte(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Replace("0x", "");
                return Convert.ToByte(str, 16);
            }
            else
            {
                return Convert.ToByte(str, 10);
            }
        }

        public static int StringToInt(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Replace("0x", "");
                return Convert.ToInt32(str, 16);
            }
            else
            {
                return Convert.ToInt32(str, 10);
            }
        }

        public static UInt64 StringToUint64(string str)
        {
            if (str.StartsWith("0x"))
            {
                str = str.Replace("0x", "");
                return Convert.ToUInt64(str, 16);
            }
            else
            {
                return Convert.ToUInt64(str, 10);
            }
        }

        private void loadXMLFile(XmlElement xe)
        {
            listDevices.Clear();
            listModes.Clear();
            listStates.Clear();

            XmlNode xn = xe.SelectSingleNode("Devices");

            if (xn != null)
            {
                foreach (XmlNode xnd in xn.ChildNodes)
                {
                    if (xnd.Name == "Device")
                    {
                        cDeviceConfig newDevice = new cDeviceConfig();
                        newDevice.InitFromXml(xnd);
                        listDevices.Add(newDevice);
                    }
                }
            }

            xn = xe.SelectSingleNode("Modes");

            if (xn != null)
            {
                foreach (XmlNode xnn in xn.ChildNodes)
                {
                    if (xnn.Name == "Mode")
                    {
                        cModesConfig newmode = new cModesConfig();
                        newmode.InitFromXml(xnn);
                        listModes.Add(newmode);
                    }
                }
            }

            xn = xe.SelectSingleNode("States");

            if (xn != null)
            {
                foreach (XmlNode xnn in xn.ChildNodes)
                {
                    if (xnn.Name == "State")
                    {
                        cStatesConfig newstate = new cStatesConfig();
                        newstate.InitFromXml(xnn);
                        listStates.Add(newstate);
                    }
                }
            }
        }

        public void OpenXmlFile()
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.Filter = "xml file (*.xml)|*.xml";
                ofd.ShowDialog();
                if (ofd.FileName == "")
                    return;

                XmlDocument xd = new XmlDocument();
                xd.Load(ofd.FileName);

                loadXMLFile(xd.DocumentElement);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Open + parse error\r\nError: " + ex.Message);
            }
        }


        public static void treevieRemoveMode(byte mode)
        {
            try
            {
                MainTreeView.Nodes[1].Nodes[mode].Remove();
            }
            catch
            { }
        }

        public static void treevieRemoveDevice(byte device)
        {
            try
            {
                MainTreeView.Nodes[0].Nodes[device].Remove();
            }
            catch
            { }
        }


        public static void treevieSaveMode(byte mode)
        {
            try
            {
                MainTreeView.Nodes[1].Nodes[mode].Text = listModes[mode].id.ToString() + "-" + listModes[mode].name;
            }
            catch
            { }
        }

        public static void treevieSaveDevice(byte device)
        {
            try
            {
                MainTreeView.Nodes[0].Nodes[device].Text = "dev 0x" + listDevices[device].address.ToString("X2");
            }
            catch
            { }
        }

        public static void treevieAddMode(cModesConfig newMode)
        {
            try
            {
                MainTreeView.Nodes[1].Nodes.Add(newMode.id.ToString() + "-" + newMode.name);
            }
            catch
            { }
        }

        public static void treevieAddDevice(cDeviceConfig newDevice)
        {
            try
            {
                MainTreeView.Nodes[0].Nodes.Add("dev 0x" + newDevice.address.ToString("X2"));
            }
            catch
            { }
        }

        public static void treevieAddSegment(byte stateID, byte substateID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes[stateID].Nodes[substateID].Nodes.Add(listStates[stateID].list_SubStates[substateID].list_segment[listStates[stateID].list_SubStates[substateID].list_segment.Count - 1].segm_name);
            }
            catch
            { }
        }

        public static void treevieSaveSegment(byte stateID, byte substateID, byte segmentID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes[stateID].Nodes[substateID].Nodes[segmentID].Text = listStates[stateID].list_SubStates[substateID].list_segment[segmentID].segm_name;
            }
            catch
            { }
        }

        public static void treevieDeleteSegment(byte stateID, byte substateID, byte segmentID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes[stateID].Nodes[substateID].Nodes.RemoveAt(segmentID);
            }
            catch
            { }
        }

        public static void treevieAddState(byte stateID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes.Add(listStates[stateID].stateID.ToString() + "-" + listStates[stateID].stateName);
            }
            catch
            { }
        }

        public static void treevieAddSubState(byte stateID, byte substateID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes[stateID].Nodes.Add(listStates[stateID].list_SubStates[substateID].substateID.ToString() + "-" + listStates[stateID].list_SubStates[substateID].substateName);
            }
            catch
            { }
        }

        public static void treevieDeleteState(byte stateID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes.RemoveAt(stateID);
            }
            catch
            { }
        }

        public static void treevieSaveState(byte stateID)
        {
            try
            {
                MainTreeView.Nodes[2].Nodes[stateID].Text = listStates[stateID].stateID.ToString() + "-" + listStates[stateID].stateName;
            }
            catch
            { }
        }


        public static void RefreshMainTreeView()
        {
            try
            {
                MainTreeView.Nodes.Clear();

                TreeNode devices = MainTreeView.Nodes.Add("Devices");
                foreach (cDeviceConfig device in listDevices)
                {
                    TreeNode tnd = devices.Nodes.Add("dev 0x" + device.address.ToString("X2"));
                }

                TreeNode modes = MainTreeView.Nodes.Add("Modes");
                foreach (cModesConfig mode in listModes)
                {
                    TreeNode md = modes.Nodes.Add(mode.id.ToString() + "-" + mode.name);
                    //md.Tag = mode;
                }

                TreeNode states = MainTreeView.Nodes.Add("States");
                foreach (cStatesConfig state in listStates)
                {
                    TreeNode nodeState = states.Nodes.Add(state.stateID.ToString() + "-" + state.stateName);

                    foreach (cSubStatesConfig substate in state.list_SubStates)
                    {
                        TreeNode nodeSubState = nodeState.Nodes.Add(substate.substateID.ToString() + "-" + substate.substateName);
                        //nodeState.Tag = state;

                        foreach (ts_segments segment in substate.list_segment)
                        {
                            TreeNode nodesegment = nodeSubState.Nodes.Add(segment.segm_name);
                        }
                    }
                }
            }
            catch { }
        }

        public static XmlDocument GetWholeConfigAsXmlDoc()
        {
            XmlDocument xd = new XmlDocument();

            XmlElement xeConfig = xd.CreateElement("Config");
            xd.AppendChild(xeConfig);

            //--------------------------------------------------  Pixels ------------------------------------
            XmlElement xeDevices = xd.CreateElement("Devices");
            xeConfig.AppendChild(xeDevices);

            foreach (cDeviceConfig device in listDevices)
            {
                XmlElement xeDevice = xd.CreateElement("Device");
                xeDevices.AppendChild(xeDevice);
                xeDevice.SetAttribute("deviceAddress", "0x" + device.address.ToString("X2"));
                xeDevice.SetAttribute("line1_Offset", device.line1_Offset.ToString());
                xeDevice.SetAttribute("line2_Offset", device.line2_Offset.ToString());
            }
            //--------------------------------------------------  Modes ------------------------------------
            XmlElement xeModes = xd.CreateElement("Modes");
            xeConfig.AppendChild(xeModes);

            foreach (cModesConfig mode in listModes)
            {
                XmlElement xeMode = xd.CreateElement("Mode");
                xeModes.AppendChild(xeMode);

                xeMode.SetAttribute("id", mode.id.ToString());
                xeMode.SetAttribute("name", mode.name);
                xeMode.SetAttribute("auxiliary", "0x" + mode.auxiliary.ToString("X"));

                switch (mode.mode)
                {
                    case te_segm_modes.mode_led_off:
                        xeMode.SetAttribute("mode", "LED_OFF");
                        break;
                    case te_segm_modes.mode_led_on:
                        xeMode.SetAttribute("mode", "LED_ON");
                        break;
                    case te_segm_modes.mode_led_on_plus_white:
                        xeMode.SetAttribute("mode", "LED_ON_W");
                        break;
                    case te_segm_modes.mode_led_toogle:
                        xeMode.SetAttribute("mode", "LED_TGL");
                        break;
                    case te_segm_modes.mode_duty_max_to_min:
                        xeMode.SetAttribute("mode", "DUTY_MAX_MIN");
                        break;
                    case te_segm_modes.mode_duty_min_to_max:
                        xeMode.SetAttribute("mode", "DUTY_MIN_MAX");
                        break;
                    case te_segm_modes.mode_snake:
                        xeMode.SetAttribute("mode", "SNAKE");
                        break;
                    case te_segm_modes.mode_snake_reverse:
                        xeMode.SetAttribute("mode", "SNAKE_REV");
                        break;
                    case te_segm_modes.mode_snake_magic_eye_add:
                        xeMode.SetAttribute("mode", "EYE_ADD");
                        break;
                    case te_segm_modes.mode_snake_magic_eye_one_led:
                        xeMode.SetAttribute("mode", "EYE_ONE_LED");
                        break;
                    case te_segm_modes.mode_snake_inverse:
                        xeMode.SetAttribute("mode", "SNAKE_INV");
                        break;
                    case te_segm_modes.mode_snake_inverse_reverse:
                        xeMode.SetAttribute("mode", "SNAKE_INV_REV");
                        break;
                }

                xeMode.SetAttribute("stepTime", mode.step_time1ms.ToString());
                xeMode.SetAttribute("redMax", mode.max_RED.ToString());
                xeMode.SetAttribute("redMin", mode.min_RED.ToString());
                xeMode.SetAttribute("greenMax", mode.max_GREEN.ToString());
                xeMode.SetAttribute("greenMin", mode.min_GREEN.ToString());
                xeMode.SetAttribute("blueMax", mode.max_BLUE.ToString());
                xeMode.SetAttribute("blueMin", mode.min_BLUE.ToString());


            }

            //--------------------------------------------------  States ------------------------------------
            XmlElement xeStates = xd.CreateElement("States");
            xeConfig.AppendChild(xeStates);

            foreach (cStatesConfig state in listStates)
            {
                XmlElement xeState = xd.CreateElement("State");
                xeStates.AppendChild(xeState);

                xeState.SetAttribute("id", state.stateID.ToString());
                xeState.SetAttribute("name", state.stateName);

                foreach (cSubStatesConfig substate in state.list_SubStates)
                {
                    XmlElement xeSubState = xd.CreateElement("SubState");
                    xeState.AppendChild(xeSubState);

                    xeSubState.SetAttribute("id", substate.substateID.ToString());
                    xeSubState.SetAttribute("name", substate.substateName);
                    xeSubState.SetAttribute("SyncStateEnable", substate.SyncStateEnable.ToString());
                    xeSubState.SetAttribute("SyncStateTimer", substate.SyncStateTimer.ToString());

                    foreach (ts_segments segm in substate.list_segment)
                    {
                        XmlElement xeSegment = xd.CreateElement("Segment");
                        xeSubState.AppendChild(xeSegment);

                        xeSegment.SetAttribute("name", segm.segm_name);
                        xeSegment.SetAttribute("pixels", segm.pixels.ToString());

                        foreach (ts_order_data order_data in segm.order_data)
                        {
                            XmlElement xeSequence = xd.CreateElement("sequence");
                            xeSegment.AppendChild(xeSequence);

                            xeSequence.SetAttribute("time", order_data.order_time.ToString());
                            xeSequence.SetAttribute("repetion", order_data.order_repeat.ToString());
                            xeSequence.SetAttribute("mode", order_data.order_mode.ToString());
                        }
                    }
                }
            }


            return xd;
        }

        public static void SaveConfigToFile(string FileName)
        {
            GetWholeConfigAsXmlDoc().Save(FileName);
        }


        public static class crc
        {
            static readonly byte[] Crc8Tab = new byte[]
            {
                0x00, 0x5e, 0xbc, 0xe2, 0x61, 0x3f, 0xdd, 0x83,
                0xc2, 0x9c, 0x7e, 0x20, 0xa3, 0xfd, 0x1f, 0x41,
                0x9d, 0xc3, 0x21, 0x7f, 0xfc, 0xa2, 0x40, 0x1e,
                0x5f, 0x01, 0xe3, 0xbd, 0x3e, 0x60, 0x82, 0xdc,
                0x23, 0x7d, 0x9f, 0xc1, 0x42, 0x1c, 0xfe, 0xa0,
                0xe1, 0xbf, 0x5d, 0x03, 0x80, 0xde, 0x3c, 0x62,
                0xbe, 0xe0, 0x02, 0x5c, 0xdf, 0x81, 0x63, 0x3d,
                0x7c, 0x22, 0xc0, 0x9e, 0x1d, 0x43, 0xa1, 0xff,
                0x46, 0x18, 0xfa, 0xa4, 0x27, 0x79, 0x9b, 0xc5,
                0x84, 0xda, 0x38, 0x66, 0xe5, 0xbb, 0x59, 0x07,
                0xdb, 0x85, 0x67, 0x39, 0xba, 0xe4, 0x06, 0x58,
                0x19, 0x47, 0xa5, 0xfb, 0x78, 0x26, 0xc4, 0x9a,
                0x65, 0x3b, 0xd9, 0x87, 0x04, 0x5a, 0xb8, 0xe6,
                0xa7, 0xf9, 0x1b, 0x45, 0xc6, 0x98, 0x7a, 0x24,
                0xf8, 0xa6, 0x44, 0x1a, 0x99, 0xc7, 0x25, 0x7b,
                0x3a, 0x64, 0x86, 0xd8, 0x5b, 0x05, 0xe7, 0xb9,
                0x8c, 0xd2, 0x30, 0x6e, 0xed, 0xb3, 0x51, 0x0f,
                0x4e, 0x10, 0xf2, 0xac, 0x2f, 0x71, 0x93, 0xcd,
                0x11, 0x4f, 0xad, 0xf3, 0x70, 0x2e, 0xcc, 0x92,
                0xd3, 0x8d, 0x6f, 0x31, 0xb2, 0xec, 0x0e, 0x50,
                0xaf, 0xf1, 0x13, 0x4d, 0xce, 0x90, 0x72, 0x2c,
                0x6d, 0x33, 0xd1, 0x8f, 0x0c, 0x52, 0xb0, 0xee,
                0x32, 0x6c, 0x8e, 0xd0, 0x53, 0x0d, 0xef, 0xb1,
                0xf0, 0xae, 0x4c, 0x12, 0x91, 0xcf, 0x2d, 0x73,
                0xca, 0x94, 0x76, 0x28, 0xab, 0xf5, 0x17, 0x49,
                0x08, 0x56, 0xb4, 0xea, 0x69, 0x37, 0xd5, 0x8b,
                0x57, 0x09, 0xeb, 0xb5, 0x36, 0x68, 0x8a, 0xd4,
                0x95, 0xcb, 0x29, 0x77, 0xf4, 0xaa, 0x48, 0x16,
                0xe9, 0xb7, 0x55, 0x0b, 0x88, 0xd6, 0x34, 0x6a,
                0x2b, 0x75, 0x97, 0xc9, 0x4a, 0x14, 0xf6, 0xa8,
                0x74, 0x2a, 0xc8, 0x96, 0x15, 0x4b, 0xa9, 0xf7,
                0xb6, 0xe8, 0x0a, 0x54, 0xd7, 0x89, 0x6b, 0x35
            };

            static readonly ushort[] Crc16Tab = new ushort[]
            {
                0x0000, 0x1189, 0x2312, 0x329B, 0x4624, 0x57AD, 0x6536, 0x74BF,
                0x8C48, 0x9DC1, 0xAF5A, 0xBED3, 0xCA6C, 0xDBE5, 0xE97E, 0xF8F7,
                0x1081, 0x0108, 0x3393, 0x221A, 0x56A5, 0x472C, 0x75B7, 0x643E,
                0x9CC9, 0x8D40, 0xBFDB, 0xAE52, 0xDAED, 0xCB64, 0xF9FF, 0xE876,
                0x2102, 0x308B, 0x0210, 0x1399, 0x6726, 0x76AF, 0x4434, 0x55BD,
                0xAD4A, 0xBCC3, 0x8E58, 0x9FD1, 0xEB6E, 0xFAE7, 0xC87C, 0xD9F5,
                0x3183, 0x200A, 0x1291, 0x0318, 0x77A7, 0x662E, 0x54B5, 0x453C,
                0xBDCB, 0xAC42, 0x9ED9, 0x8F50, 0xFBEF, 0xEA66, 0xD8FD, 0xC974,
                0x4204, 0x538D, 0x6116, 0x709F, 0x0420, 0x15A9, 0x2732, 0x36BB,
                0xCE4C, 0xDFC5, 0xED5E, 0xFCD7, 0x8868, 0x99E1, 0xAB7A, 0xBAF3,
                0x5285, 0x430C, 0x7197, 0x601E, 0x14A1, 0x0528, 0x37B3, 0x263A,
                0xDECD, 0xCF44, 0xFDDF, 0xEC56, 0x98E9, 0x8960, 0xBBFB, 0xAA72,
                0x6306, 0x728F, 0x4014, 0x519D, 0x2522, 0x34AB, 0x0630, 0x17B9,
                0xEF4E, 0xFEC7, 0xCC5C, 0xDDD5, 0xA96A, 0xB8E3, 0x8A78, 0x9BF1,
                0x7387, 0x620E, 0x5095, 0x411C, 0x35A3, 0x242A, 0x16B1, 0x0738,
                0xFFCF, 0xEE46, 0xDCDD, 0xCD54, 0xB9EB, 0xA862, 0x9AF9, 0x8B70,
                0x8408, 0x9581, 0xA71A, 0xB693, 0xC22C, 0xD3A5, 0xE13E, 0xF0B7,
                0x0840, 0x19C9, 0x2B52, 0x3ADB, 0x4E64, 0x5FED, 0x6D76, 0x7CFF,
                0x9489, 0x8500, 0xB79B, 0xA612, 0xD2AD, 0xC324, 0xF1BF, 0xE036,
                0x18C1, 0x0948, 0x3BD3, 0x2A5A, 0x5EE5, 0x4F6C, 0x7DF7, 0x6C7E,
                0xA50A, 0xB483, 0x8618, 0x9791, 0xE32E, 0xF2A7, 0xC03C, 0xD1B5,
                0x2942, 0x38CB, 0x0A50, 0x1BD9, 0x6F66, 0x7EEF, 0x4C74, 0x5DFD,
                0xB58B, 0xA402, 0x9699, 0x8710, 0xF3AF, 0xE226, 0xD0BD, 0xC134,
                0x39C3, 0x284A, 0x1AD1, 0x0B58, 0x7FE7, 0x6E6E, 0x5CF5, 0x4D7C,
                0xC60C, 0xD785, 0xE51E, 0xF497, 0x8028, 0x91A1, 0xA33A, 0xB2B3,
                0x4A44, 0x5BCD, 0x6956, 0x78DF, 0x0C60, 0x1DE9, 0x2F72, 0x3EFB,
                0xD68D, 0xC704, 0xF59F, 0xE416, 0x90A9, 0x8120, 0xB3BB, 0xA232,
                0x5AC5, 0x4B4C, 0x79D7, 0x685E, 0x1CE1, 0x0D68, 0x3FF3, 0x2E7A,
                0xE70E, 0xF687, 0xC41C, 0xD595, 0xA12A, 0xB0A3, 0x8238, 0x93B1,
                0x6B46, 0x7ACF, 0x4854, 0x59DD, 0x2D62, 0x3CEB, 0x0E70, 0x1FF9,
                0xF78F, 0xE606, 0xD49D, 0xC514, 0xB1AB, 0xA022, 0x92B9, 0x8330,
                0x7BC7, 0x6A4E, 0x58D5, 0x495C, 0x3DE3, 0x2C6A, 0x1EF1, 0x0F78
            };



            public static ushort GetCRC16(byte[] ValArray)
            {
                return GetCRC16(ValArray, ValArray.Length);
            }

            public static ushort GetCRC16(byte[] ValArray, int wLen)
            {
                return GetCRC16(ValArray, 0, wLen);
            }

            public static ushort GetCRC16(byte[] ValArray, int wLen, ushort startCrc)
            {
                ushort wCRC = startCrc;

                for (int i = 0; i < wLen; wCRC = (ushort)((wCRC >> 8) ^ Crc16Tab[(byte)wCRC ^ ValArray[i++]])) ;
                return wCRC;
            }

            public static ushort GetCRC16(byte[] ValArray, int StartIndex, int wLen)
            {
                ushort wCRC = 0;

                for (int i = StartIndex; i < wLen + StartIndex; wCRC = (ushort)((wCRC >> 8) ^ Crc16Tab[(byte)wCRC ^ ValArray[i++]])) ;
                return wCRC;
            }

            public static byte GetCRC8(byte[] ValArray, int StartIndex, int Len)
            {
                byte CRC = 0;

                for (int i = StartIndex; i < Len + StartIndex; CRC = (byte)((CRC >> 8) ^ Crc8Tab[(byte)CRC ^ ValArray[i++]])) ;
                return CRC;
            }

            public static byte GetCRC8(byte[] ValArray, int Len)
            {
                byte CRC = 0;

                for (int i = 0; i < Len; CRC = (byte)((CRC >> 8) ^ Crc8Tab[(byte)CRC ^ ValArray[i++]])) ;
                return CRC;
            }
        }

        public class cCanLongMsg
        {
            private ushort crc16;

            public uint ID;

            public byte[] data;
            public byte dataLen;
            public byte dataIdx;

            public DateTime firstPackeTime = DateTime.Now;


            public const byte ECAN_MsgFlags_LongMessage = 0x10;
            public const byte ECAN_MsgFlags_LongMessageFirstPacket = 0x08;
            public const byte ECAN_MsgFlags_IsResponse = 0x02;



            public cCanLongMsg()
            {
            }

            public cCanLongMsg(UInt32 id, byte[] msg_data, byte dlen)
            {
                if (dlen < 5)
                    return;

                //b[0] = 0;   //RFU
                //b[1] = (byte)(buf.Length);
                //b[2] = (byte)(crc16 >> 8);
                //b[3] = (byte)(crc16);

                ID = id;
                dataLen = msg_data[1];
                crc16 = (ushort)(msg_data[2] * 256 + msg_data[3]);
                dataIdx = 0;

                dlen -= 4;
                dataIdx += dlen;

                data = new byte[dataLen];
                Array.Copy(msg_data, 4, data, 0, dlen);
            }

            public bool putNextMessage(UInt32 id, byte[] msg_data, byte dlen)
            {

                Array.Copy(msg_data, 0, data, dataIdx, dlen);
                dataIdx += dlen;

                if (dataIdx < dataLen)
                    return false;           //not whole message yet

                UInt16 crc16d = crc.GetCRC16(data);
                if (crc16d != crc16)
                {
                    //throw new Exception("CAN long message CRC ERRROR");
                    //Console.Write("CAN long message CRC ERRROR");
                    // TODO cGlobals.mainForm.AppendToDebug("CAN long message CRC ERRROR");
                    return false;
                }
                return true;
            }


            public override string ToString()
            {

                string s = "";
                char[] cs = Encoding.ASCII.GetChars(data);
                for (int i = 0; i < data.Length; i++)
                {
                    if ((data[i] < 0x20) | (data[i] > 0x7F))
                        s += "_";
                    else
                        s += cs[i];
                }
                return "<" + DateTime.Now.ToString("mm:ss") + "<CAN ID:" + ID.ToString("X8") + " D:" + GetHexArrAsString(data) + "\r\n\"" + s + "\"";

                //return base.ToString();
            }

            private string GetHexArrAsString(byte[] arr)
            {

                string s = "";
                for (int i = 0; i < arr.Length; i++)
                {
                    s += arr[i].ToString("X2") + ",";
                }
                s.TrimEnd(',');

                return s;
            }
        }
    }
}

using System.Configuration;

namespace DevConfig.Utils
{
    public static class Extension
    {
        public static void Add<T>(this ApplicationSettingsBase settings, string propertyName, T val)
        {

            bool itemExists = false;
            foreach (SettingsProperty property in settings.Properties)
            {
                if (property.Name == propertyName)
                {
                    itemExists = true;
                    break;
                }
            }
            if (!itemExists)
            {
                var p = new SettingsProperty(propertyName)
                {
                    PropertyType = typeof(T),
                    Provider = settings.Providers["LocalFileSettingsProvider"],
                    SerializeAs = SettingsSerializeAs.Xml
                };

                p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());

                settings.Properties.Add(p);
                settings.Reload();
            }

            settings[propertyName] = val;
            settings.Save();
        }

        public static T Get<T>(this ApplicationSettingsBase settings, string propertyName, T defaultValue)
        {
            bool itemExists = false;
            foreach (SettingsProperty property in settings.Properties)
            {
                if (property.Name == propertyName)
                {
                    itemExists = true;
                    break;
                }
            }
            if (!itemExists)
            {
                var p = new SettingsProperty(propertyName)
                {
                    PropertyType = typeof(T),
                    Provider = settings.Providers["LocalFileSettingsProvider"],
                    SerializeAs = SettingsSerializeAs.Xml
                };

                p.Attributes.Add(typeof(UserScopedSettingAttribute), new UserScopedSettingAttribute());

                settings.Properties.Add(p);
                settings.Reload();
            }
            //finally set value with new value if none was loaded from userConfig.xml
            var item = settings[propertyName];
            if (item == null)
            {
                settings[propertyName] = defaultValue;
                settings.Save();
            }

            return (T)settings[propertyName];
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string ToHumanSting(this ulong size) => ToHumanSting((double)size);

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static string ToHumanSting(this double size)
        {
            string[] suffix = new string[] { "", "k", "M", "G", "T" };

            int si = 0;
            while (size > 1024 && si < suffix.Length)
            {
                si++;
                size = size / 1024;
            }

            return $"{size:0.##}{suffix[si]}";
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////
        public static uint ToUInt32(this string str)
        {
            return str.StartsWith("0x") ? Convert.ToUInt32(str.Substring("0x".Length), 16) : Convert.ToUInt32(str);
        }
    }
}

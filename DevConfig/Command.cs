using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevConfig
{
    internal class Command
    {
        internal const byte Ident = 0x02;
        internal const byte ParamRead = 0x48;
        internal const byte GetListParam = 0x49;
        internal const byte StartUpdate = 0x50;
        internal const byte UpdateMsg = 0x51;
    }
}

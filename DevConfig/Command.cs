﻿namespace DevConfig
{
    internal class Command
    {
        internal const byte Ident = 0x02;
        internal const byte Reset = 0x5F;
        internal const byte ParamRead = 0x48;
        internal const byte GetListParam = 0x49;
        internal const byte StartUpdate = 0x50;
        internal const byte UpdateMsg = 0x51;
    }
}

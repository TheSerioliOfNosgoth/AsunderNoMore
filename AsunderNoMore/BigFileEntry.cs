using System;
using System.Runtime.InteropServices;

namespace AsunderNoMore
{
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct BigfileCode
    {
        [FieldOffset(0)] public char code0;
        [FieldOffset(1)] public char code1;
        [FieldOffset(2)] public char code2;
        [FieldOffset(3)] public char code3;
        [FieldOffset(0)] public uint code;
    }

    class BigfileEntry
    {
        public string FilePath;
        public uint FileHash;
        public uint FileLength;
        public BigfileCode FileCode;
    }
}
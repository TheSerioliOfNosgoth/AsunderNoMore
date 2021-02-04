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

    class AssetDesc
    {
        public string FilePath { get; set; } = "";
        public uint FileHash { get; set; } = 0;
        public uint FileLength { get; set; } = 0;
        public uint FileOffset { get; set; } = 0;
        public uint FileCode { get { return Code.code; } set { Code.code = value; } }

        public BigfileCode Code;
    }
}
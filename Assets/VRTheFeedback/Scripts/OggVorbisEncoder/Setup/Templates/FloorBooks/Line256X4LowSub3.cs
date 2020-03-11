﻿namespace OggVorbisEncoder.Setup.Templates.FloorBooks
{
    public class Line256X4LowSub3 : IStaticCodeBook
    {
        public int Dimensions { get; } = 1;

        public byte[] LengthList { get; } = {
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 4, 2, 4, 3, 5, 4,
            5, 5, 5, 5, 6, 6, 6, 6, 6, 6, 6, 7, 7, 8, 6, 9,
            7, 12, 11, 16, 13, 16, 12, 15, 13, 15, 12, 14, 12, 15, 15, 15
        };

        public CodeBookMapType MapType { get; } = CodeBookMapType.None;
        public int QuantMin { get; } = 0;
        public int QuantDelta { get; } = 0;
        public int Quant { get; } = 0;
        public int QuantSequenceP { get; } = 0;
        public int[] QuantList { get; } = null;
    }
}
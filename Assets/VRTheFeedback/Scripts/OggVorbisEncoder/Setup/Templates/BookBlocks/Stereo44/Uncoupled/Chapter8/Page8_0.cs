namespace OggVorbisEncoder.Setup.Templates.BookBlocks.Stereo44.Uncoupled.Chapter8
{
    public class Page8_0 : IStaticCodeBook
    {
        public int Dimensions { get; } = 2;

        public byte[] LengthList { get; } = {
         1, 4, 4, 7, 7, 8, 8, 8, 7, 9, 8,10, 9,11,10, 4,
         6, 6, 8, 8,10, 9, 9, 9,10,10,11,10,12,10, 4, 6,
         6, 8, 8,10,10, 9, 9,10,10,11,11,11,12, 7, 8, 8,
        10,10,11,11,11,10,12,11,12,12,13,11, 7, 8, 8,10,
        10,11,11,10,10,11,11,12,12,13,13, 8,10,10,11,11,
        12,11,12,11,13,12,13,12,14,13, 8,10, 9,11,11,12,
        12,12,12,12,12,13,13,14,13, 8, 9, 9,11,10,12,11,
        13,12,13,13,14,13,14,13, 8, 9, 9,10,11,12,12,12,
        12,13,13,14,15,14,14, 9,10,10,12,11,13,12,13,13,
        14,13,14,14,14,14, 9,10,10,12,12,12,12,13,13,14,
        14,14,15,14,14,10,11,11,13,12,13,12,14,14,14,14,
        14,14,15,15,10,11,11,12,12,13,13,14,14,14,15,15,
        14,16,15,11,12,12,13,12,14,14,14,13,15,14,15,15,
        15,17,11,12,12,13,13,14,14,14,15,15,14,15,15,14,
        17,
};

        public CodeBookMapType MapType { get; } = (CodeBookMapType)1;
        public int QuantMin { get; } = -520986624;
        public int QuantDelta { get; } = 1620377600;
        public int Quant { get; } = 4;
        public int QuantSequenceP { get; } = 0;

        public int[] QuantList { get; } = {
        7,
        6,
        8,
        5,
        9,
        4,
        10,
        3,
        11,
        2,
        12,
        1,
        13,
        0,
        14,
};
    }
}
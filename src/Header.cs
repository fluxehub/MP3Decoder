using System;
using System.Diagnostics;

namespace MP3Decoder.Format 
{
    public enum Spec
    {
        MPEG_ONE,
        MPEG_TWO
    }

    public enum ChannelMode
    {
        STEREO,
        JOINT_STEREO,
        DUAL_CHANNEL,
        SINGLE_CHANNEL
    }

    public class Header 
    {
        public Spec ID { get; set; }
        public int Layer { get; set; }
        public bool Protection { get; set; }
        public int Bitrate { get; set; }
        public double SamplingFrequency { get; set; }
        public bool Padding { get; set; }
        public ChannelMode Mode { get; set; }
        public int ModeExtension { get; set; }
        public bool Copyright { get; set; }
        public bool IsCopy { get; set; }
        public int Emphasis { get; set; } // Hopefully never used

        public Header(uint headerData)
        {
            uint syncword = (headerData >> 20) & 0xFFF;
            Debug.Assert(syncword == 0xFFF, $"Unexpected syncword {syncword}");

            // If we actually get MPEG_TWO, panic
            uint id = (headerData >> 19) & 1;
            this.ID = id == 1 ? Spec.MPEG_ONE : Spec.MPEG_TWO;
            Debug.Assert(this.ID == Spec.MPEG_ONE);

            uint layer = (headerData >> 17) & 0b11;  
            this.Layer = (int) (4 - layer);
            Debug.Assert(this.Layer == 2);

            uint protectionBit = (headerData >> 16) & 1;
            this.Protection = protectionBit == 0;

            uint bitrateIndex = (headerData >> 12) & 0xF;
            // If the index is "free", idk what this means
            Debug.Assert(bitrateIndex != 0);

            int[,] bitrateTable = {
                { 0, 32, 64, 96, 128, 160, 192, 224, 256, 288, 320, 352, 284, 416, 448 },
                { 0, 32, 48, 56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320, 384 },
                { 0, 32, 40, 48,  56,  64,  80,  96, 112, 128, 160, 192, 224, 256, 320 }
            };

            this.Bitrate = bitrateTable[this.Layer - 1, bitrateIndex];

            uint samplingFrequency = (headerData >> 10) & 0b11;
            double[] samplingFrequencyTable = { 44.1, 48.0, 32.0 };
            this.SamplingFrequency = samplingFrequencyTable[samplingFrequency];

            uint paddingBit = (headerData >> 9) & 1;
            this.Padding = paddingBit == 1;

            // Skip privateBit

            uint mode = (headerData >> 6) & 0b11;
            switch (mode)
            {
            case 0:
                this.Mode = ChannelMode.STEREO;
                break;
            case 1:
                this.Mode = ChannelMode.JOINT_STEREO;
                break;
            case 2:
                this.Mode = ChannelMode.DUAL_CHANNEL;
                break;
            case 3:
                this.Mode = ChannelMode.SINGLE_CHANNEL;
                break;
            }

            // TODO: Layer 2 allowed modes bitrate combos

            this.ModeExtension = (int) (headerData >> 4) & 0b11;
            this.Copyright = (int) ((headerData >> 3) & 1) == 1;
            this.IsCopy = (int) ((headerData >> 2) & 1) == 0;
            this.Emphasis = (int) (headerData & 0b11);

            // Not bothering with this rn
            Debug.Assert(this.Emphasis == 0);
        }
    }
}
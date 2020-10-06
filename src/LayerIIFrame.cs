using System.Collections.Generic;
using System.Diagnostics;

namespace MP3Decoder.Format
{
    public class LayerIIFrame
    {
        Header FrameHeader { get; set; }
        IEnumerable<byte> Data { get; set; }
        int Bound { get; set; }
        int SubbandLimit { get; set; }
        int NumberOfChannels { get; set; }

        public LayerIIFrame(Header header, IEnumerable<byte> data)
        {
            this.FrameHeader = header;
            this.Data = data;
            this.NumberOfChannels = this.FrameHeader.Mode == ChannelMode.SINGLE_CHANNEL ? 1 : 2;

            Debug.Assert(this.FrameHeader.Bitrate == 320 && this.FrameHeader.SamplingFrequency == 48);
            Debug.Assert(this.FrameHeader.Mode == ChannelMode.STEREO);
            // TODO: Limit based off bitrate and sample rate
            this.SubbandLimit = 27;
            this.Bound = this.SubbandLimit;
        }

        public void Decode()
        {
            // Bit allocation decoding
            for (int subband = 0; subband < this.Bound; ++subband)
            {
                for (int channel = 0; channel < this.NumberOfChannels; ++channel)
                {
                    int nbal = 0;
                    if (subband <= 10)
                    {
                        nbal = 4;
                    }
                    else if (subband <= 22)
                    {
                        nbal = 3;
                    }
                    else
                    {
                        nbal = 2;
                    }
                }
            }
        }
    }
}
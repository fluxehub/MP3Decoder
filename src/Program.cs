using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

namespace MP3Decoder
{
    class Program
    {
        // TODO: Extract all this to a main decoder class
        static int GetDataStart(byte[] id3Header)
        {
            int offset = 0;

            if (Encoding.UTF8.GetString(new ArraySegment<byte>(id3Header, 0, 3)) == "ID3")
            {
                // ID3 data is present
                offset = 10;
                int synchsafeOffset = (id3Header[6] << 21) + (id3Header[7] << 14) + (id3Header[8] << 7) + id3Header[9];
                offset += synchsafeOffset;

                if (((id3Header[5] >> 10) & 1) == 1)
                {
                    offset += 10;
                }
            }

            return offset;
        }

        static void Main(string[] args)
        {
            byte[] data = File.ReadAllBytes("thewarehouse.mp3");

            byte[] id3Header = new byte[10];
            for (int i = 0; i < 10; ++i)
            {
                id3Header[i] = data[i];
            }

            int offset = GetDataStart(id3Header);

            List<Format.LayerIIFrame> frameList = new List<Format.LayerIIFrame>();

            for (int i = offset; i < data.Length; ++i)
            {
                byte b = data[i];
                if (b == 0xFF)
                {
                    if (((data[i + 1] >> 4) & 0xF) == 0xF)
                    {
                        uint headerData = (uint) ((data[i] << 24) + (data[i + 1] << 16) + (data[i + 2] << 8) + data[i + 3]);
                        Format.Header header = new Format.Header(headerData);
                        
                        if (header.Layer == 1)
                        {
                            i += (int) (4 * (12 * (header.Bitrate / header.SamplingFrequency) + (header.Padding ? 1 : 0))) - 1;
                        }
                        else
                        {
                            int dataStart = i + 4;
                            i += (int) (144 * (header.Bitrate / header.SamplingFrequency) + (header.Padding ? 1 : 0)) - 1;
                            frameList.Add(new Format.LayerIIFrame(header, new ArraySegment<byte>(data, dataStart, i - dataStart + 1)));
                        }
                    }
                }
            }

            Console.WriteLine("Wow!");
        }
    }
}

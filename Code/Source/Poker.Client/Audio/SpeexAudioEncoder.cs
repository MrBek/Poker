using System.IO;
using NSpeex;
using Poker.Client.Conversion;
using UnityEngine;

namespace Poker.Client.Audio
{
    public class SpeexAudioEncoder : IAudioEncoder
    {
        private readonly bool       encodeOgg;
        private readonly BandMode   bandMode;
        private readonly bool       isVbr;
        private readonly int        quality;
        private SpeexEncoder        speexEncoder;
        private OggSpeexWriter      oggWriter;
        private Stream              targetStream;

        private short[] shortBuffer;
        private byte[]  byteBuffer;

        public void Open(Stream targetStream, int channelCount, int sampleFrequency)
        {
            this.targetStream   = targetStream;
            speexEncoder        = new SpeexEncoder(bandMode) { Quality = quality,VBR = isVbr};

            if ( encodeOgg )
            {
                oggWriter = new OggSpeexWriter(GetModeAsInt(),sampleFrequency,channelCount,1,isVbr);
                oggWriter.Open(targetStream);
                oggWriter.WriteHeader("Unity3d");
            }
        }

        public void Close()
        {
            if ( encodeOgg )
            {
                oggWriter.Close();
            }
        }

        public void Write(float[] samples,int count)
        {
            Debug.Log("Samples: " + count);
            if (shortBuffer == null || shortBuffer.Length < count)
            {
                shortBuffer = new short[count];
            }

            if (byteBuffer == null || byteBuffer.Length < count * 4)
            {
                byteBuffer = new byte[count * 4];
            }

            Debug.Log("ShortBuffer: " + shortBuffer.Length);
            Debug.Log("ByteBuffer: " + byteBuffer.Length);

            ArrayConverters.FloatToShort(samples,count,shortBuffer);

            var bytesEncoded = speexEncoder.Encode(shortBuffer, 0, count, byteBuffer, 0, byteBuffer.Length);

            Debug.Log("BytesEncoded: " + bytesEncoded);

            if ( encodeOgg )
            {
                oggWriter.WritePacket(byteBuffer,0,bytesEncoded);
            }
            else
            {
                targetStream.Write(byteBuffer,0,bytesEncoded);
            }
        }

        private int GetModeAsInt()
        {
            if ( bandMode == BandMode.Wide)
            {
                return 1;
            }
            if ( bandMode == BandMode.UltraWide)
            {
                return 2;
            }

            return 0;
        }

        public SpeexAudioEncoder(bool encodeOgg, BandMode bandMode, bool isVbr, int quality)
        {
            this.encodeOgg  = encodeOgg;
            this.bandMode   = bandMode;
            this.isVbr      = isVbr;
            this.quality    = quality < 0 ? 10 : quality;
        }
    }
}
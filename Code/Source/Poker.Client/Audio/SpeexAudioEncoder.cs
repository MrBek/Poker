using System.IO;
using NSpeex;
using Poker.Common.Conversion;
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
        private AudioFrameBuffer    audioFrameBuffer;

        private short[] shortBuffer;
        private byte[]  byteBuffer;

        public void Open(Stream targetStream, int channelCount, int sampleFrequency)
        {
            this.targetStream   = targetStream;
            speexEncoder        = new SpeexEncoder(bandMode) { Quality = quality,VBR = isVbr};

            Debug.Log("FrameSize: " + speexEncoder.FrameSize);

            audioFrameBuffer = new AudioFrameBuffer(speexEncoder.FrameSize,1,WriteCore);

            if ( encodeOgg )
            {
                oggWriter = new OggSpeexWriter(GetModeAsInt(),sampleFrequency,channelCount,1,isVbr);
                oggWriter.Open(targetStream);
                oggWriter.WriteHeader("Unity3d");
            }
        }

        public void Close()
        {
            audioFrameBuffer.Dispose();

            if ( encodeOgg )
            {
                oggWriter.Close();
            }
        }

        public void Write(float[] input,int count)
        {
            audioFrameBuffer.Insert(input,count);
        }

        private void WriteCore(float[] samples,int count)
        {
            if (shortBuffer == null || shortBuffer.Length < count * 10)
            {
                shortBuffer = new short[count * 10];
            }

            if (byteBuffer == null || byteBuffer.Length < count *  10)
            {
                byteBuffer = new byte[count * 10];
            }

            ArrayConverters.FloatToShort(samples,count,shortBuffer);
            var bytesEncoded = speexEncoder.Encode(shortBuffer, 0, count, byteBuffer, 0, byteBuffer.Length);

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
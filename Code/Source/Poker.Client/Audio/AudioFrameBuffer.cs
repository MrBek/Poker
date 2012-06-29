using System;
using System.Collections.Generic;
using UnityEngine;

namespace Poker.Client.Audio
{
    public class AudioFrameBuffer : IDisposable
    {
        private readonly float[]                sampleBuffer;
        private int                             sampleBufferPosition;
        private readonly Action<float[], int>   callback;

        public void Insert(float[] input,int count)
        {
            var remainingCount  = count;
            var inputPosition   = 0;

            if ( sampleBufferPosition > 0 )
            {
                var samplesToCopy   = sampleBuffer.Length - sampleBufferPosition;
                samplesToCopy       = remainingCount >= samplesToCopy ? samplesToCopy : remainingCount;
            
                if ( samplesToCopy > 0 )
                {
                    Array.Copy(input,inputPosition,sampleBuffer,sampleBufferPosition,samplesToCopy);
                }

                sampleBufferPosition    += samplesToCopy;
                inputPosition           += samplesToCopy;
                remainingCount          -= samplesToCopy;
            
                EmitIfFull();
            }

            var fullPacketCount = remainingCount/sampleBuffer.Length;
            var lastPacketSize = remainingCount%sampleBuffer.Length;

            for( var i = 0; i < fullPacketCount; ++ i)
            {
                Array.Copy(input,inputPosition,sampleBuffer,0,sampleBuffer.Length);
                sampleBufferPosition += sampleBuffer.Length;

                EmitIfFull();
                inputPosition += sampleBuffer.Length;
            }

            if ( lastPacketSize > 0 )
            {
                Array.Copy(input,inputPosition,sampleBuffer,0,lastPacketSize);
                sampleBufferPosition = lastPacketSize;
            }
        }

        public void Dispose()
        {
            if ( sampleBufferPosition > 0 )
            {
                callback(sampleBuffer, sampleBuffer.Length);
            }
        }

        private void EmitIfFull()
        {
            if ( sampleBufferPosition == sampleBuffer.Length)
            {
                callback(sampleBuffer, sampleBuffer.Length);
                sampleBufferPosition = 0;
            }
        }

        public AudioFrameBuffer(int frameSize,int frameCount,Action<float[],int> callback)
        {
            sampleBuffer    = new float[frameSize * frameCount];
            this.callback   = callback;
        }
    }
}
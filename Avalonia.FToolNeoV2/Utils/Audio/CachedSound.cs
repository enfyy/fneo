using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.FToolNeoV2.Models;
using NAudio.Wave;

namespace Avalonia.FToolNeoV2.Utils;

class CachedSound
{
    public float[] AudioData { get; private set; }
    public WaveFormat WaveFormat { get; private set; }
    public CachedSound(Stream fileStream)
    {
        using var audioFileReader = new WavFileReader(fileStream);
        audioFileReader.Volume = Constants.AUDIO_VOLUME;
        
        //TODO: only 32000 sample rate is allowed rn so maybe resample here. 
        WaveFormat = audioFileReader.WaveFormat;
        var wholeFile = new List<float>((int)(audioFileReader.Length / 4));
        var readBuffer= new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
        int samplesRead;
        
        while((samplesRead = audioFileReader.Read(readBuffer,0,readBuffer.Length)) > 0)
            wholeFile.AddRange(readBuffer.Take(samplesRead));
        
        AudioData = wholeFile.ToArray();
    }
}
using System;
using System.Collections.Generic;
using Avalonia.FToolNeoV2.Enums;
using Avalonia.FToolNeoV2.Models;
using Avalonia.FToolNeoV2.Utils;
using Avalonia.FToolNeoV2.Utils.Audio;
using Avalonia.Platform;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Avalonia.FToolNeoV2.Services;

public class AudioService : IDisposable
{
    private static AudioService? _instance;
    
    private static readonly object singletonLock = new();

    public static bool IsInitialized;

    private readonly Dictionary<AudioAsset, CachedSound> _cachedSounds = new();

    public static AudioService Instance
    {
        get
        {
            lock (singletonLock)
                return _instance ??= new AudioService();
        }
    }
    
    //------------------------------------------------------------------------------------------------------------------

    private readonly IWavePlayer _outputDevice;
    
    private readonly MixingSampleProvider _mixer;

    private AudioService(int sampleRate = 32000, int channelCount = 2)
    {
        _outputDevice = new WaveOutEvent();
        _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
        _mixer.ReadFully = true;
        
        _outputDevice.Init(_mixer);
        _outputDevice.Play();
        
        var assetLoader = AvaloniaLocator.Current.GetService<IAssetLoader>();
        
        foreach (AudioAsset audioAsset in Enum.GetValues(typeof(AudioAsset)))
        {
            var uri = new Uri($"avares://{Constants.ASSEMBLY_NAME}/Assets/{audioAsset.ToString()}.wav");
            _cachedSounds.Add(audioAsset, new CachedSound(assetLoader.Open(uri)));
        }
        
        IsInitialized = true;
    }

    /// <summary>
    /// Add input to the mixer.
    /// </summary>
    /// <param name="input">The input that gets added.</param>
    private void AddMixerInput(ISampleProvider input) => _mixer.AddMixerInput(ConvertToRightChannelCount(input));

    /// <summary>
    /// Convert to right channel count.
    /// </summary>
    /// <param name="input">The input that gets converted.</param>
    /// <returns><see cref="ISampleProvider"/> with the right channel count.</returns>
    /// <exception cref="NotImplementedException">Thrown when the input has more than 2 channels.</exception>
    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == _mixer.WaveFormat.Channels)
            return input;
        
        if (input.WaveFormat.Channels == 1 && _mixer.WaveFormat.Channels == 2)
            return new MonoToStereoSampleProvider(input);
        
        throw new NotImplementedException("Not yet implemented this channel count conversion");
    }

    /// <summary>
    /// Play the sound.
    /// </summary>
    /// <param name="audioAsset">The type of sound that gets played.</param>
    public void PlaySound(AudioAsset audioAsset)
    {
        if (PersistenceManager.Instance.GetApplicationState().ApplicationSettings.IsAudioMuted) return;

        var found = _cachedSounds.TryGetValue(audioAsset, out var sound);
        if (!found || sound == null) 
            throw new ArgumentException("There is no sound loaded for the provided type.");
        
        AddMixerInput(new CachedSoundSampleProvider(sound));
    }

    public void Dispose() => _outputDevice.Dispose();
}
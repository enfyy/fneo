using System.IO;
using NAudio.Wave.SampleProviders;

// ReSharper disable once CheckNamespace
namespace NAudio.Wave
{
    /// <summary>
    /// Derived from AudioFileReader: https://github.com/naudio/NAudio/blob/fb35ce8367f30b8bc5ea84e7d2529e172cf4c381/NAudio/AudioFileReader.cs/>
    /// WavFileReader simplifies opening an audio file in NAudio
    /// Simply pass in the filename, and it will attempt to open the
    /// file and set up a conversion path that turns into PCM IEEE float.
    /// ACM codecs will be used for conversion.
    /// It provides a volume property and implements both WaveStream and
    /// ISampleProvider, making it possibly the only stage in your audio
    /// pipeline necessary for simple playback scenarios
    /// </summary>
    public class WavFileReader : WaveStream, ISampleProvider
    {
        /// <summary>
        /// WaveFormat of this stream
        /// </summary>
        public override WaveFormat WaveFormat => _sampleChannel.WaveFormat;

        /// <summary>
        /// Length of this stream (in bytes)
        /// </summary>
        public override long Length { get; }

        /// <summary>
        /// Position of this stream (in bytes)
        /// </summary>
        public override long Position
        {
            get => SourceToDest(_readerStream!.Position);
            set { lock (_lockObject) { _readerStream!.Position = DestToSource(value); }  }
        }

        /// <summary>
        /// Gets or Sets the Volume of this AudioFileReader. 1.0f is full volume
        /// </summary>
        public float Volume
        {
            get => _sampleChannel.Volume;
            set => _sampleChannel.Volume = value;
        }

        private WaveStream? _readerStream; // the waveStream which we will use for all positioning

        private readonly SampleChannel _sampleChannel; // sample provider that gives us most stuff we need

        private readonly int _destBytesPerSample;

        private readonly int _sourceBytesPerSample;

        private readonly object _lockObject;


        /// <summary>
        /// Initializes a new instance of AudioFileReader
        /// </summary>
        /// <param name="fileStream">The file stream.</param>
        public WavFileReader(Stream fileStream)
        {
            _lockObject = new object();
            CreateReaderStream(fileStream);
            _sourceBytesPerSample = (_readerStream!.WaveFormat.BitsPerSample / 8) * _readerStream.WaveFormat.Channels;
            _sampleChannel = new SampleChannel(_readerStream, false);
            _destBytesPerSample = 4 * _sampleChannel.WaveFormat.Channels;
            Length = SourceToDest(_readerStream.Length);
        }

        /// <summary>
        /// Creates the reader stream, supporting all filetypes in the core NAudio library,
        /// and ensuring we are in PCM format
        /// </summary>
        /// <param name="fileStream">File Stream</param>
        private void CreateReaderStream(Stream fileStream)
        {
            _readerStream = new WaveFileReader(fileStream);

            if (_readerStream.WaveFormat.Encoding is WaveFormatEncoding.Pcm or WaveFormatEncoding.IeeeFloat) 
                return;
                
            _readerStream = WaveFormatConversionStream.CreatePcmStream(_readerStream);
            _readerStream = new BlockAlignReductionStream(_readerStream);
        }

        /// <summary>
        /// Reads from this wave stream
        /// </summary>
        /// <param name="buffer">Audio buffer</param>
        /// <param name="offset">Offset into buffer</param>
        /// <param name="count">Number of bytes required</param>
        /// <returns>Number of bytes read</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            var waveBuffer = new WaveBuffer(buffer);
            var samplesRequired = count / 4;
            var samplesRead = Read(waveBuffer.FloatBuffer, offset / 4, samplesRequired);
            return samplesRead * 4;
        }

        /// <summary>
        /// Reads audio from this sample provider
        /// </summary>
        /// <param name="buffer">Sample buffer</param>
        /// <param name="offset">Offset into sample buffer</param>
        /// <param name="count">Number of samples required</param>
        /// <returns>Number of samples read</returns>
        public int Read(float[] buffer, int offset, int count)
        {
            lock (_lockObject)
            {
                return _sampleChannel.Read(buffer, offset, count);
            }
        }

        /// <summary>
        /// Helper to convert source to dest bytes
        /// </summary>
        private long SourceToDest(long sourceBytes) => _destBytesPerSample * (sourceBytes / _sourceBytesPerSample);

        /// <summary>
        /// Helper to convert dest to source bytes
        /// </summary>
        private long DestToSource(long destBytes) => _sourceBytesPerSample * (destBytes / _destBytesPerSample);

        /// <summary>
        /// Disposes this AudioFileReader
        /// </summary>
        /// <param name="disposing">True if called from Dispose</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_readerStream != null) {
                    _readerStream.Dispose();
                    _readerStream = null;
                }
            }
            base.Dispose(disposing);
        }
    }
}
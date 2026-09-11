using NAudio.Wave;
using OpenTK.Audio.OpenAL;

class Audio(string path)
{
    int source;
    int buffer;

    string path = path;
    bool Loaded = false;

    public float Volume = 1f;
    public float Pitch = 1f;

    void Load()
    {
        ALDevice device = ALC.OpenDevice(null);

        ALContext context = ALC.CreateContext(device, (int[]?)null);

        ALC.MakeContextCurrent(context);

        using WaveFileReader wav = new WaveFileReader(File.OpenRead(path));

        byte[] pcmData = new byte[wav.Length];

        int bytesRead = wav.Read(pcmData, 0, pcmData.Length);

        int sampleRate = wav.WaveFormat.SampleRate;
        int channels = wav.WaveFormat.Channels;
        int bitsPerSample = wav.WaveFormat.BitsPerSample;

        ALFormat format;

        if (channels == 1 && bitsPerSample == 8)
            format = ALFormat.Mono8;
        else if (channels == 1 && bitsPerSample == 16)
            format = ALFormat.Mono16;
        else if (channels == 2 && bitsPerSample == 8)
            format = ALFormat.Stereo8;
        else if (channels == 2 && bitsPerSample == 16)
            format = ALFormat.Stereo16;
        else
            throw new Exception("Unsupported WAV format");

        buffer = AL.GenBuffer();
        source = AL.GenSource();

        
        unsafe
        {
            fixed (byte* ptr = pcmData)
            {
                AL.BufferData(
                    buffer,
                    format,
                    (nint)ptr,
                    pcmData.Length,
                    sampleRate
                );
            }
        }
        

        AL.Source(source, ALSourcei.Buffer, buffer);

        Loaded = true;
    }

    // void 
    
    public void Play(bool loop=false)
    {
        if (!Loaded) Load();
        
        AL.Source(source, ALSourcei.Buffer, buffer);
        
        if (loop) 
        {
            AL.Source(source, ALSourceb.Looping, true);
        }
        
        AL.Source(source, ALSourcef.Gain, Volume);
        AL.Source(source, ALSourcef.Pitch, Pitch);
        


        AL.SourcePlay(source);
    }

    public void Stop()
    {
        AL.SourceStop(source);
    }
}
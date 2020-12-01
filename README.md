# ALSA.NET
Simple .Net Standard Wrapper for the Advanced Linux Sound Architecture (ASLA).

## Dependencies
Make sure to have the following libary installed on your target device:

`sudo apt-get install libasound2-dev`

## First Steps
1. Create your custom device settings or use the systems defaults
```
var soundDeviceSettings = new SoundDeviceSettings();
```

2. Build your sound device interface
```
using var alsaDevice = AlsaDeviceBuilder.Create(soundDeviceSettings);
```

3. Record 10 seconds of Audio to a file
```
alsaDevice.Record(10, "output.wav");
```

4. Play a Wav file
```
alsaDevice.Play("output.wav");
```

Ensure to look into the example projects for more details on how to use this package.

### Thank you
Thanks to ZhangGaoxing for the initial work on the ALSA Native Bindings.

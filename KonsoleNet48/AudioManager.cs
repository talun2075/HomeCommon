using AudioSwitcher.AudioApi;
using AudioSwitcher.AudioApi.CoreAudio;
using AudioSwitcher.AudioApi.Observables;
using System;
using System.Linq;
using System.Data;

namespace KonsoleNet48
{
    internal class AudioManager
    {
        private readonly CoreAudioDevice defaultPlaybackDevice;
        private readonly CoreAudioController cac;
        private double currentvolume;
        private Boolean currentMuteState;

        public AudioManager()
        {
            cac = new CoreAudioController();
            CoreAudioDevice cad = cac.GetPlaybackDevices().FirstOrDefault(x => x.IsDefaultDevice);
            defaultPlaybackDevice = cad;
            currentvolume = defaultPlaybackDevice.Volume;
            currentMuteState = defaultPlaybackDevice.IsMuted;
            defaultPlaybackDevice.MuteChanged.Subscribe(OnMuteChanged);
            defaultPlaybackDevice.VolumeChanged.Subscribe(OnVolChanged);
        }

        public void OnMuteChanged(DeviceMuteChangedArgs changedArgs)
        {
            if(currentMuteState != changedArgs.IsMuted)
            {
                currentMuteState = changedArgs.IsMuted;
            }

        }
        public void OnVolChanged(DeviceVolumeChangedArgs changedArgs)
        {
            if (changedArgs.Volume != currentvolume)
                currentvolume = changedArgs.Volume;
        }
    }
}

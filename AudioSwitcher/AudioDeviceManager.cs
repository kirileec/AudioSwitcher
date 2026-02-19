using System;
using System.Collections.Generic;
using CoreAudio;

namespace AudioSwitcher
{
    public class AudioDeviceManager : IDisposable
    {
        private MMDeviceEnumerator _deviceEnumerator;
        private MMDeviceCollection _devices;

        public AudioDeviceManager()
        {
            _deviceEnumerator = new MMDeviceEnumerator(Guid.Empty);
            _devices = GetOutputDevices();
        }
        public MMDevice GetDeviceById(string deviceId)
        {
            if (_devices == null)
            {
                return null;
            }
            return _devices.FirstOrDefault(d => d.ID == deviceId);
        }


        public MMDeviceCollection GetOutputDevices()
        {
            
            return _deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
        }

        public MMDevice GetDefaultOutputDevice()
        {
            try
            {
                return _deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Console);
            }
            catch
            {
                return null;
            }
        }

        public bool SetDefaultOutputDevice(MMDevice device)
        {
            try
            {
                _deviceEnumerator.SetDefaultAudioEndpoint(device);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose()
        {
        }
    }
}

using NAudio.CoreAudioApi;
using System;
using System.Runtime.InteropServices;

namespace Wox.Plugins.AudioAndDarkNightSwitch
{
    public static class AudioManager
    {
        private const string AUDIO_IMMDEVICE_IID = "D666063F-1587-4E43-81F1-B948E807363F";
        private const string AUDIO_IMMDEVICE_ENUMERATOR_IID = "A95664D2-9614-4F35-A746-DE8DB63617E6";
        private const string AUDIO_IMMDEVICE_ENUMERATOR_OBJECT_IID = "BCDE0395-E52F-467C-8E3D-C4579291692E";

        private const string POLICY_CONFIG_VISTA_IID = "568B9108-44BF-40B4-9006-86AFE5B5A620";
        private const string POLICY_CONFIG_IID = "F8679F50-850A-41CF-9C72-430F290290C8";
        private const string POLICY_CONFIG_X_IID = "8F9FB2AA-1C0B-4D54-B6BB-B2F2A10CE03C";
        private const string POLICY_CONFIG_CLIENT_IID = "870AF99C-171D-4F9E-AF0D-E63DF40C2BC9";


        [Guid(POLICY_CONFIG_IID)]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        interface IPolicyConfig
        {
            [PreserveSig]
            int GetMixFormat(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] IntPtr ppFormat);

            [PreserveSig]
            int GetDeviceFormat(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.Bool)] bool bDefault,
                [In] IntPtr ppFormat);

            [PreserveSig]
            int ResetDeviceFormat([In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName);

            [PreserveSig]
            int SetDeviceFormat(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] IntPtr pEndpointFormat,
                [In] IntPtr mixFormat);

            [PreserveSig]
            int GetProcessingPeriod(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.Bool)] bool bDefault,
                [In] IntPtr pmftDefaultPeriod,
                [In] IntPtr pmftMinimumPeriod);

            [PreserveSig]
            int SetProcessingPeriod(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] IntPtr pmftPeriod);

            [PreserveSig]
            int GetShareMode(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] IntPtr pMode);

            [PreserveSig]
            int SetShareMode(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] IntPtr mode);

            [PreserveSig]
            int GetPropertyValue(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.Bool)] bool bFxStore,
                [In] IntPtr key,
                [In] IntPtr pv);

            [PreserveSig]
            int SetPropertyValue(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.Bool)] bool bFxStore,
                [In] IntPtr key,
                [In] IntPtr pv);

            [PreserveSig]
            int SetDefaultEndpoint(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.U4)] ERole role);

            [PreserveSig]
            int SetEndpointVisibility(
                [In] [MarshalAs(UnmanagedType.LPWStr)] string pszDeviceName,
                [In] [MarshalAs(UnmanagedType.Bool)] bool bVisible);
        }

        [ComImport, Guid(POLICY_CONFIG_CLIENT_IID)]
        private class _PolicyConfigClient
        {
        }

        public static MMDevice[] GetPlayBackDevices()
        {
            var coll = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);
            int i = 0;
            var results = new MMDevice[coll.Count];
            foreach (var dev in coll)
            {
                results[i++] = dev;
            }
            return results;
        }

        [Flags]
        public enum ERole : uint
        {
            eConsole = 0,
            eMultimedia = (eConsole + 1),
            eCommunications = (eMultimedia + 1),
            ERole_enum_count = (eCommunications + 1)
        }

        public static bool SwitchToPlaybackDevice(string devId)
        {
            IPolicyConfig config = new _PolicyConfigClient() as IPolicyConfig;
            config.SetDefaultEndpoint(devId, ERole.eConsole);
            config.SetDefaultEndpoint(devId, ERole.eMultimedia);
            config.SetDefaultEndpoint(devId, ERole.eCommunications);
            return true;
        }
    }
}

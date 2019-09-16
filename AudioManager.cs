using NAudio.CoreAudioApi;
using System;
using System.Runtime.InteropServices;

namespace Wox.Plugins.AudioAndDarkNightSwitch
{
    public static class AudioManager
    {
        private const string POLICY_CONFIG_IID = "F8679F50-850A-41CF-9C72-430F290290C8";
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
            Console = 0,
            Multimedia = (Console + 1),
            Communications = (Multimedia + 1),
            All = (Communications + 1)
        }

        public static bool SwitchToPlaybackDevice(string devId)
        {
            var config = new _PolicyConfigClient() as IPolicyConfig;
            config.SetDefaultEndpoint(devId, ERole.Console);
            config.SetDefaultEndpoint(devId, ERole.Multimedia);
            config.SetDefaultEndpoint(devId, ERole.Communications);
            return true;
        }
    }
}

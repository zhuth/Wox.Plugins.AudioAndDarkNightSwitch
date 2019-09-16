using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;

namespace Wox.Plugins.AudioAndDarkNightSwitch
{
    public static class ScreenTint
    {
        [DllImport("user32")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32")]
        private static extern bool GetDeviceGammaRamp(IntPtr hDC, [Out] ushort[] lpRamp);

        [DllImport("gdi32")]
        private static extern bool SetDeviceGammaRamp(IntPtr hDC, ushort[] lpRamp);

        private static IntPtr hDC;

        private static ushort[] nightmode, normal;

        private static ushort[] getGampArray(byte[] rgb)
        {
            var g = new ushort[768];
            for (var i = 0; i < 3; ++i)
                for (var j = 0; j < 256; ++j)
                    g[i * 256 + j] = (ushort)(j * rgb[i]);
            return g;
        }

        static ScreenTint()
        {
            hDC = GetDC(IntPtr.Zero);
            normal = getGampArray(new byte[] { 255, 255, 255 });
            SetTintTemperature();
        }

        public static byte[] GetRGBForTemperature(int temperature)
        {
            temperature /= 100;
            double r = 255, g = 255, b = 255;

            if (temperature <= 66)
            {
                g = temperature;
                g = 99.4708025861 * Math.Log(g) - 161.1195681661;
                if (temperature <= 19) b = 0;
                else
                {
                    b = temperature - 10;
                    b = 138.5177312231 * Math.Log(b) - 305.0447927307;
                }
            }
            else
            {
                r = temperature - 60;
                r = 329.698727446 * Math.Pow(r, -0.1332047592);
                g = temperature - 60;
                g = 288.1221695283 * Math.Pow(g, -0.0755148492);
            }

            return new byte[]
            {
                (byte)((r<0)?0:(r>255)?255:r),
                (byte)((g<0)?0:(g>255)?255:g),
                (byte)((b<0)?0:(b>255)?255:b),
            };
        }

        public static void SetTintTemperature(int value = 0)
        {
            if (value > 0)
            {
                Properties.Settings.Default.TintTemperature = value;
                Properties.Settings.Default.Save();
            }
            var rgb = GetRGBForTemperature(Properties.Settings.Default.TintTemperature);
            nightmode = getGampArray(rgb);
            if (Properties.Settings.Default.Mode == "Dark")
            {
                Tint();
            }
        }

        internal static void AutoMode()
        {
            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);
            int lightMode = (int)registryKey.GetValue("SystemUsesLightTheme");
            if (lightMode == 0) Tint();
            else Restore();
        }

        public static void Tint()
        {
            if (Properties.Settings.Default.Mode != "Dark")
            {
                Properties.Settings.Default.Mode = "Dark";
                Properties.Settings.Default.Save();
            }
            SetDeviceGammaRamp(hDC, nightmode);

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);
            registryKey.SetValue("SystemUsesLightTheme", 0, RegistryValueKind.DWord);
            registryKey.SetValue("AppsUseLightTheme", 0, RegistryValueKind.DWord);
        }

        public static void Restore()
        {
            if (Properties.Settings.Default.Mode == "Dark")
            {
                Properties.Settings.Default.Mode = "Light";
                Properties.Settings.Default.Save();
            }
            SetDeviceGammaRamp(hDC, normal);

            RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize", true);
            registryKey.SetValue("SystemUsesLightTheme", 1, RegistryValueKind.DWord);
            registryKey.SetValue("AppsUseLightTheme", 1, RegistryValueKind.DWord);
        }

    }
}

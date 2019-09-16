using System;
using System.Collections.Generic;
using Wox.Plugin;

namespace Wox.Plugins.AudioAndDarkNightSwitch
{

    public class Main : IPlugin
    {
        public List<Result> Query(Query query)
        {
            var results = new List<Result>();

            switch (query.ActionKeyword)
            {
                case "DarkNight":
                case "LightDay":
                    if (query.ActionKeyword == "DarkNight") DarkNight();
                    else LightDay();
                    break;
                case "Sound":
                    foreach (var dev in AudioManager.GetPlayBackDevices())
                    {
                        results.Add(new Result
                        {
                            Action = new Func<ActionContext, bool>((a) => AudioManager.SwitchToPlaybackDevice(dev)),
                            Title = dev.FriendlyName
                        });
                    }
                    break;
            }

            return results;
        }

        public bool DarkNight()
        {
            ScreenTint.Tint();
            ScreenTint.AppTheme(true);
            ScreenTint.SystemTheme(true);
            return true;
        }
        public bool LightDay()
        {
            ScreenTint.Restore();
            ScreenTint.AppTheme(false);
            ScreenTint.SystemTheme(false);
            return true;
        }

        public void Init(PluginInitContext context)
        {
            context.CurrentPluginMetadata.ActionKeywords = new List<string>(new string[]       {
                "DarkNight",
                "LightDay",
                "Sound"
            });
        }

    }

    public static class DebuggerConsole
    {
        public static void Main(string[] args)
        {
            var devs = AudioManager.GetPlayBackDevices();
            foreach (var dev in devs)
            {
                Console.WriteLine($"{dev.FriendlyName} {dev.ID}");
            }
            AudioManager.SwitchToPlaybackDevice(devs[2]);
            Console.ReadLine();
        }
    }
}

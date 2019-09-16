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
                    results.Add(new Result
                    {
                        Action = new Func<ActionContext, bool>((a) =>
                        {
                            if (query.ActionKeyword == "DarkNight") DarkNight();
                            else LightDay();
                            return true;
                        }),
                        Title = query.ActionKeyword
                    });
                    break;
                case "DarkNightTemp":
                    int tmp;
                    if (int.TryParse(query.Search, out tmp))
                    {
                        results.Add(new Result
                        {
                            Action = new Func<ActionContext, bool>((a) =>
                            {
                                ScreenTint.SetTintTemperature(tmp);

                                return true;
                            }),
                            Title = query.ActionKeyword
                        });
                    }
                    break;
                case "Sound":
                    foreach (var dev in AudioManager.GetPlayBackDevices())
                    {
                        string devId = dev.ID;
                        results.Add(new Result
                        {
                            Action = new Func<ActionContext, bool>((a) => AudioManager.SwitchToPlaybackDevice(devId)),
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
            return true;
        }
        public bool LightDay()
        {
            ScreenTint.Restore();
            return true;
        }

        public void Init(PluginInitContext context)
        {
            ScreenTint.AutoMode();
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
            AudioManager.SwitchToPlaybackDevice(devs[2].ID);
            Console.ReadLine();
        }
    }
}

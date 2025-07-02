using System.ComponentModel;
using Exiled.API.Interfaces;

namespace CrazyHintFramework
{
    public class Config : IConfig
    {
        [Description("Whether the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        [Description("Whether to show debug messages.")]
        public bool Debug { get; set; } = false;

        [Description("Log related settings.")]
        public LogSettings Log { get; set; } = new();

        [Description("Plugin behavior settings.")]
        public PluginSettings Settings { get; set; } = new();
    }

    public class LogSettings
    {
        [Description("Enable logging to file.")]
        public bool EnableFileLog { get; set; } = true;

        [Description("Log file name.")]
        public string FileName { get; set; } = "CrazyHintFramework_log.txt";
    }

    public class PluginSettings
    {
        [Description("The delay between events in seconds.")]
        public float EventDelay { get; set; } = 3.5f;

        [Description("Enable custom feature.")]
        public bool EnableFeatureX { get; set; } = false;
    }
}
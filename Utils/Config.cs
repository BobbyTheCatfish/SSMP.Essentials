using UnityEngine;
using BepInEx.Configuration;

namespace SSMPEssentials.Utils
{

    internal static class Config
    {
        public const uint SSMPApiVersion = 1;
        public const string ModName = "SSMP Essentials";
        public const string Version = "0.1.2";

        static ConfigEntry<bool>? _testsEnabled;
        public static bool TestsEnabled => _testsEnabled?.Value ?? false;

        static ConfigEntry<KeyCode>? _spectateNext;
        public static KeyCode SpectateNext => _spectateNext?.Value ?? KeyCode.None;

        static ConfigEntry<KeyCode>? _spectatePrevious;
        public static KeyCode SpectatePrevious => _spectatePrevious?.Value ?? KeyCode.None;

        static ConfigEntry<KeyCode>? _exitSpectate;
        public static KeyCode ExitSpectate => _exitSpectate?.Value ?? KeyCode.None;

        static ConfigEntry<KeyCode>? _freecamToggle;
        public static KeyCode FreecamToggle => _freecamToggle?.Value ?? KeyCode.None;

        static ConfigEntry<Color>? _defaultColor;

        public static Color DefaultColor => _defaultColor?.Value ?? Color.white;

        public static void Init(ConfigFile config)
        {
#if DEBUG
            _testsEnabled = config.Bind("Misc", "Run Tests", false);
#endif
            _spectatePrevious = config.Bind("Keybinds", "Spectate Previous Player", KeyCode.Alpha2);
            _spectateNext = config.Bind("Keybinds", "Spectate Next Player", KeyCode.Alpha3);
            _exitSpectate = config.Bind("Keybinds", "Exit Spectate Mode", KeyCode.Alpha4);
            _freecamToggle = config.Bind("Keybinds", "Toggle Freecam Mode", KeyCode.Alpha5);
            _defaultColor = config.Bind("Username Colors", "Default Color", Color.white);
        }
    }
}

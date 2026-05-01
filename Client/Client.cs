using System;
using System.Linq;
using SSMP.Api.Client;
using SSMPEssentials.Client.Modules;
using SSMPEssentials.Server.Modules;
using SSMPEssentials.Client.Commands;
using SSMPEssentials.Utils;
using HarmonyLib;

namespace SSMPEssentials.Client
{
    internal class Client : ClientAddon
    {
        protected override string Name => Config.ModName;
        protected override string Version => Config.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IClientApi api;

        internal static Client instance;

        internal static ServerSettings ServerSettings = new(true);

        internal static Action OnServerSettingsUpdate = () => { };

        internal static Action<SSMP.Game.Settings.ServerSettings>? OnSSMPSettingsUpdate;

        public override void Initialize(IClientApi clientApi)
        {
            instance = this;
            api = clientApi;

            Log.SetLogger(Logger);
            PacketReceiver.Init();
            PacketSender.Init();

            api.CommandManager.RegisterCommand(new Huddle());
            api.CommandManager.RegisterCommand(new TeleportRequest());
            api.CommandManager.RegisterCommand(new TeleportAccept());
            api.CommandManager.RegisterCommand(new TeleportDeny());
            api.CommandManager.RegisterCommand(new TeleportBack());
            api.CommandManager.RegisterCommand(new ColorCommand());

            api.ClientManager.ConnectEvent += ColoredNames.OnJoin;

            api.ClientManager.DisconnectEvent += () => Spectate.ReturnToSelf();
            
            api.ClientManager.PlayerEnterSceneEvent += PlayerHealth.OnPlayerEnter;
            api.ClientManager.PlayerEnterSceneEvent += ColoredNames.ChangeTextColor;

            api.ClientManager.PlayerDisconnectEvent += (p) => ColoredNames.ResetPlayerColor(p, true);

            Spectate.Init();
            ColoredNames.Init();

            HeroController.OnHeroInstanceSet += ColoredNames.HeroSetHook;
            if (HeroController.SilentInstance)
            {
                ColoredNames.HeroSetHook(HeroController.instance);
            }

            Log.LogInfo("SSMP Essentials Client Initialized");

        }

        public static void LocalChat(string message)
        {
            SSMPEssentialsPlugin.NextFrames.Add(() => api.UiManager.ChatBox.AddMessage(message));
        }

        public static IClientPlayer? GetPlayerByName(string username)
        {
            // Try to find player
            var players = api.ClientManager.Players;
            var foundPlayer = players.FirstOrDefault(p => p.Username.Equals(username, StringComparison.CurrentCultureIgnoreCase));

            return foundPlayer;
        }

        public static IClientPlayer? GetPlayer(ushort id)
        {
            return api.ClientManager.GetPlayer(id);
        }

        public static SSMP.Game.Settings.ServerSettings GetClientServerSettings()
        {
            var manager = api.ClientManager;
            var settingsType = manager.GetType();

            var traverse = Traverse.Create(manager).Field("_modSettings").Property("ServerSettings");
            return traverse.GetValue<SSMP.Game.Settings.ServerSettings>();
        }
    }
}

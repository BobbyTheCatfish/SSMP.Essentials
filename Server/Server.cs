using SSMP.Api.Server;
using SSMPEssentials.Server.Modules;
using SSMPEssentials.Server.Commands;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Server
{
    internal class Server : ServerAddon
    {
        protected override string Name => Config.ModName;
        protected override string Version => Config.Version;
        public override uint ApiVersion => Config.SSMPApiVersion;
        public override bool NeedsNetwork => true;

        internal static IServerApi api;

        internal static Server instance;

        internal static ServerSettings ServerSettings = new(false);

        public static Modules.Colors? Colors;

        public override void Initialize(IServerApi serverApi)
        {
            instance = this;
            api = serverApi;
            Colors = new();

            Log.SetLogger(Logger);

            serverApi.ServerManager.PlayerConnectEvent += SendJoinInfo;
            serverApi.ServerManager.PlayerDisconnectEvent += Colors.OnPlayerLeave;

            serverApi.CommandManager.RegisterCommand(new SettingsCommand());

            ServerSettings = ServerSettings.ReadFromFile();
            PacketReceiver.Init();
            PacketSender.Init();

            Log.LogInfo("SSMP Essentials Server Initialized");
        }

        public static IServerPlayer? GetPlayer(ushort id)
        {
            return api.ServerManager.GetPlayer(id);
        }

        public static void SendMessageToPlayer(ushort id, string message)
        {
            api.ServerManager.SendMessage(id, message);
        }

        public static void BroadcastMessage(string message)
        {
            api.ServerManager.BroadcastMessage(message);
        }

        static void SendJoinInfo(IServerPlayer player)
        {
            PacketSender.SendSettingsUpdate(player.Id);
            PacketSender.SendAllPlayerHealth(player.Id);

            Colors?.OnPlayerJoin(player.Id);
        }
    }
}

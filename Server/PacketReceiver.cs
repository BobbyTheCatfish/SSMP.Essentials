using SSMP.Api.Server;
using SSMP.Api.Server.Networking;
using SSMP.Math;
using SSMPEssentials.Data;
using SSMPEssentials.Utils;
using SSMPEssentials.Client.Packets;
using Cause = SSMPEssentials.Utils.CauseOfDeath;
using SSMPEssentials.Server.Modules;

namespace SSMPEssentials.Server
{
    internal static class PacketReceiver
    {
        public static void Init()
        {
            var receiver = Server.api.NetServer.GetNetworkReceiver<PacketIDs>(Server.instance, Client.Packets.Packets.Instantiate);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.Huddle, OnHuddle);
            receiver.RegisterPacketHandler<TeleportRequestPacket>(PacketIDs.TeleportRequest, OnTeleportRequest);
            receiver.RegisterPacketHandler<TeleportPacket>(PacketIDs.TeleportAccept, OnTeleportRequestAccept);
            receiver.RegisterPacketHandler<MessagePacket>(PacketIDs.Message, OnMessage);
            receiver.RegisterPacketHandler<DeathPacket>(PacketIDs.PlayerDeath, OnPlayerDeath);
            receiver.RegisterPacketHandler<HealthPacket>(PacketIDs.PlayerHealth, OnPlayerHealth);
            receiver.RegisterPacketHandler<ColorPacket>(PacketIDs.Color, OnColor);
        }

        public static void OnHuddle(ushort id, TeleportPacket data)
        {
            // Check if disabled
            if (!Server.ServerSettings.HuddleEnabled)
            {
                Server.SendMessageToPlayer(id, "Huddles are currently disabled.");
                return;
            }

            // Check if authorized
            var player = Server.GetPlayer(id);
            if (player == null || !player.IsAuthorized)
            {
                Server.SendMessageToPlayer(id, "You need to be authorized to use that command.");
                return;
            }

            // No scene = teleporting to someone else
            if (data.Scene == "")
            {
                var targetPlayer = Server.GetPlayer(data.PlayerId);
                if (targetPlayer == null)
                {
                    Server.SendMessageToPlayer(id, "I couldn't find that player.");
                    return;
                }

                data.Scene = targetPlayer.CurrentScene;
                data.Position = targetPlayer.Position ?? Vector2.Zero;
            }
            else
            {
                data.PlayerId = id;
            }

            PacketSender.BroadcastWarp(data.Scene, data.Position, data.PlayerId);
        }

        public static void OnTeleportRequest(ushort id, TeleportRequestPacket data)
        {
            // Check if disabled
            if (!Server.ServerSettings.TeleportsEnabled)
            {
                Server.SendMessageToPlayer(id, "Teleporting is currently disabled.");
                return;
            }

            var targetPlayer = Server.GetPlayer(data.PlayerId);
            if (targetPlayer == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendTeleportRequest(data.PlayerId, id);
        }

        public static void OnTeleportRequestAccept(ushort id, TeleportPacket data)
        {
            // Check if disabled
            if (!Server.ServerSettings.TeleportsEnabled)
            {
                Server.SendMessageToPlayer(id, "Teleporting is currently disabled.");
                return;
            }

            var playerToTeleport = Server.GetPlayer(data.PlayerId);
            if (playerToTeleport == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendTeleportAccepted(data.PlayerId, id, data.Scene, data.Position);
        }

        public static void OnMessage(ushort id, MessagePacket data)
        {
            var player = Server.GetPlayer(data.PlayerId);
            if (player == null)
            {
                Server.SendMessageToPlayer(id, "I couldn't find that player.");
                return;
            }

            PacketSender.SendMessage(data.PlayerId, id, data.Message);
        }

        public static void OnPlayerDeath(ushort id, DeathPacket data)
        {
            // Check if disabled
            if (!Server.ServerSettings.DeathMessagesEnabled) return;

            var player = Server.GetPlayer(id);
            if (player == null) return;

            var opponent = Server.GetPlayer(data.KillerID);
            var deathString = Common.ColoredUsername(player) + " " + DetermineDeathString(data.Cause, opponent);

            if (data.Cause != Cause.Player && data.RanAway)
            {
                deathString += " while fighting " + Common.ColoredUsername(opponent);
            }

            Server.api.ServerManager.BroadcastMessage(deathString);
        }

        static string DetermineDeathString(Cause cause, IServerPlayer? player)
        {
            if (!DeathMessages.Messages.TryGetValue(cause, out var strings))
            {
                strings = DeathMessages.Messages[Cause.Generic];
            }

            var str = strings.GetRandom();
            if (cause == Cause.Player)
            {
                str += " " + Common.ColoredUsername(player);
            }

            return str;
        }

        static void OnPlayerHealth(ushort id, HealthPacket data)
        {
            Log.LogDebug($"Received health from {id}: {data.Health}");
            var health = PlayerDataTracker.ServerInstance.GetPlayer(id);
            health.Health = data.Health;

            PacketSender.BroadcastPlayerHealth(id, health.Health);
        }

        static void OnColor(ushort id, ColorPacket data)
        {
            Log.LogDebug($"Received color from {id}: {data.Color.ToHtmlString()}");

            Server.Colors?.SetColor(id, data.Color);
            PacketSender.BroadcastColor(id, data.Color);
        }
    }
}

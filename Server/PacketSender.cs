using SSMP.Api.Server.Networking;
using SSMP.Math;
using SSMPEssentials.Data;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Server
{
    internal static class PacketSender
    {
        static IServerAddonNetworkSender<PacketIDs>? sender;
        internal static void Init()
        {
            sender = Server.api.NetServer.GetNetworkSender<PacketIDs>(Server.instance);
        }

        static void Broadcast(PacketIDs id, Packet packet, ushort senderId, bool collection = true)
        {
            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }

            var players = Server.api.ServerManager.Players;
            foreach (var player in players)
            {
                if (player.Id == senderId) continue;

                if (collection) sender.SendCollectionData(id, packet, player.Id);
                else sender.SendSingleData(id, packet, player.Id);
            }
        }

        internal static void BroadcastWarp(string scene, Vector2 position, ushort destinationPlayerId)
        {
            var data = new Client.Packets.TeleportPacket
            {
                Scene = scene,
                Position = position
            };

            Broadcast(PacketIDs.Huddle, data, destinationPlayerId, false);
        }

        internal static void SendTeleportRequest(ushort targetPlayerId, ushort senderId)
        {
            var data = new Client.Packets.TeleportRequestPacket
            {
                PlayerId = senderId
            };

            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }
            sender.SendCollectionData(PacketIDs.TeleportRequest, data, targetPlayerId);
        }

        internal static void SendTeleportAccepted(ushort playerToTeleportId, ushort destinationPlayerId, string scene,  Vector2 position)
        {
            var data = new Client.Packets.TeleportPacket
            {
                PlayerId = destinationPlayerId,
                Position = position,
                Scene = scene
            };

            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }
            sender.SendSingleData(PacketIDs.TeleportAccept, data, playerToTeleportId);
        }

        internal static void SendMessage(ushort recipientId, ushort senderId, Messages message)
        {
            var data = new Client.Packets.MessagePacket
            {
                Message = message,
                PlayerId = senderId
            };

            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }
            sender.SendCollectionData(PacketIDs.Message, data, recipientId);
        }

        internal static void BroadcastPlayerHealth(ushort playerId, HealthData health)
        {
            var data = new Packets.PlayerHealthPacket
            {
                PlayerId = playerId,
                Health = health
            };

            Broadcast(PacketIDs.PlayerHealth, data, playerId, true);
        }

        internal static void SendAllPlayerHealth(ushort recipientId)
        {
            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }

            foreach (var player in PlayerDataTracker.ServerInstance.GetAllData())
            {
                var data = new Packets.PlayerHealthPacket
                {
                    PlayerId = player.Id,
                    Health = player.Health
                };

                sender.SendCollectionData(PacketIDs.PlayerHealth, data, recipientId);
            }
        }

        internal static void BroadcastSettingsUpdate()
        {
            var data = new Packets.SettingsPacket
            {
                ServerSettings = Server.ServerSettings
            };

            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }
            sender.BroadcastSingleData(PacketIDs.Settings, data);
        }

        internal static void SendSettingsUpdate(ushort id)
        {
            var data = new Packets.SettingsPacket
            {
                ServerSettings = Server.ServerSettings
            };

            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }
            sender.SendSingleData(PacketIDs.Settings, data, id);
        }

        internal static void BroadcastColor(ushort id, ColorLite color)
        {
            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }

            var data = new Packets.PlayerColorPacket
            {
                Color = color,
                PlayerId = id,
            };

            Broadcast(PacketIDs.Color, data, id, true);
        }

        internal static void SendColor(ushort playerId, ushort recipientId, ColorLite color)
        {
            if (sender == null)
            {
                Log.LogError("Unable to send packet, packet sender is null.");
                return;
            }


            var data = new Packets.PlayerColorPacket
            {
                Color = color,
                PlayerId = playerId,
            };

            sender.SendCollectionData(PacketIDs.Color, data, recipientId);
        }
    }
}

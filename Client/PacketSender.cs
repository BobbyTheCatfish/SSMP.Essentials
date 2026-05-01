using System.Globalization;
using UnityEngine.SceneManagement;
using SSMP.Api.Client.Networking;
using SSMP.Networking.Packet;
using SSMP.Math;
using SSMPEssentials.Client.Modules;
using SSMPEssentials.Data;
using SSMPEssentials.Utils;
using SSMPEssentials.Client.Packets;

namespace SSMPEssentials.Client
{
    internal static class PacketSender
    {
        static IClientAddonNetworkSender<PacketIDs>? sender;
        internal static void Init()
        {
            sender = Client.api.NetClient.GetNetworkSender<PacketIDs>(Client.instance);
        }

        static void SendData(PacketIDs packetId, IPacketData data)
        {
            if (Client.api.NetClient.IsConnected && sender != null)
            {
                sender.SendSingleData(packetId, data);
            }
        }

        internal static void SendMessage(ushort id, Messages message)
        {
            var data = new MessagePacket
            {
                Message = message,
                PlayerId = id,
            };
            SendData(PacketIDs.Message, data);
        }

        internal static void SendHuddle()
        {
            var foundScenePosition = Warp.GetHornetScenePosition(out var scene, out var position);
            if (!foundScenePosition)
            {
                return;
            }

            Log.LogDebug($"Sending huddle packet for scene {scene} at location {position}");
            SendData(PacketIDs.Huddle, new TeleportPacket
            {
                Scene = scene,
                Position = position,
            });
        }

        internal static void SendHuddle(ushort id)
        {
            if (GameManager.SilentInstance == null || GameManager.SilentInstance.GameState != GlobalEnums.GameState.PLAYING)
            {
                Client.LocalChat("Resume the game first!");
                return;
            }

            Log.LogDebug($"Sending huddle packet for user {id}");
            SendData(PacketIDs.Huddle, new TeleportPacket
            {
                Scene = "",
                Position = Vector2.Zero,
                PlayerId = id
            });
        }

        internal static void SendTeleportAccept(TeleportRequests.Request request)
        {
            var foundScenePosition = Warp.GetHornetScenePosition(out var scene, out var position);
            if (!foundScenePosition)
            {
                return;
            }

            request.Responded = true;

            Log.LogDebug($"Sending teleport accept to {request.PlayerId}");
            SendData(PacketIDs.TeleportAccept, new TeleportPacket
            {
                Scene = scene,
                Position = position,
                PlayerId = request.PlayerId
            });
        }

        internal static void SendTeleportRequest(ushort playerId)
        {
            var data = new TeleportRequestPacket
            {
                PlayerId = playerId
            };

            SendData(PacketIDs.TeleportRequest, data);
        }

        internal static void SendDeath(CauseOfDeath cause, ushort killerId = 0, bool ranAway = false)
        {
            var scene = SceneManager.GetActiveScene().name;
            SceneTeleportMap.GetTeleportMap().TryGetValue(scene, out var map);

            var areaName = scene;
            if (map != null)
            {
                var split = map.MapZone.ToString().Replace('_', ' ');
                areaName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(split);
            }

            var data = new DeathPacket
            {
                Scene = areaName,
                KillerID = killerId,
                Cause = cause,
                RanAway = ranAway
            };

            SendData(PacketIDs.PlayerDeath, data);
        }

        internal static void SendHealth(HealthData healthData)
        {
            Log.LogDebug($"Sending health: {healthData}");

            var data = new HealthPacket
            {
                Health = healthData
            };

            SendData(PacketIDs.PlayerHealth, data);
        }

        internal static void SendColor(UnityEngine.Color32 color)
        {
            Log.LogDebug($"Sending color: {color}");

            var data = new ColorPacket
            {
                Color =
                {
                    r = color.r,
                    g = color.g,
                    b = color.b,
                }
            };

            SendData(PacketIDs.Color, data);
        }
    }
}

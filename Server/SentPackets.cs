using System;
using SSMP.Networking.Packet;
using SSMP.Networking.Packet.Data;
using SSMPEssentials.Client.Packets;
using SSMPEssentials.Server.Modules;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Server.Packets
{
    internal class PlayerHealthPacket : HealthPacket
    {
        public ushort PlayerId;

        public override void WriteData(IPacket packet)
        {
            base.WriteData(packet);
            packet.Write(PlayerId);
        }

        public override void ReadData(IPacket packet)
        {
            base.ReadData(packet);
            PlayerId = packet.ReadUShort();
        }
    }

    internal class SettingsPacket : Packet
    {
        public ServerSettings ServerSettings = null!;

        public override void WriteData(IPacket packet)
        {
            foreach (var prop in ServerSettings.GetProps())
            {
                if (!prop.CanRead) continue;
                
                packet.Write((bool)prop.GetValue(ServerSettings, null));
            }
        }

        public override void ReadData(IPacket packet)
        {
            ServerSettings = new(false);
            foreach (var prop in ServerSettings.GetProps())
            {
                if (!prop.CanWrite) continue;
                prop.SetValue(ServerSettings, packet.ReadBool(), null);
            }

#if DEBUG
            var json = JsonConvert.SerializeObject(ServerSettings, Formatting.Indented);
            Log.LogWarning(json);
#endif
        }
    }
    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            return packetID switch
            {
                PacketIDs.Huddle => new TeleportPacket(),
                PacketIDs.TeleportRequest => new PacketDataCollection<TeleportRequestPacket>(),
                PacketIDs.TeleportAccept => new TeleportPacket(),
                PacketIDs.Message => new PacketDataCollection<MessagePacket>(),
                PacketIDs.PlayerHealth => new PacketDataCollection<PlayerHealthPacket>(),
                PacketIDs.Settings => new SettingsPacket(),
                _ => new ErrorThrowingPacket(packetID, true)
            };
        }

        internal class ErrorThrowingPacket : Packet
        {
            public ErrorThrowingPacket(PacketIDs id, bool server)
            {
                Log.LogError(id.ToString(), server);
                throw new NotImplementedException(id.ToString());
            }
        }
    }
}

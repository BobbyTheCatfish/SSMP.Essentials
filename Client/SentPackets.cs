using SSMP.Math;
using SSMP.Networking.Packet;
using SSMPEssentials.Data;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Packets
{
    internal class TeleportPacket : Packet
    {        
        public ushort PlayerId;
        public string Scene = "";
        public Vector2 Position = Vector2.Zero;

        public override void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
            packet.Write(Scene);
            packet.Write(Position);
        }

        public override void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
            Scene = packet.ReadString();
            Position = packet.ReadVector2();
        }
    }

    internal class MessagePacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public ushort PlayerId;

        public Messages Message;

        public void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
            packet.Write((ushort)Message);
        }

        public void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
            Message = (Messages)packet.ReadUShort();
        }
    }

    internal class TeleportRequestPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public ushort PlayerId;
        public void WriteData(IPacket packet)
        {
            packet.Write(PlayerId);
        }

        public void ReadData(IPacket packet)
        {
            PlayerId = packet.ReadUShort();
        }
    }

    internal class DeathPacket : IPacketData
    {
        public bool IsReliable => true;
        public bool DropReliableDataIfNewerExists => true;

        public ushort KillerID;
        public CauseOfDeath Cause = CauseOfDeath.Generic;
        public bool RanAway = false;
        public string Scene = "";
        public virtual void WriteData(IPacket packet)
        {
            packet.Write(KillerID);
            packet.Write((ushort)Cause);
            packet.Write(RanAway);
            packet.Write(Scene);
        }

        public virtual void ReadData(IPacket packet)
        {
            KillerID = packet.ReadUShort();
            Cause = (CauseOfDeath)packet.ReadUShort();
            RanAway = packet.ReadBool();
            Scene = packet.ReadString();
        }
    }

    public class HealthPacket : Packet
    {
        public HealthData Health = new();
        public override void WriteData(IPacket packet)
        {
            packet.Write(Health.Health);
            packet.Write(Health.MaxHealth);
            packet.Write(Health.BlueHealth);
            packet.Write(Health.LifebloodState);
        }

        public override void ReadData(IPacket packet)
        {
            Health = new();

            Health.Health = packet.ReadInt();
            Health.MaxHealth = packet.ReadInt();
            Health.BlueHealth = packet.ReadInt();
            Health.LifebloodState = packet.ReadBool();
        }
    }

    internal class ColorPacket : Packet
    {
        public ColorLite Color;

        public override void WriteData(IPacket packet)
        {
            packet.Write(Color.r);
            packet.Write(Color.g);
            packet.Write(Color.b);
        }

        public override void ReadData(IPacket packet)
        {
            Color = new ColorLite
            {
                r = packet.ReadByte(),
                g = packet.ReadByte(),
                b = packet.ReadByte()
            };
        }
    }

    public static class Packets
    {
        internal static IPacketData Instantiate(PacketIDs packetID)
        {
            return packetID switch
            {
                PacketIDs.Huddle => new TeleportPacket(),
                PacketIDs.TeleportRequest => new TeleportRequestPacket(),
                PacketIDs.TeleportAccept => new TeleportPacket(),
                PacketIDs.Message => new MessagePacket(),
                PacketIDs.PlayerDeath => new DeathPacket(),
                PacketIDs.PlayerHealth => new HealthPacket(),
                PacketIDs.Color => new ColorPacket(),
                _ => new Server.Packets.Packets.ErrorThrowingPacket(packetID, false)
            };
        }
    }
}

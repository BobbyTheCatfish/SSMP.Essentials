using SSMP.Api.Client;
using SSMP.Api.Server;
using SSMP.Game;
using SSMP.Networking.Packet;

namespace SSMPEssentials.Utils
{
    internal class Common
    {
        public static string ServerTextColor(string text, Colors color)
        {
            var colorStr = color switch
            {
                Colors.White => "&f",
                Colors.Black => "&1",
                Colors.Orange => "&6",
                Colors.Purple => "&5",
                Colors.Blue => "&b",
                Colors.Green => "&2",
                Colors.Red => "&4",
                Colors.Yellow => "&e",
                _ => "&f"
            };

            return $"{colorStr}{text}&r";
        }

        public static string LocalTextColor(string text, Colors color)
        {
            var colorStr = color switch
            {
                Colors.White => "#FFFFFF",
                Colors.Black => "#000000",
                Colors.Orange => "#FFAA00",
                Colors.Purple => "#AA00AA",
                Colors.Blue => "#55FFFF",
                Colors.Green => "#00AA00",
                Colors.Red => "#AA0000",
                Colors.Yellow => "#FFFF55",
                _ => "#FFFFFF"
            };

            return $"<color={colorStr}>{text}</color>";
        }

        public static string ColoredUsername(IServerPlayer? player, Colors defaultColor = Colors.White)
        {
            if (player == null) return ServerTextColor("Unknown Player", defaultColor);

            var username = player.Username;
            return player.Team switch
            {
                Team.Lifeblood => ServerTextColor(username, Colors.Blue),
                Team.Moss => ServerTextColor(username, Colors.Green),
                Team.Grimm => ServerTextColor(username, Colors.Red),
                Team.Hive => ServerTextColor(username, Colors.Orange),
                _ => ServerTextColor(username, defaultColor),
            };
        }

        public static string ColoredUsername(IClientPlayer? player, Colors defaultColor = Colors.White)
        {
            if (player == null) return LocalTextColor("Unknown Player", defaultColor);

            var username = player.Username;
            return player.Team switch
            {
                Team.Lifeblood => LocalTextColor(username, Colors.Blue),
                Team.Moss => LocalTextColor(username, Colors.Green),
                Team.Grimm => LocalTextColor(username, Colors.Red),
                Team.Hive => LocalTextColor(username, Colors.Orange),
                _ => LocalTextColor(username, defaultColor),
            };
        }
    }

    public class Packet : IPacketData
    {
        public virtual bool IsReliable => true;
        public virtual bool DropReliableDataIfNewerExists => true;

        public virtual void WriteData(IPacket packet)
        {
            Log.LogInfo("THIS SHOULD NOT RUN");
        }
        public virtual void ReadData(IPacket packet)
        {
            Log.LogInfo("THIS SHOULD NOT RUN");
        }
    }
}

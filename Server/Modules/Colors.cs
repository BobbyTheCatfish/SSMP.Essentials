using SSMP.Api.Server;
using SSMPEssentials.Utils;
using System.Collections.Generic;

namespace SSMPEssentials.Server.Modules
{
    internal class Colors
    {
        private readonly Dictionary<ushort, ColorLite> colors = [];

        public ColorLite GetColor(ushort id)
        {
            if (colors.TryGetValue(id, out var color))
            {
                return color;
            }

            return new ColorLite
            {
                r = 255,
                g = 255,
                b = 255,
            };
        }

        public void SetColor(ushort id, ColorLite color)
        {
            colors[id] = color;
        }

        public void OnPlayerJoin(ushort id)
        {
            foreach (var color in colors)
            {
                PacketSender.SendColor(color.Key, id, color.Value);
            }
        }

        public void OnPlayerLeave(IServerPlayer player)
        {
            colors.Remove(player.Id);
        }
    }
}

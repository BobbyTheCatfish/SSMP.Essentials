using System.Linq;
using SSMP.Api.Command.Client;

namespace SSMPEssentials.Client.Commands
{
    internal class Huddle : IClientCommand
    {
        public string Trigger => "/huddle";
        public string[] Aliases => ["/tpall"];
        public void Execute(string[] arguments)
        {
            var args = arguments.ToList();
            args.RemoveAt(0);
            var username = string.Join(" ", args);

            // Base case, no username
            if (args.Count <= 0)
            {
                PacketSender.SendHuddle();
                return;
            }

            // Huddle to a specific user
            var player = Client.GetPlayerByName(username);
            if (player != null)
            {
                PacketSender.SendHuddle(player.Id);
                return;
            }

            // Attempt failed
            Client.LocalChat($"Player '{username}' not found.");
            return;
        }
    }
}

using SSMP.Api.Command.Client;
using UnityEngine;
using SSMPEssentials.Client.Modules;

namespace SSMPEssentials.Client.Commands
{
    internal class ColorCommand : IClientCommand
    {
        public string Trigger => "/color";
        public string[] Aliases => ["/usercolor", "/namecolor"];

        public void Execute(string[] arguments)
        {
            // Disabled
            if (!ColoredNames.ColorsEnabled())
            {
                Client.LocalChat("Colors are currently disabled.");
                return;
            }

            // Invalid syntax
            var syntax = $"Invalid Syntax. {Trigger} <css color>";
            if (arguments.Length < 2)
            {
                Client.LocalChat(syntax);
                return;
            }

            if (!ColorUtility.TryParseHtmlString(arguments[1], out var color))
            {
                Client.LocalChat("That color could not be parsed. Try a hex code or standard CSS color.");
                return;
            }

            ColoredNames.SetOwnTextColor(color);

            PacketSender.SendColor(color);
        }

    }
}

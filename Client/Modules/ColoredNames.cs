using SSMP.Api.Client;
using SSMPEssentials.Utils;
using System.Collections;
using System.Collections.Generic;
using TMProOld;
using UnityEngine;

namespace SSMPEssentials.Client.Modules
{
    internal static class ColoredNames
    {
        private static readonly Dictionary<ushort, Color> playerColors = [];

        private static Color selfColor = Color.white;

        public static bool ColorsEnabled()
        {
            if (!Client.ServerSettings.ColoredUsernames) return false;
            if (Client.api.ClientManager.ServerSettings.TeamsEnabled)
            {
                if (!Client.ServerSettings.ColorsOverrideTeams) return false;
            }

            return true;
        }

        public static void OnJoin()
        {
            playerColors.Clear();
            selfColor = Color.white;
        }

        public static void Init()
        {
            Client.OnServerSettingsUpdate += CheckResetAll;
            Client.api.ClientManager.ServerSettings.ChangeEvent += (e) => CheckResetAll();
        }

        public static void SetPlayerColor(IClientPlayer player, Color color)
        {
            playerColors[player.Id] = color;
            ChangeTextColor(player);
        }

        private static Color ConvertColor(ColorLite color)
        {
            if (!ColorUtility.TryParseHtmlString(color.ToHtmlString(), out var newColor)) {
                newColor = Color.white;
            }

            return newColor;
        }

        public static void OnColorReceived(ushort id, ColorLite color, bool isSelf)
        {
            var convertedColor = ConvertColor(color);

            playerColors[id] = convertedColor;

            var player = Client.GetPlayer(id);
            if (player != null && player.IsInLocalScene)
            {
                ChangeTextColor(player);
            }
        }

        public static void HeroSetHook(HeroController hc)
        {
            HeroController.OnHeroInstanceSet -= HeroSetHook;

            // Wait for username object to exist
            static IEnumerator Routine()
            {
                yield return new WaitForEndOfFrame();

                SetOwnTextColor(Config.DefaultColor);
                PacketSender.SendColor(Config.DefaultColor);
            }

            SSMPEssentialsPlugin.instance.StartCoroutine(Routine());
        }

        public static void SetOwnTextColor(Color color)
        {
            selfColor = color;
            if (!HeroController.SilentInstance) return;

            var username = HeroController.instance.gameObject.FindGameObjectInChildren("Username");
            if (!username) return;

            if (!ColorsEnabled()) return;

            if (username.TryGetComponent<TextMeshPro>(out var text))
            {
                text.color = color;
            }
        }

        public static void ChangeTextColor(IClientPlayer player)
        {
            var container = player.PlayerContainer;
            if (!container) return;

            var username = container.FindGameObjectInChildren("Username");
            if (!username) return;

            if (!playerColors.TryGetValue(player.Id, out var color)) {
                color = Color.white;
            }

            if (!ColorsEnabled()) return;

            ChangeTextColor(username, color);
        }

        static void ChangeTextColor(GameObject text, Color color)
        {
            if (text.TryGetComponent<TextMeshPro>(out var textComponent))
            {
                textComponent.color = color;
            }
        }

        public static void ResetPlayerColor(IClientPlayer player, bool remove)
        {
            SetPlayerColor(player, Color.white);
            
            if (remove) playerColors.Remove(player.Id);
        }

        private static void CheckResetAll()
        {
            if (!ColorsEnabled()) ResetAll();
            else SetAll();
        }

        private static void ResetAll()
        {
            Log.LogInfo("Resetting all");
            foreach (var id in playerColors.Keys)
            {
                var player = Client.GetPlayer(id);

                var container = player?.PlayerContainer;
                if (!container) continue;

                var username = container.FindGameObjectInChildren("Username");
                if (!username) continue;

                ChangeTextColor(username, Color.white);
            }

            var ownUsername = HeroController.instance.gameObject.FindGameObjectInChildren("Username");
            if (!ownUsername) return;

            ChangeTextColor(ownUsername, Color.white);
        }

        private static void SetAll()
        {
            Log.LogInfo("Setting all");
            foreach (var id in playerColors.Keys)
            {
                var player = Client.GetPlayer(id);
                if (player != null) ChangeTextColor(player);
            }

            SetOwnTextColor(selfColor);
        }
    }
}

using System;
using System.Linq;
using GlobalEnums;
using UnityEngine.SceneManagement;
using SSMPEssentials.Client.Commands;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Modules
{
    internal static class PlayerDeaths
    {
        public static CauseOfDeath LatestCause = CauseOfDeath.Generic;
        public static ushort LatestPlayerAttack = 0;
        public static DateTime LastDamageTime = DateTime.Now;
        public static DateTime PlayerAttackTime = DateTime.MinValue;

        public static void Init()
        {
            var damagedFsmRegisters = HeroController.instance.gameObject.GetComponents<EventRegister>();
            var damageRegister = damagedFsmRegisters.FirstOrDefault(r => r.subscribedEvent == "HERO DAMAGED");
            if (damageRegister != null)
            {
                damageRegister.ReceivedEvent += LastDitchEffort;
            }
        }
        
        private static void LastDitchEffort()
        {
            if (LastDamageTime.AddSeconds(1) < DateTime.Now)
            {
                LastDamageTime = DateTime.Now;
                LatestCause = CauseOfDeath.Generic;
            }
        }

        public static void OnDeath()
        {
            if (!Client.ServerSettings.DeathMessagesEnabled) return;

            var ranAway = PlayerAttackTime.AddSeconds(15) > DateTime.Now;

            PacketSender.SendDeath(LatestCause, LatestPlayerAttack, ranAway);

            var currentScene = SceneManager.GetActiveScene().name;
            var hornet = HeroController.SilentInstance ? HeroController.SilentInstance.gameObject : null;

            // Check if hornet even exists
            if (hornet == null) return;

            LatestCause = CauseOfDeath.Generic;
            LatestPlayerAttack = 0;

            TeleportBack.PreviousScene = currentScene;
            TeleportBack.PreviousLocation = hornet.transform.position;
        }

        public static void DetermineCauseOfDamage(DamageHero damager)
        {
            LastDamageTime = DateTime.Now;
            if (damager.GetComponentInParent<FireballProjectile>())
            {
                LatestCause = CauseOfDeath.Coal;
                return;
            }

            if (damager.CompareTag("Explosion"))
            {
                LatestCause = CauseOfDeath.Explosion;
                return;
            }

            bool isPlayer = false;
            var parent = damager.transform;
            while (parent != null && !isPlayer)
            {
                if (parent.name == "Player Prefab") isPlayer = true;
                else parent = parent.parent;
            }

            Log.LogDebug(isPlayer, damager.name);
            if (isPlayer && parent)
            {
                LatestCause = CauseOfDeath.Player;

                var player = Client.api.ClientManager.Players.FirstOrDefault(p => p.PlayerObject == parent.gameObject);
                if (player == null)
                {
                    Log.LogWarning("Attacked by unknown player");
                    return;
                }
                LatestPlayerAttack = player?.Id ?? 0;
                PlayerAttackTime = DateTime.Now;
                return;
            }

            LatestCause = DetermineHazardType(damager.hazardType);
        }

        static CauseOfDeath DetermineHazardType(HazardType hazardType)
        {
            return hazardType switch
            {
                HazardType.ENEMY => CauseOfDeath.Enemy,
                HazardType.SPIKES => CauseOfDeath.Spikes,
                HazardType.ACID => CauseOfDeath.Acid,
                HazardType.LAVA => CauseOfDeath.Lava,
                HazardType.PIT => CauseOfDeath.Pit,
                HazardType.COAL => CauseOfDeath.Coal,
                HazardType.ZAP => CauseOfDeath.Zap,
                HazardType.EXPLOSION => CauseOfDeath.Explosion,
                HazardType.SINK => CauseOfDeath.Sink,
                HazardType.STEAM => CauseOfDeath.Steam,
                HazardType.RESPAWN_PIT => CauseOfDeath.Generic,
                _ => CauseOfDeath.Generic
            };
        }
    }
}

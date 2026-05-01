using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SSMP.Game.Settings;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Server.Modules
{
    internal class ServerSettings
    {
        [JsonProperty("huddle")]
        [SettingAlias("huddle")]
        public bool HuddleEnabled { get; set; } = true;
        
        [JsonProperty("teleport")]
        [SettingAlias("tp", "teleport")]
        public bool TeleportsEnabled { get; set; } = true;
        
        [JsonProperty("back")]
        [SettingAlias("back", "tpback")]
        public bool BackEnabled { get; set; } = true;
        
        [JsonProperty("teleport_requests")]
        [SettingAlias("tprequests", "tpa", "tpd", "tpr", "tpreq")]
        public bool TeleportsNeedRequests { get; set; } = true;
        
        [JsonProperty("death_messages")]
        [SettingAlias("deathmessages", "death")]
        public bool DeathMessagesEnabled { get; set; } = true;
        
        [JsonProperty("healthbars")]
        [SettingAlias("healthbars", "health")]
        public bool HealthbarsEnabled { get; set; } = true;

        [JsonProperty("spectate")]
        [SettingAlias("spectate")]
        public bool SpectateEnabled { get; set; } = true;

        [JsonProperty("spectate_team")]
        [SettingAlias("spectateteam")]
        public bool SpectateTeamOnly { get; set; } = false;
        

        [JsonProperty("freecam")]
        [SettingAlias("freecam")]
        public bool FreecamEnabled { get; set; } = true;

        [JsonProperty("colors")]
        [SettingAlias("colors")]
        public bool ColoredUsernames { get; set; } = true;

        [JsonProperty("colors_over_teams")]
        [SettingAlias("coloroverride", "colorbypass")]
        public bool ColorsOverrideTeams { get; set; } = false;


        private static string Filepath()
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(dir, "ServerSettings.json");
        }

        public static ServerSettings ReadFromFile()
        {
            var path = Filepath();
            if (!File.Exists(path))
            {
                Log.LogWarning($"{path} doesn't exist");
                new ServerSettings(false).WriteToFile();
                return new ServerSettings(false);
            }

            try
            {
                var fileContents = File.ReadAllText(path);
                var settings = JsonConvert.DeserializeObject<ServerSettings>(fileContents);
                return settings ?? new ServerSettings(false);
            }
            catch (Exception e)
            {
                Log.LogError($"Could not load server settings from file:\n{e}");
                return new ServerSettings(false);
            }
        }

        public void WriteToFile()
        {
            var path = Filepath();
            if (!Directory.Exists(Path.GetDirectoryName(path)))
            {
                Log.LogWarning($"{path} directory doesn't exist");
                return;
            }

            var settings = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (settings == null) return;

            try
            {
                File.WriteAllText(path, settings);
            }
            catch (Exception e)
            {
                Log.LogError($"Could not write server settings to file:\n{e}");
            }
        }

        public IEnumerable<PropertyInfo> GetProps()
        {
            var props = typeof(ServerSettings).GetProperties();
            foreach (var prop in props)
            {
                yield return prop;
            }
        }

        public ServerSettings(bool client)
        {
            var props = GetProps();
            foreach (var prop in props)
            {
                prop.SetValue(this, !client);
            }
        }
    }
}

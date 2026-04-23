using UnityEngine.SceneManagement;
using SSMP.Math;
using SSMPEssentials.Client.Commands;
using SSMPEssentials.Utils;

namespace SSMPEssentials.Client.Modules
{
    internal class Warp
    {
        static bool teleporting = false;
        string scene;
        UnityEngine.Vector2 position;

        public Warp(string scene, UnityEngine.Vector2 position)
        {
            this.scene = scene;
            this.position = position;
        }

        void SetHornetPosition()
        {
            GameManager.instance.OnFinishedEnteringScene -= SetHornetPosition;
            Log.LogDebug("Setting hornet position");

            var hornet = HeroController.SilentInstance ? HeroController.SilentInstance.gameObject : null;
            if (hornet != null) hornet.transform.SetPosition2D(position);

            teleporting = false;
        }

        public void WarpToPosition()
        {
            var currentScene = SceneManager.GetActiveScene().name;
            var hornet = HeroController.SilentInstance ? HeroController.SilentInstance.gameObject : null;

            // Check if hornet even exists
            if (hornet == null || teleporting)
            {
                FailedWarp(scene);
                return;
            }

            TeleportBack.PreviousScene = currentScene;
            TeleportBack.PreviousLocation = hornet.transform.position;

            teleporting = true;

            // No scene changes required
            if (scene == currentScene)
            {
                SetHornetPosition();
                return;
            }

            WarpToScene();
        }

        void WarpToScene()
        {
            // Get transition gate
            SceneTeleportMap.GetTeleportMap().TryGetValue(scene, out var teleport);
            if (teleport == null)
            {
                FailedWarp(scene);
                return;
            }

            GameManager.SceneLoadInfo loadInfo = new()
            {
                SceneName = scene,
                EntryGateName = teleport.TransitionGates[0],
                PreventCameraFadeOut = true,
                WaitForSceneTransitionCameraFade = false,
                AlwaysUnloadUnusedAssets = true,
                IsFirstLevelForPlayer = false
            };

            GameManager.instance.BeginSceneTransition(loadInfo);
            GameManager.instance.OnFinishedEnteringScene += SetHornetPosition;
        }
        
        static void FailedWarp(string scene)
        {
            Client.LocalChat($"I couldn't warp you. Please join the rest of the server in {scene}.");
        }

        public static bool GetHornetScenePosition(out string scene, out Vector2 position)
        {
            scene = SceneManager.GetActiveScene().name;
            position = Vector2.Zero;

            if (GameManager.SilentInstance == null || GameManager.SilentInstance.GameState != GlobalEnums.GameState.PLAYING)
            {
                Client.LocalChat("Resume the game first!");
                return false;
            }

            if (scene == null)
            {
                Client.LocalChat("I couldn't find the current scene!");
                Log.LogError("ACTUAL ERROR: Unable to find current scene.");
                return false;
            }

            var hornet = HeroController.SilentInstance ? HeroController.SilentInstance.gameObject : null;
            if (hornet == null)
            {
                Client.LocalChat("I couldn't find your position. Uh oh!");
                Log.LogError("ACTUAL ERROR: Unable to find hornet object");
                return false;
            }

            position = (Vector2)(UnityEngine.Vector2)hornet.transform.position;
            return true;
        }
    }
}

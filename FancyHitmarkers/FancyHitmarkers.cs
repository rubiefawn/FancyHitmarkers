using BepInEx;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace rubiefawn
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    public class FancyHitmarkers : BaseUnityPlugin
    {
        public const string PluginGUID = $"{PluginAuthor}.{PluginName}";
        public const string PluginAuthor = "rubiefawn";
        public const string PluginName = "FancyHitmarkers";
        public const string PluginVersion = "1.1.2";

        private static readonly Color critcolor = new(1.0f, 0.75f, 0.0f);
        private static readonly Color killcolor = new(1.0f, 0.0f, 0.0f);

        private const float phi = 1.618f;
        private const float basetime = 0.15f;
        private const float crittime = basetime * phi * phi;
        private const float killtime = basetime * phi * phi * phi;

        private static void ApplyHitmarkerColor(
            On.RoR2.UI.CrosshairManager.orig_HandleHitMarker orig,
            DamageDealtMessage dmg
        )
        {
            (Color color, float time) = (!dmg.victim?.GetComponent<HealthComponent>().alive ?? true, dmg.crit) switch
            {
                (true, _) => (killcolor, killtime),
                (_, true) => (critcolor, crittime),
                _ => (Color.white, basetime)
            };

            orig(dmg);

            foreach (CrosshairManager x in CrosshairManager.instancesList)
            {
                if (dmg.attacker == x.cameraRigController?.target)
                {
                    x.hitmarker.color = color;
                    x.hitmarkerTimer = time;
                    x.hitmarker.gameObject.SetActive(false);
                    x.hitmarker.gameObject.SetActive(true);
                }
            }
        }

        private void OnEnable()
        {
            On.RoR2.UI.CrosshairManager.HandleHitMarker += ApplyHitmarkerColor;
        }

        private void OnDisable()
        {
            On.RoR2.UI.CrosshairManager.HandleHitMarker -= ApplyHitmarkerColor;
        }

    }
}

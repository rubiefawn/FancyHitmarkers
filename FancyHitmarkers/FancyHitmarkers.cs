using BepInEx;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace rubiefawn {
	[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
	public class FancyHitmarkers : BaseUnityPlugin {
		public const string PluginGUID = PluginAuthor + "." + PluginName;
		public const string PluginAuthor = "rubiefawn";
		public const string PluginName = "FancyHitmarkers";
		public const string PluginVersion = "1.1.1";

		private static Color critcolor = new Color(1f, 0.75f, 0f);
		private static Color killcolor = new Color(1f, 0f, 0f);

		private const float phi = 1.618f;
        private const float basetime = 0.15f;
		private const float crittime = basetime * phi * phi;
		private const float killtime = basetime * phi * phi * phi;

		private void OnEnable() {
			On.RoR2.UI.CrosshairManager.HandleHitMarker += ColorHitmarkers;
			On.RoR2.UI.CrosshairManager.UpdateHitMarker += AdjustHitmarkerAlpha;
		}

		private void OnDisable() {
			On.RoR2.UI.CrosshairManager.HandleHitMarker -= ColorHitmarkers;
			On.RoR2.UI.CrosshairManager.UpdateHitMarker -= AdjustHitmarkerAlpha;
		}

		private void AdjustHitmarkerAlpha(On.RoR2.UI.CrosshairManager.orig_UpdateHitMarker orig, CrosshairManager self) {
			orig(self);
			self.hitmarkerAlpha = Mathf.Pow(self.hitmarkerTimer * 6.666f, 0.666f);
			Color temp = self.hitmarker.color; // i hate properties, just let me set color.a for the love of god
			temp.a = self.hitmarkerAlpha;
			self.hitmarker.color = temp;
		}

		private static void ColorHitmarkers(On.RoR2.UI.CrosshairManager.orig_HandleHitMarker orig, DamageDealtMessage dmg) {
			Color color = Color.white;
			float time = basetime;
			if (!dmg.victim?.GetComponent<HealthComponent>().alive ?? true) {
				color = killcolor;
				time = killtime;
			} else if (dmg.crit) {
				color = critcolor;
				time = crittime;
            }

			orig(dmg);

			for (int i = 0; i < CrosshairManager.instancesList.Count; i++) {
				var it = CrosshairManager.instancesList[i];
				if (dmg.attacker == it.cameraRigController?.target) {
					it.hitmarker.color = color;
					it.hitmarkerTimer = time;
					it.hitmarker.gameObject.SetActive(false);
					it.hitmarker.gameObject.SetActive(true);
				}
            }
		}

	}
}

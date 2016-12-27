using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public class ModuleLightEVAToggle : PartModule
	{
		private List<ModuleLight> moduleLight;
		private bool isStockLight = true;
		private List<PartModule> notStockLight;
		// For now not stock light represent only WBILight

		public override void OnStart (StartState state)
		{
			if (part.Modules.Contains<ModuleLight> ()) {
				moduleLight = part.Modules.GetModules<ModuleLight> ();
			} else if (part.Modules.Contains("WBILight")) {
				isStockLight = false;
				notStockLight = new List<PartModule> ();
				foreach (PartModule pM in part.Modules) {
					if (pM.ClassName == "WBILight") {
						notStockLight.Add (pM);
					}
				}
			}
		}

		[KSPEvent (active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiName = "Toggle Light")]
		public void LightToggleEVA ()
		{
			if (isStockLight) {
				foreach (ModuleLight light in moduleLight) {
					if (light.isOn) {
						light.LightsOff ();
					} else {
						light.LightsOn ();
					}
				}
			} else {
				foreach (PartModule pM in notStockLight) {
					pM.SendMessage ("ToggleLightsAction");
				}
			}
		}
	}
}


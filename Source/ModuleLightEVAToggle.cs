using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public class ModuleLightEVAToggle : PartModule
	{
		private List<ModuleLight> moduleLight;

		public override void OnStart (StartState state)
		{
			if (part.Modules.Contains<ModuleLight> ()) {
				moduleLight = part.Modules.GetModules<ModuleLight> ();
			}
		}

		[KSPEvent (active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiName = "Toggle Light")]
		public void LightToggleEVA ()
		{
			foreach (ModuleLight light in moduleLight) {
				if (light.isOn) {
					light.LightsOff ();
				} else {
					light.LightsOn ();
				}
			}
		}

		private void D (String str)
		{
			Debug.Log ("[Crew Light - ModuleLightEVAToggle] : " + str);
		}
	}
}


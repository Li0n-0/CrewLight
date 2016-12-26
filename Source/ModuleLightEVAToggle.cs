using System;
using UnityEngine;

namespace CrewLight
{
	public class ModuleLightEVAToggle : PartModule
	{
		private ModuleLight moduleLight;

		public override void OnStart (StartState state)
		{
			moduleLight = part.Modules.GetModule<ModuleLight> ();
		}

		[KSPEvent (active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiName = "Toggle Light")]
		public void LightToggleEVA ()
		{
			if (moduleLight.isOn) {
				moduleLight.LightsOff ();
			} else {
				moduleLight.LightsOn ();
			}
		}
	}
}


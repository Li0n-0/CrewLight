using System;
using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	public class ModuleLightEVAToggle : PartModule
	{
		private List<Part> ogSymPart;

		public override void OnStart (StartState state)
		{
			if ((part.Modules.Contains<ModuleLight> () || part.Modules.Contains ("ModuleKELight")) && CLSettings.useVesselLightsOnEVA) {
				ogSymPart = new List<Part> (part.symmetryCounterparts);
			} else {
				Destroy (this);
			}
		}

		[KSPEvent (active = true, guiActiveUnfocused = true, externalToEVAOnly = true, guiName = "Toggle Light")]
		public void LightToggleEVA ()
		{
			if (! CLSettings.lightSymLights) {
				// Remove the symmetry counter parts before lightning, then add them back
				part.symmetryCounterparts.Clear ();
			}

			if (part.Modules.Contains<ModuleLight> ()) {
				List<ModuleLight> lights = part.Modules.GetModules<ModuleLight> ();
				foreach (ModuleLight light in lights) {
					if (light.isOn) {
						SwitchLight.Off (light);
					} else {
						SwitchLight.On (light);
					}
				}
			}
			if (part.Modules.Contains ("ModuleKELight")) {
				foreach (PartModule partM in part.Modules) {
					if (partM.ClassName == "ModuleKELight") {
						if ((bool)partM.GetType ().InvokeMember ("isOn", System.Reflection.BindingFlags.GetField, null, partM, null)) {
							SwitchLight.Off (part);
						} else {
							SwitchLight.On (part);
						}
					}
				}
			}

			if (! CLSettings.lightSymLights) {
				part.symmetryCounterparts = ogSymPart;
			}
		}

		private void D (String str)
		{
			Debug.Log ("[Crew Light - ModuleLightEVAToggle] : " + str);
		}
	}
}


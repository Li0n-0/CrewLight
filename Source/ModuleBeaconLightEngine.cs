using System;
using UnityEngine;

namespace CrewLight
{
	public class ModuleBeaconLightEngine : PartModule
	{
		private PartModule navLight;
		private bool isOn = false;

		public void Start ()
		{
			if (! HighLogic.LoadedSceneIsFlight || ! CLSettings.beaconOnEngine) {
				Destroy (this);
			}

			foreach (PartModule pm in part.Modules) {
				if (pm.ClassName == "ModuleNavLight") {
					navLight = pm;
					break;
				}
			}
		}

		public void FixedUpdate ()
		{
			if (part.vessel.ctrlState.mainThrottle > 0 && ! isOn) {
				SwitchLight.On (navLight);
				isOn = true;
				return;
			} 
			if (part.vessel.ctrlState.mainThrottle == 0 && isOn){
				SwitchLight.Off (navLight);
				isOn = false;
			}
		}
	}
}


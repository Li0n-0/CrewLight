using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrewLight
{
	public static class SwitchLight
	{
		public static void On (PartModule light)
		{
			switch (light.moduleName) {
			case "ModuleColorChanger":
				if (! light.GetComponent<ModuleColorChanger> ().animState) {
					light.SendMessage ("ToggleEvent");
				}
				break;
			case "ModuleLight":
			case "ModuleStockLightColoredLens":
			case "ModuleMultiPointSurfaceLight":
			case "ModuleColoredLensLight":
				light.SendMessage ("LightsOn");
				break;
			case "ModuleAnimateGeneric":
				if (light.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
					light.SendMessage ("Toggle");
				}
				break;
			case "WBILight":
				light.SendMessage ("TurnOnLights");
				break;
			case "ModuleNavLight":
				if (CLSettings.useAviationLightsEffect && CLSettings.inSunlight) {
					switch (light.part.name) {
					case "lightbeacon.amber":
						light.SendMessage (AviationLightsParser (CLSettings.beaconAmber));
						break;
					case "lightbeacon.red":
						light.SendMessage (AviationLightsParser (CLSettings.beaconRed));
						break;
					case "lightnav.blue":
						light.SendMessage (AviationLightsParser (CLSettings.navBlue));
						break;
					case "lightnav.green":
						light.SendMessage (AviationLightsParser (CLSettings.navGreen));
						break;
					case "lightnav.red":
						light.SendMessage (AviationLightsParser (CLSettings.navRed));
						break;
					case "lightnav.white":
						light.SendMessage (AviationLightsParser (CLSettings.navWhite));
						break;
					case "lightstrobe.white":
						light.SendMessage (AviationLightsParser (CLSettings.strobeWhite));
						break;
					}
				} else {
					light.SendMessage ("LightOn");
				}

				break;
			}
		}

		private static string AviationLightsParser (int i)
		{
			switch (i) {
			case 0:
				return "LightOff";
			case 1:
				return "LightFlash";
			case 2:
				return "LightDoubleFlash";
			case 3:
				return "LightInterval";
			case 4:
				return "LightOn";
			default:
				return "LightOn";
			}
		}

		public static void Off (PartModule light)
		{
			switch (light.moduleName) {
			case "ModuleColorChanger":
				if (light.GetComponent<ModuleColorChanger> ().animState) {
					light.SendMessage ("ToggleEvent");
				}
				break;
			case "ModuleLight":
			case "ModuleStockLightColoredLens":
			case "ModuleMultiPointSurfaceLight":
			case "ModuleColoredLensLight":
				light.SendMessage ("LightsOff");
				break;
			case "ModuleAnimateGeneric":
				if (! light.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
					light.SendMessage ("Toggle");
				}
				break;
			case "WBILight":
				light.SendMessage ("TurnOffLights");
				break;
			case "ModuleNavLight":
				light.SendMessage ("LightOff");
				break;
			}
		}

		public static void On (Part part)
		{
			On (GetLightModule (part));
		}

		public static void Off (Part part) 
		{
			Off (GetLightModule (part));
		}

		public static void On (List<PartModule> modulesLight)
		{
			foreach (PartModule light in modulesLight) {
				On (light);
			}
		}

		public static void Off (List<PartModule> modulesLight)
		{
			foreach (PartModule light in modulesLight) {
				Off (light);
			}
		}

		private static List<PartModule> GetLightModule (Part part)
		{
			List<PartModule> lightList = new List<PartModule> ();

			if (part.Modules.Contains<ModuleColorChanger> ()) {
				foreach (ModuleColorChanger module in part.Modules.GetModules<ModuleColorChanger> ()) {
					if (Regex.IsMatch(module.toggleName, "light", RegexOptions.IgnoreCase)) {
						lightList.Add (module);
					}
				}
			}
			if (part.Modules.Contains<ModuleLight> ()) {
				foreach (ModuleLight module in part.Modules.GetModules<ModuleLight> ()) {
					lightList.Add (module);
				}
			}
			if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
				foreach (ModuleAnimateGeneric module in part.Modules.GetModules<ModuleAnimateGeneric> ()) {
					if (Regex.IsMatch(module.actionGUIName, "light", RegexOptions.IgnoreCase)) {
						lightList.Add (module);
					}
				}
			}
			if (part.Modules.Contains ("WBILight")) {
				foreach (PartModule module in part.Modules) {
					if (module.moduleName == "WBILight") {
						lightList.Add (module);
					}
				}
			}

			return lightList;
		}

		private static void D (String str)
		{
			Debug.Log ("[Crew Light - SwitchLight] : " + str);
		}
	}
}


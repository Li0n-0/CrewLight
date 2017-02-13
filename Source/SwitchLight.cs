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
			case "ModuleColorChangerConsumer":
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
			case "ModuleAnimateGenericConsumer":
				if (light.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
					light.SendMessage ("Toggle");
				}
				break;
			case "WBILight":
				light.SendMessage ("TurnOnLights");
				break;
			}
		}

		public static void Off (PartModule light)
		{
			switch (light.moduleName) {
			case "ModuleColorChanger":
			case "ModuleColorChangerConsumer":
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
			case "ModuleAnimateGenericConsumer":
				if (! light.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
					light.SendMessage ("Toggle");
				}
				break;
			case "WBILight":
				light.SendMessage ("TurnOffLights");
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


using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
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
				ModuleColorChanger castMCC = (ModuleColorChanger)light;
				if (! castMCC.animState) {
					castMCC.ToggleEvent ();
				}
				break;
			case "ModuleLight":
			case "ModuleStockLightColoredLens":
			case "ModuleMultiPointSurfaceLight":
			case "ModuleColoredLensLight":
				ModuleLight castML = (ModuleLight)light;
				castML.LightsOn ();
				break;
			case "ModuleAnimateGeneric":
			case "ModuleAnimateGenericConsumer":
				ModuleAnimateGeneric castMAG = (ModuleAnimateGeneric)light;
				if (castMAG.animSwitch) {
					castMAG.Toggle ();
				}
				break;
			case "WBILight":
				light.GetType ().InvokeMember ("TurnOnLights", BindingFlags.InvokeMethod, null, light, null);
				break;
			case "ModuleNavLight":
				if (CLSettings.useAviationLightsEffect && CLSettings.inSunlight) {
					switch (light.part.name) {
					case "lightbeacon.amber":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.beaconAmber });
						break;
					case "lightbeacon.red":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.beaconRed });
						break;
					case "lightnav.blue":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.navBlue });
						break;
					case "lightnav.green":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.navGreen });
						break;
					case "lightnav.red":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.navRed });
						break;
					case "lightnav.white":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.navWhite });
						break;
					case "lightstrobe.white":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { CLSettings.strobeWhite });
						break;
					}
				} else {
					light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, new object[] {4});
				}
				break;
			case "ModuleKELight":
				light.GetType ().InvokeMember ("LightsOn", BindingFlags.InvokeMethod, null, light, null);
				break;
			}
		}

		public static void Off (PartModule light)
		{
			switch (light.moduleName) {
			case "ModuleColorChanger":
			case "ModuleColorChangerConsumer":
				ModuleColorChanger castMCC = (ModuleColorChanger)light;
				if (castMCC.animState) {
					castMCC.ToggleEvent ();
				}
				break;
			case "ModuleLight":
			case "ModuleStockLightColoredLens":
			case "ModuleMultiPointSurfaceLight":
			case "ModuleColoredLensLight":
				ModuleLight castML = (ModuleLight)light;
				castML.LightsOff ();
				break;
			case "ModuleAnimateGeneric":
			case "ModuleAnumateGenericConsumer":
				ModuleAnimateGeneric castMAG = (ModuleAnimateGeneric)light;
				castMAG.Toggle ();
				break;
			case "WBILight":
				light.GetType ().InvokeMember ("TurnOffLights", BindingFlags.InvokeMethod, null, light, null);
				break;
			case "ModuleNavLight":
				light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, new object[] {0});
				break;
			case "ModuleKELight":
				light.GetType ().InvokeMember ("LightsOff", BindingFlags.InvokeMethod, null, light, null);
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
			// Kerbal Electric Lights
			if (part.Modules.Contains ("ModuleKELight")) {
				foreach (PartModule module in part.Modules) {
					if (module.moduleName == "ModuleKELight") {
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


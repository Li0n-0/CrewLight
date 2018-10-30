using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using UnityEngine;
using KSP.Localization;

namespace CrewLight
{
	public/* static*/ class SwitchLight
	{
		private static CL_GeneralSettings generalSettings;
		private static CL_AviationLightsSettings aviationLSettings;

		private static bool settingsLoaded = false;

		private static void LoadSettings ()
		{
			generalSettings = HighLogic.CurrentGame.Parameters.CustomParams<CL_GeneralSettings> ();
			aviationLSettings = HighLogic.CurrentGame.Parameters.CustomParams<CL_AviationLightsSettings> ();
			settingsLoaded = true;
		}

		public static void On (PartModule light)
		{
			if (!settingsLoaded) {
				LoadSettings ();
			}

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
				if (generalSettings.useAviationLightsEffect && GameSettingsLive.inSunlight) {
					switch (light.part.name) {
					case "lightbeacon.amber":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.beaconAmber) });
						break;
					case "lightbeacon.red":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.beaconRed) });
						break;
					case "lightnav.blue":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.navBlue) });
						break;
					case "lightnav.green":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.navGreen) });
						break;
					case "lightnav.red":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.navRed) });
						break;
					case "lightnav.white":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.navWhite) });
						break;
					case "lightstrobe.white":
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { ParseNavLightStr (aviationLSettings.strobeWhite) });
						break;
					default:
						light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.SetField, null, light, 
							new object[] { 4 });
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

		public static List<PartModule> GetLightModule (Part part)
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
					if (Regex.IsMatch(module.actionGUIName, "light", RegexOptions.IgnoreCase) 
						|| Regex.IsMatch(module.startEventGUIName, "light", RegexOptions.IgnoreCase)) {
						lightList.Add (module);
					}
				}
			}
			// Wild Blue Industry
			if (part.Modules.Contains ("WBILight")) {
				foreach (PartModule module in part.Modules) {
					if (module.moduleName == "WBILight") {
						lightList.Add (module);
					}
				}
			}
			// AviationLights
			if (part.Modules.Contains ("ModuleNavLight")) {
				foreach (PartModule module in part.Modules) {
					if (module.moduleName == "ModuleNavLight") {
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

		public static bool IsOn (Part part)
		{
			return IsOn (GetLightModule (part));
		}

		public static bool IsOn (List<PartModule> modulesLight)
		{
			// not that usefull but needed for IsOn(Part)
			return IsOn (modulesLight[0]);
		}

		public static bool IsOn (PartModule light)
		{
			switch (light.moduleName) {
			case "ModuleColorChanger":
			case "ModuleColorChangerConsumer":
				ModuleColorChanger castMCC = (ModuleColorChanger)light;
				return castMCC.animState;

			case "ModuleLight":
			case "ModuleStockLightColoredLens":
			case "ModuleMultiPointSurfaceLight":
			case "ModuleColoredLensLight":
				ModuleLight castML = (ModuleLight)light;
				return castML.isOn;

			case "ModuleAnimateGeneric":
			case "ModuleAnumateGenericConsumer":
				ModuleAnimateGeneric castMAG = (ModuleAnimateGeneric)light;
				return !castMAG.animSwitch;

			case "WBILight":
				return (bool)light.GetType ().InvokeMember ("isDeployed", BindingFlags.GetField, null, light, null);

			case "ModuleNavLight":
				if ((int)light.GetType ().InvokeMember ("navLightSwitch", BindingFlags.GetField, null, light, null) != 0) {
					return true;
				} else { return false; }
					
			case "ModuleKELight":
				return (bool)light.GetType ().InvokeMember ("isOn", BindingFlags.GetField, null, light, null);
			
			default:
				return false;
			}
		}

		private static int ParseNavLightStr (string navLightStr)
		{
			int navLightInt;

			switch (navLightStr)
			{
			case "#autoLOC_CL_0063"://off
				navLightInt = 0;
				break;
			case "#autoLOC_CL_0064"://flash
				navLightInt = 1;
				break;
			case "#autoLOC_CL_0065"://double-flash
				navLightInt = 2;
				break;
			case "#autoLOC_CL_0066"://interval
				navLightInt = 3;
				break;
			case "#autoLOC_CL_0067"://on
				navLightInt = 4;
				break;
			default:
				navLightInt = 4;
				break;
			}

			return navLightInt;
		}

		private static void D (String str)
		{
			Debug.Log ("[Crew Light - SwitchLight] : " + str);
		}
	}
}


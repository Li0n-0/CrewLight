using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrewLight
{
	public static class SwitchLight
	{
		public static void On (Part part)
		{
			/* Send the event that turn on the light, different flavor for different PartModule */
			if (part.Modules.Contains<ModuleColorChanger>()) {
				foreach (ModuleColorChanger anim in part.Modules.GetModules<ModuleColorChanger>()) {
					if (Regex.IsMatch(anim.toggleName, "light", RegexOptions.IgnoreCase) && anim.animState == false) {
						anim.ToggleEvent ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleColorChanger");
//						return;
					}
				}
			}
			if (part.Modules.Contains<ModuleLight>()) { // For the Karibou rover, and maybe others...
				foreach (ModuleLight anim in part.Modules.GetModules<ModuleLight>()) {
					if (anim.isOn == false) {
						anim.LightsOn ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleLight");
//						return;
					}
				}
			}
			if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
				foreach (ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
					if (Regex.IsMatch(anim.actionGUIName, "light", RegexOptions.IgnoreCase) && anim.animSwitch == true){// anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") && anim.animSwitch == true) {
						anim.Toggle ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleAnimateGeneric");
//						return;
					}
				}
			}
			if (part.Modules.Contains("WBILight")) {
				foreach (PartModule partM in part.Modules) {
					if (partM.ClassName == "WBILight") {
						partM.SendMessage ("TurnOnLights");

					}
				}
			}
		}

		public static void Off (Part part) {
			if (part.protoModuleCrew.Count == 0) {
				if (part.Modules.Contains<ModuleColorChanger>()) {
					foreach (ModuleColorChanger anim in part.Modules.GetModules<ModuleColorChanger>()) {
						if (Regex.IsMatch(anim.toggleName, "light", RegexOptions.IgnoreCase) && anim.animState == true) {
							anim.ToggleEvent ();
//							return;
						}
					}
				}
				if (part.Modules.Contains<ModuleLight>()) {
					foreach (ModuleLight anim in part.Modules.GetModules<ModuleLight>()) {
						if (anim.isOn == true) {
							anim.LightsOff ();
//							return;
						}
					}
				}
				if (part.Modules.Contains<ModuleAnimateGeneric>()) {
					foreach (ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
						if (Regex.IsMatch(anim.actionGUIName, "light", RegexOptions.IgnoreCase) && anim.animSwitch == false){//(anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") && anim.animSwitch == false) {
							anim.Toggle ();
//							return;
						}
					}
				}
				if (part.Modules.Contains("WBILight")) {
					foreach (PartModule partM in part.Modules) {
						if (partM.ClassName == "WBILight") {
							partM.SendMessage ("TurnOffLights");
						}
					}
				}
			}
		}

		public static void AllLightsOn (List<PartModule> modulesLight)
		{
			foreach (PartModule partM in modulesLight) {
				switch (partM.ClassName) {
				case "ModuleColorChanger":
					if (!partM.GetComponent<ModuleColorChanger> ().animState) {
						partM.SendMessage ("ToggleEvent");
					}
					break;
				case "ModuleLight":
				case "ModuleStockLightColoredLens":
				case "ModuleMultiPointSurfaceLight":
				case "ModuleColoredLensLight":
					partM.SendMessage ("LightsOn");
					break;
				case "ModuleAnimateGeneric":
					if (partM.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
						partM.SendMessage ("Toggle");
					}
					break;
				case "WBILight":
					partM.SendMessage ("TurnOnLights");
					break;
				}
			}
		}

		public static void AllLightsOff (List<PartModule> modulesLight)
		{
			foreach (PartModule partM in modulesLight) {
				switch (partM.ClassName) {
				case "ModuleColorChanger":
					if (partM.GetComponent<ModuleColorChanger> ().animState) {
						partM.SendMessage ("ToggleEvent");
					}
					break;
				case "ModuleLight":
				case "ModuleStockLightColoredLens":
				case "ModuleMultiPointSurfaceLight":
				case "ModuleColoredLensLight":
					partM.SendMessage ("LightsOff");
					break;
				case "ModuleAnimateGeneric":
					if (partM.GetComponent<ModuleAnimateGeneric> ().animSwitch == false) {
						partM.SendMessage ("Toggle");
					}
					break;
				case "WBILight":
					partM.SendMessage ("TurnOffLights");
					break;
				}
			}
		}

		private static void D (String str)
		{
			Debug.Log ("[Crew Light - SwitchLight] : " + str);
		}
	}
}


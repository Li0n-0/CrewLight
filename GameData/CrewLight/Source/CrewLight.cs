using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrewLight
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class CrewLight : MonoBehaviour
	{
		/*
		 *
		 *	This little plugin does two things : when a vessel is selected, all the part containing a Kerbal
		 *	are lightnend.
		 *	Second : when a Kerbal change seat, either by a transfer or boarding the corresponding part is 
		 *	lightned.
		 * 
		 */

		private Settings settings;
		private bool morseCodeIsEnabled;
//		private bool isStart;

		private List<PartModule> lightModules;
		private List<bool?> lightIsOn;
		private float timeFromVesselLoad;

		public void Start () 
		{
			settings = new Settings ();
			settings.Load ();
			morseCodeIsEnabled = settings.useMorseCode;
//			isStart = true;

			GameEvents.onCrewTransferred.Add (UpdateLight);
			GameEvents.onVesselChange.Add (StartLight);
			if (morseCodeIsEnabled) {
				GameEvents.onVesselChange.Add (StopLightCoroutine);
				GameEvents.onGameSceneLoadRequested.Add (OnGameSceneChange);
				GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
			}
			StartLight (FlightGlobals.ActiveVessel);
//			StartCoroutine (RoutineLight());
		}

		public void OnDestroy () 
		{
			if (morseCodeIsEnabled) {
				StopLightCoroutine ();
				GameEvents.onVesselChange.Remove (StopLightCoroutine);
				GameEvents.onGameSceneLoadRequested.Remove (OnGameSceneChange);
				GameEvents.onVesselGoOffRails.Remove (OnVesselGoOffRails);
			}
			GameEvents.onCrewTransferred.Remove (UpdateLight);
			GameEvents.onVesselChange.Remove (StartLight);
		}

		private void StartLight (Vessel vessel) {
			/* Set the lights for a whole vessel */
			StartCoroutine("LightCrewCab", vessel);
		}

		private IEnumerator LightCrewCab (Vessel vessel)
		{
			if (vessel.crewedParts != 0 && vessel.isEVA == false) {
				yield return new WaitForSeconds (.1f);
				foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()){
					if (crewMember.KerbalRef != null) {// If this is false it should means the Kerbal is in a Command Seat
						Light (crewMember.KerbalRef.InPart);
					}
				}
			}
			Debug.Log ("[Crew Light] Start : Assigning timeFromVesselLoad, Time.time = " + Time.time);
			timeFromVesselLoad = Time.time;
		}

		private void UpdateLight (GameEvents.HostedFromToAction<ProtoCrewMember, Part> eData) 
		{
			/* Update the status of the lights when a Kerbal moves */
			Light (eData.to);
			LightOff (eData.from);
		}

		private void Light (Part part)
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

		private void LightOff (Part part) {
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

//		private void LightToggle (Part part) {
//			if (part.protoModuleCrew.Count == 0) {
//				if (part.Modules.Contains<ModuleColorChanger>()) {
//					foreach (ModuleColorChanger anim in part.Modules.GetModules<ModuleColorChanger>()) {
//						if (anim.toggleName == "Toggle Lights") {
//							anim.ToggleEvent ();
//							return;
//						}
//					}
//				}
//				if (part.Modules.Contains<ModuleLight>()) {
//					foreach (ModuleLight anim in part.Modules.GetModules<ModuleLight>()) {
//						if (anim.isOn == true) {
//							anim.LightsOff ();
//							return;
//						} else {
//							anim.LightsOn ();
//							return;
//						}
//					}
//				}
//				if (part.Modules.Contains<ModuleAnimateGeneric>()) {
//					foreach (ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
//						if (anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") {
//							anim.Toggle ();
//							return;
//						}
//					}
//				}
//				if (part.Modules.Contains("WBILight")) {
//					foreach (PartModule partM in part.Modules) {
//						if (partM.ClassName == "WBILight") {
//							partM.SendMessage ("ToggleAnimation");
//						}
//					}
//				}
//			}
//		}

		private void OnGameSceneChange (GameScenes gameScene)
		{
			StopLightCoroutine ();
		}

		private void OnVesselGoOffRails (Vessel vessel)
		{
//			Debug.Log ("[Crew Light] OnVesselGoOffRails : Vessel spotted : " + vessel.vesselName);
//
//			Debug.Log ("[Crew Light] OnVesselGoOffRails : timeFromVesselLoad = " + timeFromVesselLoad);
//			Debug.Log ("[Crew Light] OnVesselGoOffRails : Time.time = " + Time.time);
//			Debug.Log ("[Crew Light] OnVesselGoOffRails : Evaluate : timeFromVesselLoad + 2.5 <= Time.time");
			if (timeFromVesselLoad + 2.5f <= Time.time && vessel != FlightGlobals.ActiveVessel) {
				if (settings.onlyForControllable) {
					if (vessel.IsControllable) {
						StartCoroutine ("DistantVesselLight", vessel);
//						Debug.Log ("[Crew Light] Coroutine : Start DistantVesselLight");
					}
				} else {
					StartCoroutine ("DistantVesselLight", vessel);
//					Debug.Log ("[Crew Light] Coroutine : Start DistantVesselLight");
				}
			}
		}

		void StopLightCoroutine (Vessel v = null)
		{
			StopCoroutine("DistantVesselLight");
			StopCoroutine ("BlinkLights");

			if (lightIsOn != null && lightModules != null) {
				LightPreviousState ();
			}
		}
			
		IEnumerator DistantVesselLight (Vessel vessel)
		{
			/*
			 * Populate two lists : one for all the lightable part
			 * the second for their state
			 * 
			 * Blink the lights according to the morse message define in the setting
			 * 
			 * Restore the lights to their previous state
			 */

			yield return new WaitForSeconds (.1f);// I had one crash once when searching for parts during the physic loading

			lightModules = new List<PartModule> ();
			lightIsOn = new List<bool?> ();

			int iSearch = 0;// Max parts being search per tick
//			Debug.Log ("[Crew Light] : Starting populate list of part module for the distant vessel");
			foreach (Part part in vessel.Parts) {
				
				if (iSearch == 200) {
					iSearch = 0;
					yield return new WaitForSeconds (.1f);
				}

				if (part.Modules.Contains<ModuleColorChanger> ()) {
					ModuleColorChanger partM = part.Modules.GetModule<ModuleColorChanger> ();
					if (partM.toggleName == "Toggle Lights") {
						lightModules.Add (partM);
						if (partM.animState) {
							lightIsOn.Add (true);
						} else {
							lightIsOn.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleLight> ()) {
					foreach (ModuleLight partM in part.Modules.GetModules<ModuleLight>()) {
						lightModules.Add (partM);
						if (partM.isOn) {
							lightIsOn.Add (true);
						} else {
							lightIsOn.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
					foreach (ModuleAnimateGeneric partM in part.Modules.GetModules<ModuleAnimateGeneric>()) {
						if (partM.actionGUIName == "Toggle Lights" || partM.startEventGUIName == "Lights On") {
							lightModules.Add (partM);
							if (partM.animSwitch == false) {
								lightIsOn.Add (true);
							} else {
								lightIsOn.Add (false);
							}
							break;
						}
					}
				}
				if (part.Modules.Contains ("WBILight")) {
					foreach (PartModule partM in part.Modules) {
						if (partM.ClassName == "WBILight") {
							lightModules.Add (partM);
							lightIsOn.Add (null);
							break;
						}
					}
				}

				iSearch++;
			}
//			Debug.Log ("[Crew Light] : List is populated");
//
//			Debug.Log ("[Crew Light] : distance = " + settings.distance + " and its type is : " + settings.distance.GetType ().ToString ());
//			Debug.Log ("[Crew Light] : ti = " + settings.tiDuration + " and its type is : " + settings.tiDuration.GetType ().ToString ());
//			Debug.Log ("[Crew Light] : taah = " + settings.taahDuration + " and its type is : " + settings.taahDuration.GetType ().ToString ());
//
//			Debug.Log ("[Crew Light] : Checking the distant between the two vessel");
			// Checking the distance between the active and the encountered ship
			if (settings.distance < 200d) {
//				Debug.Log ("[Crew Light] : Distance in the settings is less than 200m");
//				Debug.Log ("[Crew Light] : Distance at this point is : " + Vector3d.Distance (FlightGlobals.ship_orbit.pos, vessel.orbit.pos));
				double vesselDistance = 1000d;
				while (vesselDistance > settings.distance) {
					yield return new WaitForSeconds (.5f);
					vesselDistance = Vector3d.Distance (FlightGlobals.ship_orbit.pos, vessel.orbit.pos);
				}
			} else {
//				Debug.Log ("[Crew Light] : Distance in the settings is less than 200m");
			}
//			Debug.Log ("[Crew Light] Coroutine : Start BlinkLight");
			StartCoroutine("BlinkLights");
		}

		IEnumerator BlinkLights ()
		{
			// Turning all the lights off before Morse blinking
			AllLightsOff (lightModules);
			yield return new WaitForSeconds (settings.ditDuration);

			// Morse message
			foreach (int c in settings.morseCode) {
				switch (c) {
				case 0:
					AllLightsOn (lightModules);
					yield return new WaitForSeconds (settings.ditDuration);
					break;
				case 1:
					AllLightsOn (lightModules);
					yield return new WaitForSeconds (settings.dahDuration);
					break;
				case 2:
					AllLightsOff (lightModules);
					yield return new WaitForSeconds (settings.letterSpaceDuration);
					break;
				case 3:
					AllLightsOff (lightModules);
					yield return new WaitForSeconds (settings.wordSpaceDuration);
					break;
				case 4:
					AllLightsOff (lightModules);
					yield return new WaitForSeconds (settings.symbolSpaceDuration);
					break;
				}
			}
			LightPreviousState ();
		}

		public void LightPreviousState ()
		{
			// Settings lights to theirs previous state
			int i = 0;
			foreach (bool? isOn in lightIsOn) {
				if (isOn == null) {
					if (lightModules[i].part.CrewCapacity > 0) {
						if (lightModules[i].part.protoModuleCrew.Count > 0) {
							Light (lightModules[i].part);
						} else {
							LightOff (lightModules [i].part);
						}
					}
				} else if (isOn == true) {
					Light (lightModules [i].part);
				} else {
					LightOff (lightModules [i].part);
				}
				i++;
			}
			lightIsOn = null;
			lightModules = null;
		}

		private void AllLightsOff (List<PartModule> moduleLight)
		{
			foreach (PartModule partM in moduleLight) {
				switch (partM.ClassName) {
				case "ModuleColorChanger":
					if (partM.GetComponent<ModuleColorChanger> ().animState) {
						partM.GetComponent<ModuleColorChanger> ().ToggleEvent ();
					}
					break;
				case "ModuleLight":
					partM.GetComponent<ModuleLight> ().LightsOff ();
					break;
				case "ModuleAnimateGeneric":
					if (partM.GetComponent<ModuleAnimateGeneric> ().animSwitch == false) {
						partM.GetComponent<ModuleAnimateGeneric> ().Toggle ();
					}
					break;
				case "WBILight":
					partM.SendMessage ("TurnOffLights");
					break;
				}
			}
		}

		private void AllLightsOn (List<PartModule> moduleLight)
		{
			foreach (PartModule partM in moduleLight) {
				switch (partM.ClassName) {
				case "ModuleColorChanger":
					if (partM.GetComponent<ModuleColorChanger> ().animState == false) {
						partM.GetComponent<ModuleColorChanger> ().ToggleEvent ();
					}
					break;
				case "ModuleLight":
					partM.GetComponent<ModuleLight> ().LightsOn ();
					break;
				case "ModuleAnimateGeneric":
					if (partM.GetComponent<ModuleAnimateGeneric> ().animSwitch) {
						partM.GetComponent<ModuleAnimateGeneric> ().Toggle ();
					}
					break;
				case "WBILight":
					partM.SendMessage ("TurnOnLights");
					break;
				}
			}
		}
	}
}


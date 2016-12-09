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
		private Settings settings;

		private int maxSearch = 200;

		public void Start () 
		{
			settings = new Settings ();
			settings.Load ();

			// Crew Light function :
			GameEvents.onCrewTransferred.Add (UpdateLight);
			GameEvents.onVesselChange.Add (StartLight);
			StartLight (FlightGlobals.ActiveVessel);

			// Distant Light function :
			if (settings.useMorseCode) {
				GameEvents.onTimeWarpRateChanged.Add (OnTimeWarpChanged);
				GameEvents.onVesselChange.Add (StopDistantLightCoroutine);
				GameEvents.onGameSceneLoadRequested.Add (OnGameSceneChange);
				GameEvents.onVesselGoOffRails.Add(OnVesselGoOffRails);
			}

			// Sun Light function :
			if (settings.useSunLight) {
				GameEvents.onVesselChange.Add (OnVesselChange);
				GameEvents.onVesselPartCountChanged.Add (OnVesselChange);
				StartCoroutine ("TrackSun", FlightGlobals.ActiveVessel);
			}
		}

		public void OnDestroy () 
		{
			// Crew Light function :
			GameEvents.onCrewTransferred.Remove (UpdateLight);
			GameEvents.onVesselChange.Remove (StartLight);

			// Distant Light function :
			if (settings.useMorseCode) {
				StopDistantLightCoroutine ();
				GameEvents.onTimeWarpRateChanged.Remove (OnTimeWarpChanged);
				GameEvents.onVesselChange.Remove (StopDistantLightCoroutine);
				GameEvents.onGameSceneLoadRequested.Remove (OnGameSceneChange);
				GameEvents.onVesselGoOffRails.Remove (OnVesselGoOffRails);
			}

			// Sun Light function :
			if (settings.useSunLight) {
				GameEvents.onVesselChange.Remove (OnVesselChange);
				GameEvents.onVesselPartCountChanged.Remove (OnVesselChange);
				StopCoroutine ("TrackSun");
			}
		}

		#region CrewLight

		private float timeFromVesselLoad;

		private void StartLight (Vessel vessel) {
			/* Set the lights in crewable parts regarding to theirs occupation */
			StartCoroutine("LightCrewCab", vessel);
			timeFromVesselLoad = Time.time;
		}

		private IEnumerator LightCrewCab (Vessel vessel)
		{
			yield return new WaitForSeconds (.1f);
			if (vessel.crewedParts != 0 && vessel.isEVA == false) {
				foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()){
					if (crewMember.KerbalRef != null) {// If this is false it should means the Kerbal is in a Command Seat
						SwitchLight.On (crewMember.KerbalRef.InPart);
					}
				}
			}
		}

		private void UpdateLight (GameEvents.HostedFromToAction<ProtoCrewMember, Part> eData) 
		{
			/* Update the status of the lights when a Kerbal moves */
			SwitchLight.On (eData.to);
			SwitchLight.Off (eData.from);
		}

		#endregion

		#region DistantLight

		private List<PartModule> distantVesselLightModule;
		private List<bool?> distantVesselLightState;

		private IEnumerator FindLightOnDistantVessel (Vessel vessel)
		{
			distantVesselLightModule = new List<PartModule>();
			distantVesselLightState = new List<bool?>();

			int iSearch = 0;

			yield return new WaitForSeconds (.1f);

			foreach (Part part in vessel.Parts) {
				if (iSearch >= maxSearch) {
					yield return new WaitForSeconds (.1f);
					iSearch = 0;
				}

				// Check for lightable modules
				if (part.Modules.Contains<ModuleColorChanger> ()) {
					ModuleColorChanger partM = part.Modules.GetModule<ModuleColorChanger> ();
					if (Regex.IsMatch(partM.toggleName, "light", RegexOptions.IgnoreCase)) {
						distantVesselLightModule.Add (partM);
						if (partM.animState) {
							distantVesselLightState.Add (true);
						} else {
							distantVesselLightState.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleLight> ()) {
					foreach (ModuleLight partM in part.Modules.GetModules<ModuleLight>()) {
						distantVesselLightModule.Add (partM);
						if (partM.isOn) {
							distantVesselLightState.Add (true);
						} else {
							distantVesselLightState.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
					foreach (ModuleAnimateGeneric partM in part.Modules.GetModules<ModuleAnimateGeneric>()) {
						if (Regex.IsMatch(partM.actionGUIName, "light", RegexOptions.IgnoreCase)) {
							distantVesselLightModule.Add (partM);
							if (partM.animSwitch == false) {
								distantVesselLightState.Add (true);
							} else {
								distantVesselLightState.Add (false);
							}
						}
					}
				}
				if (part.Modules.Contains ("WBILight")) {
					foreach (PartModule partM in part.Modules) {
						if (partM.ClassName == "WBILight") {
							distantVesselLightModule.Add (partM);
							distantVesselLightState.Add (null);
						}
					}
				}
				iSearch++;
			}
		}
			
		private void OnTimeWarpChanged ()
		{
			timeFromVesselLoad = Time.time;
		}

		private void OnGameSceneChange (GameScenes gameScene)
		{
			// Dummy method because both events OnGameSceneChangeRequested and OnVesselChange return different
			// type. A better workaroud would be to write a custom events that trigger when one of the previous 
			// event do.
			StopDistantLightCoroutine ();
		}

		private void OnVesselGoOffRails (Vessel vessel)
		{
			// Check time elapsed since active vessel has loaded so it don't light already nearby vessel
			if (timeFromVesselLoad + 2.5f <= Time.time && vessel != FlightGlobals.ActiveVessel) {
				if (settings.onlyForControllable) {
					if (vessel.IsControllable) {
						StartCoroutine ("DistantVesselLight", vessel);
					}
				} else {
					StartCoroutine ("DistantVesselLight", vessel);
				}
			}
		}

		private void StopDistantLightCoroutine (Vessel vessel = null)
		{
			StopCoroutine("DistantVesselLight");

			if (distantVesselLightState != null && distantVesselLightModule != null) {
				LightPreviousState ();
			}
		}
			
		IEnumerator DistantVesselLight (Vessel vessel)
		{
			/*
			 * Create two lists : one for all the lightable part
			 * the second for their state
			 * 
			 * Blink the lights according to the morse message define in the setting
			 * 
			 * Restore the lights to their previous state
			 */

			// Create list
			yield return StartCoroutine ("FindLightOnDistantVessel", vessel);

			// Checking the distance between the active and the encountered ship
			if (settings.distance < 200d) {
				double vesselDistance = 1000d;
				while (vesselDistance > settings.distance) {
					yield return new WaitForSeconds (.5f);
					vesselDistance = Vector3d.Distance (FlightGlobals.ship_orbit.pos, vessel.orbit.pos);
				}
			}

			SwitchLight.AllLightsOff (distantVesselLightModule);
			yield return new WaitForSeconds (settings.ditDuration);

			// Morse message
			foreach (int c in settings.morseCode) {
				switch (c) {
				case 0:
					SwitchLight.AllLightsOn (distantVesselLightModule);
					yield return new WaitForSeconds (settings.ditDuration);
					break;
				case 1:
					SwitchLight.AllLightsOn (distantVesselLightModule);
					yield return new WaitForSeconds (settings.dahDuration);
					break;
				case 2:
					SwitchLight.AllLightsOff (distantVesselLightModule);
					yield return new WaitForSeconds (settings.letterSpaceDuration);
					break;
				case 3:
					SwitchLight.AllLightsOff (distantVesselLightModule);
					yield return new WaitForSeconds (settings.wordSpaceDuration);
					break;
				case 4:
					SwitchLight.AllLightsOff (distantVesselLightModule);
					yield return new WaitForSeconds (settings.symbolSpaceDuration);
					break;
				}
			}
			LightPreviousState ();
		}

		private void LightPreviousState ()
		{
			// Settings lights to theirs previous state
			int i = 0;
			foreach (bool? isOn in distantVesselLightState) {
				if (isOn == null) {
					if (distantVesselLightModule[i].part.CrewCapacity > 0) {
						if (distantVesselLightModule[i].part.protoModuleCrew.Count > 0) {
							SwitchLight.On (distantVesselLightModule[i].part);
						} else {
							SwitchLight.Off (distantVesselLightModule [i].part);
						}
					}
				} else if (isOn == true) {
					SwitchLight.On (distantVesselLightModule [i].part);
				} else {
					SwitchLight.Off (distantVesselLightModule [i].part);
				}
				i++;
			}
			distantVesselLightState = null;
			distantVesselLightModule = null;
		}

		#endregion

		#region SunLight

		private List<PartModule> activeVesselLightModule;

		private Vector3d vesselPos, sunPos;
		private RaycastHit hit;

		private int layerMask = (1 << 10); // Scaled Scenery layer
		private bool inDark = false;
		private float waitBetweenRay = 1.5f;

		private IEnumerator FindLightOnActiveVessel (Vessel vessel)
		{
			activeVesselLightModule = new List<PartModule>();

			int iSearch = 0;

			yield return new WaitForSeconds (.1f);

			foreach (Part part in vessel.Parts) {
				if (iSearch >= maxSearch) {
					yield return new WaitForSeconds (.1f);
					iSearch = 0;
				}

				// Check if part is uncrewed
				if (part.CrewCapacity == 0) {

					if (part.Modules.Contains<ModuleColorChanger> ()) {
						ModuleColorChanger partM = part.Modules.GetModule<ModuleColorChanger> ();
						if (Regex.IsMatch(partM.toggleName, "light", RegexOptions.IgnoreCase)) {
							if (settings.onlyNoAGpart) {
								if (!partM.Actions.Contains(KSPActionGroup.Light)) {
									activeVesselLightModule.Add (partM);
								}
							} else {
								activeVesselLightModule.Add (partM);
							}
						}
					}
					if (part.Modules.Contains<ModuleLight> ()) {
						foreach (ModuleLight partM in part.Modules.GetModules<ModuleLight>()) {
							if (settings.onlyNoAGpart) {
								if (!partM.Actions.Contains(KSPActionGroup.Light)) {
									activeVesselLightModule.Add (partM);
								}
							} else {
								activeVesselLightModule.Add (partM);
							}
						}
					}
					if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
						foreach (ModuleAnimateGeneric partM in part.Modules.GetModules<ModuleAnimateGeneric>()) {
							if (Regex.IsMatch(partM.actionGUIName, "light", RegexOptions.IgnoreCase)) {
								if (settings.onlyNoAGpart) {
									if (!partM.Actions.Contains(KSPActionGroup.Light)) {
										activeVesselLightModule.Add (partM);
									}
								} else {
									activeVesselLightModule.Add (partM);
								}
							}
						}
					}
					if (part.Modules.Contains ("WBILight")) {
						foreach (PartModule partM in part.Modules) {
							if (partM.ClassName == "WBILight") {
								if (settings.onlyNoAGpart) {
									if (!partM.Actions.Contains(KSPActionGroup.Light)) {
										activeVesselLightModule.Add (partM);
									}
								} else {
									activeVesselLightModule.Add (partM);
								}
							}
						}
					}
				}
				iSearch++;
			}
		}

		private void OnVesselChange (Vessel vessel) {
			StopCoroutine ("TrackSun");
			StartCoroutine ("TrackSun", vessel);
		}

		private IEnumerator TrackSun (Vessel vessel)
		{
			yield return StartCoroutine ("FindLightOnActiveVessel", vessel);

			while (true) {
				// Get position of the sun and the vessel
				vesselPos = FlightGlobals.ActiveVessel.transform.position;
				sunPos = FlightGlobals.GetBodyByName ("Sun").position;

				if (Physics.Raycast (vesselPos, sunPos, out hit, Mathf.Infinity, layerMask)) {
					if (hit.transform != null) {
//						Debug.Log ("[Crew Light] SunLight : hit is " + hit.transform.name);
						if (hit.transform.name == "Sun") {
							if (inDark) {
								SwitchLight.AllLightsOff (activeVesselLightModule);
								inDark = false;
							}
						} else {
							if (inDark == false) {
								SwitchLight.AllLightsOn (activeVesselLightModule);
								inDark = true;
							}
						}
					}
				}
				yield return new WaitForSeconds (waitBetweenRay);
			}
		}

		#endregion
	}
}
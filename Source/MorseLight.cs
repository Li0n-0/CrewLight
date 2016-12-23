using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrewLight
{
	public class MorseLight : MonoBehaviour
	{
		private Vessel vessel;
		private List<PartModule> moduleLight;
		private List<bool?> stateLight;
		private double offLimit = 2600d;

		public void Start ()
		{
			vessel = this.GetComponent<Vessel> ();

			// Check for the right type
			if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.EVA 
				|| vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject) {

				Destroy (this);
			}

			// Check for controllable vessel
			if (CLSettings.onlyForControllable) {
				if (!vessel.IsControllable) {
					Destroy (this);
				}
			}

			// Destroy if vessel are too close
			if (GetDistance() < CLSettings.distance) {
				Destroy (this);
			}

			StartCoroutine ("StartMorseLight");
		}

		public void OnDestroy ()
		{
			StopAllCoroutines ();
			LightPreviousState ();
		}

		private double GetDistance ()
		{
			return Vector3d.Distance (FlightGlobals.ActiveVessel.orbit.pos, vessel.orbit.pos);
		}

		private IEnumerator StartMorseLight ()
		{
			yield return StartCoroutine ("FindLightPart");

			double vesselDistance = GetDistance();
			while (vesselDistance > CLSettings.distance) {
				if (vesselDistance > offLimit) {
					Destroy (this);
				}
				yield return new WaitForSeconds (.5f);
				vesselDistance = GetDistance();
			}

			SwitchLight.AllLightsOff (moduleLight);
			yield return new WaitForSeconds (CLSettings.ditDuration);

			// Morse message
			foreach (int c in CLSettings.morseCode) {
				switch (c) {
				case 0:
					SwitchLight.AllLightsOn (moduleLight);
					yield return new WaitForSeconds (CLSettings.ditDuration);
					break;
				case 1:
					SwitchLight.AllLightsOn (moduleLight);
					yield return new WaitForSeconds (CLSettings.dahDuration);
					break;
				case 2:
					SwitchLight.AllLightsOff (moduleLight);
					yield return new WaitForSeconds (CLSettings.letterSpaceDuration);
					break;
				case 3:
					SwitchLight.AllLightsOff (moduleLight);
					yield return new WaitForSeconds (CLSettings.wordSpaceDuration);
					break;
				case 4:
					SwitchLight.AllLightsOff (moduleLight);
					yield return new WaitForSeconds (CLSettings.symbolSpaceDuration);
					break;
				}
			}

			LightPreviousState ();
			Destroy (this);
		}

		private void LightPreviousState ()
		{
			// Settings lights to theirs previous state
			if (stateLight != null && moduleLight != null) {
				int i = 0;
				foreach (bool? isOn in stateLight) {
					if (isOn == null) {
						if (moduleLight[i].part.CrewCapacity > 0) {
							if (moduleLight[i].part.protoModuleCrew.Count > 0) {
								SwitchLight.On (moduleLight[i].part);
							} else {
								SwitchLight.Off (moduleLight [i].part);
							}
						}
					} else if (isOn == true) {
						SwitchLight.On (moduleLight [i].part);
					} else {
						SwitchLight.Off (moduleLight [i].part);
					}
					i++;
				}
			}
		}

		private IEnumerator FindLightPart ()
		{
			moduleLight = new List<PartModule>();
			stateLight = new List<bool?>();

			int iSearch = 0;

			yield return new WaitForSeconds (.1f);

			foreach (Part part in vessel.Parts) {
				if (iSearch >= CLSettings.maxSearch) {
					yield return new WaitForSeconds (.1f);
					iSearch = 0;
				}

				// Check for lightable modules
				if (part.Modules.Contains<ModuleColorChanger> ()) {
					ModuleColorChanger partM = part.Modules.GetModule<ModuleColorChanger> ();
					if (Regex.IsMatch(partM.toggleName, "light", RegexOptions.IgnoreCase)) {
						moduleLight.Add (partM);
						if (partM.animState) {
							stateLight.Add (true);
						} else {
							stateLight.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleLight> ()) {
					foreach (ModuleLight partM in part.Modules.GetModules<ModuleLight>()) {
						moduleLight.Add (partM);
						if (partM.isOn) {
							stateLight.Add (true);
						} else {
							stateLight.Add (false);
						}
					}
				}
				if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
					foreach (ModuleAnimateGeneric partM in part.Modules.GetModules<ModuleAnimateGeneric>()) {
						if (Regex.IsMatch(partM.actionGUIName, "light", RegexOptions.IgnoreCase)) {
							moduleLight.Add (partM);
							if (partM.animSwitch == false) {
								stateLight.Add (true);
							} else {
								stateLight.Add (false);
							}
						}
					}
				}
				if (part.Modules.Contains ("WBILight")) {
					foreach (PartModule partM in part.Modules) {
						if (partM.ClassName == "WBILight") {
							moduleLight.Add (partM);
							stateLight.Add (null);
						}
					}
				}
				iSearch++;
			}
		}

		private void D (string str)
		{
			Debug.Log ("[Crew Light - MorseLight] : " + str);
		}
	}
}


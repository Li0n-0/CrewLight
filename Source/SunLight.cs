using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace CrewLight
{
	public class SunLight : MonoBehaviour
	{

		private Vessel vessel;
		private List<PartModule> modulesLight;
		private bool inDark;

		public void Start ()
		{
			vessel = this.GetComponent<Vessel> ();

			// Checking for the type cannot be done earlier unfortunalely, it won't be correctly assigned
			if (vessel.vesselType == VesselType.Debris || vessel.vesselType == VesselType.EVA 
				|| vessel.vesselType == VesselType.Flag || vessel.vesselType == VesselType.SpaceObject) {

				Destroy (this);
			}

			StartCoroutine ("StartSunLight");
		}

		public void OnDestroy ()
		{
			StopAllCoroutines ();
		}

		private bool IsSunShine ()
		{
			Vector3d vesselPos = vessel.GetWorldPos3D ();
			Vector3d sunPos = FlightGlobals.GetBodyByName ("Sun").position;
			RaycastHit hit;

			if (Physics.Raycast(vesselPos, sunPos, out hit, Mathf.Infinity, CLSettings.layerMask)) {
				if (hit.transform.name == "Sun") {
					return true;
				}
			}
			return false;
		}

		private bool IsInDepth ()
		{
			if (vessel.LandedOrSplashed && FlightGlobals.currentMainBody.ocean) {
				if (vessel.altitude < -CLSettings.depthThreshold) {
					return true;
				}
			}
			return false;
		}
			
		private void SetLights ()
		{
			// Depth Lights :
			if (CLSettings.useDepthLight) {
				if (IsInDepth ()) {
					if (!inDark) {
						if (CLSettings.useSunLight) {
							StartCoroutine ("StageLight");
						} else {
							SwitchLight.On (modulesLight);
						}
						inDark = true;
					}
					return;
				}
			}

			// Sun Lights :
			if (IsSunShine ()) {
				if (inDark) {
					StopCoroutine ("StageLight");
					SwitchLight.Off (modulesLight);
					inDark = false;
				}
			} else {
				if (!inDark) {
					if (CLSettings.useStaggerdLight) {
						StartCoroutine ("StageLight");
					} else {
						SwitchLight.On (modulesLight);
					}
					inDark = true;
				}
			}
		}

		private IEnumerator StartSunLight ()
		{
			yield return StartCoroutine ("FindLightPart");

			inDark = IsSunShine (); 

			while (true) {
				SetLights ();
				if (TimeWarp.CurrentRate < 5f) {
					yield return new WaitForSeconds (CLSettings.delayLowTimeWarp / TimeWarp.CurrentRate);
				} else {
					yield return new WaitForSeconds (CLSettings.delayHighTimeWarp);
				}
			}
		}

		private IEnumerator StageLight ()
		{
			foreach (List<PartModule> stageList in SliceLightList ()) {
				SwitchLight.On (stageList);
				if (CLSettings.useRandomDelay) {
					yield return new WaitForSeconds (UnityEngine.Random.Range (.4f, 2f));
				} else {
					yield return new WaitForSeconds (CLSettings.delayStage);
				}
			}
		}

		private List<List<PartModule>> SliceLightList ()
		{
			List<List<PartModule>> slicedList = new List<List<PartModule>> ();
			List<PartModule> workingList = new List<PartModule> (modulesLight);

			while (workingList.Count != 0) {
				List<PartModule> stageList = new List<PartModule> ();
				int rndLightInStage = UnityEngine.Random.Range (CLSettings.minLightPerStage, CLSettings.maxLightPerStage);
				if (rndLightInStage > workingList.Count) {
					rndLightInStage = workingList.Count;
				}
				for (int i = 0 ; i < rndLightInStage ; i++) {
					int randIndex = UnityEngine.Random.Range (0, workingList.Count);
					stageList.Add (workingList [randIndex]);
					workingList.RemoveAt (randIndex);
				}
				slicedList.Add (stageList);
			}
			return slicedList;
		}

		private IEnumerator FindLightPart ()
		{
			modulesLight = new List<PartModule> ();

			int iSearch = -1;

			yield return new WaitForSeconds (.1f);

			foreach (Part part in vessel.Parts) {
				iSearch++;
				if (iSearch >= CLSettings.maxSearch) {
					yield return new WaitForSeconds (.1f);
					iSearch = 0;
				}

				// Check if the part is a landing gear/wheel
				if (part.Modules.Contains<ModuleStatusLight> ()) {
					break;
				}

				// Check if part is uncrewed
				if (part.CrewCapacity == 0 || ! CLSettings.useTransferCrew) {

					if (part.Modules.Contains<ModuleColorChanger> ()) {
						ModuleColorChanger partM = part.Modules.GetModule<ModuleColorChanger> ();
						if (Regex.IsMatch(partM.toggleName, "light", RegexOptions.IgnoreCase)) {
							if (CLSettings.onlyNoAGpart) {
								if (!partM.Actions.Contains(KSPActionGroup.Light)) {
									modulesLight.Add (partM);
								}
							} else {
								modulesLight.Add (partM);
							}
						}
					}
					if (part.Modules.Contains<ModuleLight> ()) {
						foreach (ModuleLight partM in part.Modules.GetModules<ModuleLight>()) {
							if (CLSettings.onlyNoAGpart) {
								if (!partM.Actions.Contains(KSPActionGroup.Light)) {
									modulesLight.Add (partM);
								}
							} else {
								modulesLight.Add (partM);
							}
						}
					}
					if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
						foreach (ModuleAnimateGeneric partM in part.Modules.GetModules<ModuleAnimateGeneric>()) {
							if (Regex.IsMatch(partM.actionGUIName, "light", RegexOptions.IgnoreCase)) {
								if (CLSettings.onlyNoAGpart) {
									if (!partM.Actions.Contains(KSPActionGroup.Light)) {
										modulesLight.Add (partM);
									}
								} else {
									modulesLight.Add (partM);
								}
							}
						}
					}
					if (part.Modules.Contains ("WBILight")) {
						foreach (PartModule partM in part.Modules) {
							if (partM.ClassName == "WBILight") {
								if (CLSettings.onlyNoAGpart) {
									if (!partM.Actions.Contains(KSPActionGroup.Light)) {
										modulesLight.Add (partM);
									}
								} else {
									modulesLight.Add (partM);
								}
							}
						}
					}
					if (part.Modules.Contains("ModuleNavLight")) {
						foreach (PartModule partM in part.Modules) {
							if (partM.ClassName == "ModuleNavLight") {
								if (! CLSettings.beaconOnEngine 
								    || (part.name != "lightbeacon.amber" && part.name != "lightbeacon.red")) {
									if (CLSettings.onlyNoAGpart) {
										if (!partM.Actions.Contains(KSPActionGroup.Light)) {
											modulesLight.Add (partM);
										}
									} else {
										modulesLight.Add (partM);
									}
								}
							}
						}
					}
				}
			}
		}

		private void D (String str)
		{
			Debug.Log ("[Crew Light - SunLight] : " + str);
		}
	}
}


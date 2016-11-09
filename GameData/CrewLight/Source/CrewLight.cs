using System;
using System.Collections;
using System.Collections.Generic;
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

		public void Start () 
		{
			GameEvents.onCrewTransferred.Add (UpdateLight);
			GameEvents.onVesselChange.Add (StartLight);
			GameEvents.onVesselGoOffRails.Add (LightOnAnotherVessel);// For the distant illumination
			StartCoroutine ("RoutineLight");
		}

		public void OnDestroy () 
		{
			GameEvents.onCrewTransferred.Remove (UpdateLight);
			GameEvents.onVesselChange.Remove (StartLight);
			GameEvents.onVesselGoOffRails.Remove (LightOnAnotherVessel);// For the distant illumination
		}

		void LightOnAnotherVessel (Vessel vessel) 
		{
			if (vessel != FlightGlobals.ActiveVessel) {
				Debug.Log ("[Crew Light] : Find another vessel : " + vessel.name);
				System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
				stopWatch.Start ();
				List<Part> partWithLight = vessel.Parts.FindAll (p => 
					p.Modules.Contains<ModuleColorChanger> () ||
					p.Modules.Contains<ModuleAnimateGeneric> () ||
					p.Modules.Contains<ModuleLight> () ||
					p.Modules.Contains ("WBILight"));
				stopWatch.Stop ();
				Debug.Log ("[Crew Light] : Checking all the parts on " + vessel.name + " takes " + stopWatch.ElapsedMilliseconds);
				Debug.Log ("[Crew Light] : Start Blinking thingy");
				StartCoroutine (BlinkLight (partWithLight));
			}

//			if (vessel != FlightGlobals.ActiveVessel) {
//				StartCoroutine (BlinkLight (vessel.Parts.FindAll (p => 
//					p.Modules.Contains<ModuleColorChanger> () ||
//					p.Modules.Contains<ModuleAnimateGeneric> () ||
//					p.Modules.Contains<ModuleLight> () ||
//					p.Modules.Contains ("WBILight"))));
//			}
		}
		/* 
		 * Seems to basically work,
		 * need to check for crewed part at the end
		 * need to register the state of the light before blinking them and set it back when done
		 */
		IEnumerator BlinkLight (List<Part> distPartList) 
		{
//			Debug.Log ("[Crew Light] : Start blinking thing, time is : " + Time.time);
			for (int i = 0 ; i < 2 ; i++) {
				foreach (Part part in distPartList) {
					if (part.Modules.Contains<ModuleColorChanger>()) {
						if (part.Modules.GetModule<ModuleColorChanger>().animState == false) {
							part.Modules.GetModule<ModuleColorChanger> ().ToggleEvent ();
						}
					}
					if (part.Modules.Contains<ModuleAnimateGeneric>()) {
						foreach(ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
							if ((anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") && anim.animSwitch == true) {
								anim.Toggle ();
							}
						}
					}
					if (part.Modules.Contains<ModuleLight>()) {
						part.Modules.GetModule<ModuleLight> ().LightsOn ();
					}
				}
				yield return new WaitForSeconds (.7f);
				foreach (Part part in distPartList) {
					if (part.Modules.Contains<ModuleColorChanger>()) {
						part.Modules.GetModule<ModuleColorChanger> ().ToggleEvent ();
					}
					if (part.Modules.Contains<ModuleAnimateGeneric>()) {
						foreach(ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
							if (anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") {
								anim.Toggle ();
							}
						}
					}
					if (part.Modules.Contains<ModuleLight>()) {
						part.Modules.GetModule<ModuleLight> ().LightsOff ();
					}
				}
				yield return new WaitForSeconds(.7f);
			}

//			Debug.Log ("[Crew Light] : Finish blinking thing, time is : " + Time.time);
		}

		IEnumerator RoutineLight () 
		{
			Vessel vessel = FlightGlobals.ActiveVessel;
			if (vessel.crewedParts != 0 && vessel.isEVA == false) {
				yield return new WaitForSeconds (.1f);
				StartLight (vessel);
			}
		}

		private void UpdateLight (GameEvents.HostedFromToAction<ProtoCrewMember, Part> eData) 
		{
			/* Update the status of the lights when a Kerbal moves */
			Light (eData.to);
			LightOff (eData.from);
		}

		private void EVALight (GameEvents.FromToAction<Part, Part> eData) 
		{
			/* Triggered when a kerbal leave a pod by EVA */
			LightOff (eData.from);
		}

		private void StartLight (Vessel vessel) {
			/* Set the lights for a whole vessel */
			foreach (ProtoCrewMember crewMember in vessel.GetVesselCrew()){
				if (crewMember.KerbalRef != null) {// If this is false it should means the Kerbal is in a Command Seat
					Light (crewMember.KerbalRef.InPart);
				}
			}
		}

		private void Light (Part part) {
			/* Send the event that turn on the light, different flavor for different PartModule */
			if (part.Modules.Contains<ModuleColorChanger>()) {
				foreach (ModuleColorChanger anim in part.Modules.GetModules<ModuleColorChanger>()) {
					if (anim.toggleName == "Toggle Lights" && anim.animState == false) {
						anim.ToggleEvent ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleColorChanger");
						return;
					}
				}
			}
			if (part.Modules.Contains<ModuleAnimateGeneric> ()) {
				foreach (ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
					if ((anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") && anim.animSwitch == true) {
						anim.Toggle ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleAnimateGeneric");
						return;
					}
				}
			}
			if (part.Modules.Contains<ModuleLight>()) { // For the Karibou rover, and maybe others...
				foreach (ModuleLight anim in part.Modules.GetModules<ModuleLight>()) {
					if (anim.isOn == false) {
						anim.LightsOn ();
//						Debug.Log ("[Crew Light] : " + part.name + " is lighted by ModuleLight");
						return;
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
						if (anim.toggleName == "Toggle Lights" && anim.animState == true) {
							anim.ToggleEvent ();
							return;
						}
					}
				}
				if (part.Modules.Contains<ModuleAnimateGeneric>()) {
					foreach (ModuleAnimateGeneric anim in part.Modules.GetModules<ModuleAnimateGeneric>()) {
						if ((anim.actionGUIName == "Toggle Lights" || anim.startEventGUIName == "Lights On") && anim.animSwitch == false) {
							anim.Toggle ();
							return;
						}
					}
				}
				if (part.Modules.Contains<ModuleLight>()) {
					foreach (ModuleLight anim in part.Modules.GetModules<ModuleLight>()) {
						if (anim.isOn == true) {
							anim.LightsOff ();
							return;
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
	}
}


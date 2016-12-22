using System;
using System.Collections;
//using System.Collections.Generic;
using UnityEngine;

namespace CrewLight
{
	[KSPAddon(KSPAddon.Startup.Flight, false)]
	public class LightDirector : MonoBehaviour
	{
		public void Start ()
		{
			// CrewLight :
			GameEvents.onCrewTransferred.Add (CrewLightTransfer);
			GameEvents.onVesselChange.Add (CrewLightVessel);
			CrewLightVessel (FlightGlobals.ActiveVessel);

			// MorseLight :
			if (CLSettings.useMorseCode) {
				GameEvents.onVesselLoaded.Add (MorseLightAddVessel);
			}
			// SunLight :
			if (CLSettings.useSunLight) {
				GameEvents.onVesselGoOffRails.Add (SunLightAddVessel);
				GameEvents.onVesselCreate.Add (SunLightAddVessel);
				GameEvents.onVesselPartCountChanged.Add (SunLightVesselChanged);
			}
		}

		public void OnDestroy ()
		{
			// CrewLight :
			GameEvents.onCrewTransferred.Remove (CrewLightTransfer);
			GameEvents.onVesselChange.Remove (CrewLightVessel);

			// MorseLight :
			if (CLSettings.useMorseCode) {
				GameEvents.onVesselLoaded.Remove (MorseLightAddVessel);
			}
			// SunLight :
			if (CLSettings.useSunLight) {
				GameEvents.onVesselGoOffRails.Remove (SunLightAddVessel);
				GameEvents.onVesselCreate.Remove (SunLightAddVessel);
				GameEvents.onVesselPartCountChanged.Remove (SunLightVesselChanged);
			}
		}

		#region CrewLight

		private void CrewLightVessel (Vessel vessel)
		{
			StartCoroutine ("CrewLightVesselRoutine", vessel);
		}

		private IEnumerator CrewLightVesselRoutine (Vessel vessel)
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

		private void CrewLightTransfer (GameEvents.HostedFromToAction<ProtoCrewMember, Part> eData)
		{
			SwitchLight.On (eData.to);
			SwitchLight.Off (eData.from);
		}

		#endregion

		#region MorseLight

		private void MorseLightAddVessel (Vessel vessel)
		{
			if (vessel != FlightGlobals.ActiveVessel) {
				vessel.gameObject.AddOrGetComponent<MorseLight> ();
			}
		}

		#endregion

		#region SunLight

		private void SunLightVesselChanged (Vessel vessel)
		{
			// If a vessel's part count changed then delete all instance of SunLight
			// on this vessel and add a new one, that will automatically search all 
			// parts of the vessel for lightable one
			foreach (Part part in vessel.Parts) {
				if (part.GetComponent<SunLight> () != null) {
					Destroy (part.GetComponent<SunLight> ());
				}
			}
			vessel.gameObject.AddComponent<SunLight> ();
		}

		private void SunLightAddVessel (Vessel vessel)
		{
			if (vessel.loaded) {
				vessel.gameObject.AddOrGetComponent<SunLight> ();
			}
		}

		#endregion

		private void D (String str)
		{
			Debug.Log ("[Crew Light - LightDirector] : " + str);
		}
	}
}

